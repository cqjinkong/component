using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Health.Payment;
using Health.Payment.PayAbstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shashlik.AutoMapper;
using Shashlik.Kernel;
using Shashlik.Utils.Extensions;
using Shashlik.EventBus;

namespace Jinkong.Payment.EfStore
{
    public class EfCorePrepayOrderManager : IPrepayOrderManager
    {
        private IEnumerable<IPayment> Payments { get; }
        private PayMchOptions MchOptions { get; }
        private ILock Locker { get; }
        private IServiceProvider ServiceProvider { get; }
        private PaymentDbContext DbContext { get; }
        private IPaySnBuilder PaySnBuilder { get; }
        private IDistributedCache DistributedCache { get; }
        private ILogger<EfCorePrepayOrderManager> Logger { get; }
        private IEventPublisher EventPublisher { get; }

        public EfCorePrepayOrderManager(
            IOptions<PayMchOptions> mchOptions,
            IServiceProvider serviceProvider, ILock locker, PaymentDbContext dbContext, IPaySnBuilder paySnBuilder,
            IDistributedCache distributedCache, ILogger<EfCorePrepayOrderManager> logger,
            IEventPublisher eventPublisher)
        {
            ServiceProvider = serviceProvider;
            Locker = locker;
            DbContext = dbContext;
            PaySnBuilder = paySnBuilder;
            DistributedCache = distributedCache;
            Logger = logger;
            EventPublisher = eventPublisher;
            Payments = serviceProvider.GetServices<IPayment>();
            MchOptions = mchOptions.Value;
        }

        // {0}:prepayOrderId,{1}:pay channel,{2}:appId,{3}:mchId
        const string prepayCacheKey = "PREPAY_ORDER_DATA_{0}_{1}_{2}_{3}";

        public async Task<PrepayOrderDto> Add(AddPrepayOrderInput input)
        {
            using (Locker.Lock(Lockers.PrepayOrderCreate.Format(input.SourceSn), 3))
            {
                var dataProvider = ServiceProvider.GetServices<IPrepayOrderDataProvider>()
                    .SingleOrDefault(p => p.SourceType == input.SourceType);
                if (dataProvider == null)
                    throw new Exception("无效来源或者来源没有数据提供");

                var now = DateTime.Now.GetLongDate();
                var sourceData = await dataProvider.Handle(input.UserId, input.SourceSn);
                if (sourceData.mchName.IsNullOrWhiteSpace() || !MchOptions.Mchs.ContainsKey(sourceData.mchName))
                    throw new Exception("商户名称参数错误");
                // 先查询数据库里有没有已经生成的，如果有就更新数据并直接返回
                //TODO: 价格发生变化时,是否应该更新下面的价格数据.
                var dbOrder = await DbContext.Set<PrepayOrders>()
                    .FirstOrDefaultAsync(o => o.SourceSn == input.SourceSn && o.SourceType == input.SourceType);
                if (dbOrder != null)
                {
                    if (dbOrder.Status == PrepayOrderStatus.Succeed)
                        throw new Exception("订单已付款成功");
                    else if (dbOrder.Status == PrepayOrderStatus.Waiting)
                    {
                        dbOrder.SourceId = sourceData.sourceId;
                        dbOrder.Title = sourceData.title;
                        dbOrder.Attach = sourceData.attach;
                        dbOrder.Amount = sourceData.amount;
                        dbOrder.MchName = sourceData.mchName;
                        await DbContext.SaveChangesAsync();
                        return dbOrder.MapTo<PrepayOrderDto>();
                    }
                }

                if (sourceData.amount == 0)
                {
                    return new PrepayOrderDto()
                    {
                        Amount = 0,
                        CreateTime = now,
                        ExpiresAt = 0,
                        Id = 0,
                        Sn = "",
                        Title = "",
                    };
                }

                var model = new PrepayOrders
                {
                    SourceId = sourceData.sourceId,
                    Amount = sourceData.amount,
                    Attach = sourceData.attach,
                    CreateTime = now,
                    Remark = input.Remark,
                    SourceType = input.SourceType,
                    SourceSn = input.SourceSn,
                    Sn = PaySnBuilder.BuildSn(),
                    UserId = input.UserId.ToString(),
                    UserName = input.UserName,
                    UserExtends = input.UserExtends,
                    UserPhone = input.UserPhone,
                    Status = PrepayOrderStatus.Waiting,
                    Title = sourceData.title,
                    MchName = sourceData.mchName
                };

                DbContext.Add(model);
                await DbContext.SaveChangesAsync();
                return model.MapTo<PrepayOrderDto>();
            }
        }

        /// <summary>
        /// 准备支付,请求支付数据
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<AppPrepayDto> Prepay(string userId, OrderPrepayInput input)
        {
            using var locker = Locker.Lock(Lockers.PrepayOrderGetPayData.Format(input.PrepayOrderSn), 3);
            var order = await DbContext.Set<PrepayOrders>()
                .FirstOrDefaultAsync(o => o.Sn == input.PrepayOrderSn && o.UserId == userId);
            if (order == null)
                throw new Exception("订单不存在");
            if (order.Status != PrepayOrderStatus.Waiting)
                throw new Exception("订单状态错误");

            var now = DateTime.Now.GetLongDate();
            var mch = MchOptions.GetByName(order.MchName, input.Channel.Convert2Type());
            if (mch == null)
                throw new Exception("channel 参数错误");

            var dataProvider = ServiceProvider.GetServices<IPrepayOrderDataProvider>()
                .SingleOrDefault(p => p.SourceType == order.SourceType);
            if (dataProvider == null)
                throw new Exception("无效来源或者来源没有数据提供");

            var canpay = await dataProvider.CanPay(userId.ParseTo<int>(), order.SourceSn);
            if (!canpay.canPay)
                throw new Exception(canpay.errorReason ?? "订单异常");

            var cacheKey = prepayCacheKey.Format(order.Id, input.Channel, input.AppId, mch.MchId);
            // 缓存中查看有没有支付对应的支付数据,有就直接返回,没有就调用对应的支付通道并获取
            var prepayData = await DistributedCache.GetObjectAsync<PrepayData>(cacheKey);
            if (prepayData == null)
            {
                var oldLocalTradeNo = order.CurrentPayLocalTradeNo;
                var newLocalTradeNo = Guid.NewGuid().ToString("n");
                var payParams = new PayParams()
                {
                    OrderId = order.Id,
                    OrderSn = order.Sn,
                    LocalTradeNo = newLocalTradeNo,
                    Amount = order.PayAmount,
                    Title = order.Title,
                    Attach = order.Attach,
                    ClientIp = input.ClientIp,
                    TraderId = input.TraderId,
                    RedirectUrl = input.RedirectUrl,
                    Channel = input.Channel,
                    MchId = mch.MchId,
                    AppId = input.AppId
                };
                var payment = Payments.FirstOrDefault(r => r.Channel == input.Channel);
                if (payment == null)
                    throw new Exception("无效的支付方式");

                if (!oldLocalTradeNo.IsNullOrWhiteSpace())
                {
                    try
                    {
                        // 每次请求新的支付数据时,都将旧的预付单关闭掉,避免重复支付
                        await payment.CloseOrder(input.AppId, mch.MchId, oldLocalTradeNo);
                    }
                    catch
                    {
                        // ignored
                    }
                }

                order.CurrentPayLocalTradeNo = newLocalTradeNo;
                prepayData = await payment.GetPrepayData(payParams);
                await DistributedCache.SetObjectAsync(cacheKey, prepayData, DateTimeOffset.Now.AddHours(1));
            }

            try
            {
                var result = new AppPrepayDto
                {
                    PayData = prepayData.PayData
                };

                order.PayChannel = input.Channel;
                order.PrepayOriginData = prepayData.PrepayOriginData;
                order.MchId = mch.MchId;
                order.LastPrepayTime = now;
                order.AppId = input.AppId;

                await DbContext.SaveChangesAsync();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception("支付数据生成失败", e);
            }
        }

        /// <summary>
        /// 支付成功
        /// </summary>
        /// <param name="localTradeNo"></param>
        /// <param name="transactionId"></param>
        /// <param name="mchId"></param>
        /// <param name="appId"></param>
        /// <param name="attach"></param>
        /// <param name="traderId"></param>
        /// <param name="channel"></param>
        /// <param name="payData"></param>
        /// <returns></returns>
        public async Task Paid(string localTradeNo, string transactionId, string mchId, string appId, string attach,
            string traderId, PrepayChannel channel, string payData)
        {
            using var _ = Locker.Lock(Lockers.PrepayOrderStatusChanged.Format(localTradeNo), 3);

            var order = await DbContext.Set<PrepayOrders>()
                .FirstOrDefaultAsync(r => r.CurrentPayLocalTradeNo == localTradeNo);
            if (order == null)
                throw new Exception($"支付完成,预付单不存在,localTradeNo: {localTradeNo}");
            if (order.MchId != mchId)
                Logger.LogWarning($"预付单支付成功, 商户id与原支付订单商户id不一致,可能是切换了支付方式. orderId: {order.Id}");
            if (order.AppId != appId)
                Logger.LogWarning($"预付单支付成功, appid与原支付订单appid不一致,可能是切换了支付方式. orderId: {order.Id}");
            if (order.PayChannel != channel)
                Logger.LogWarning($"预付单支付成功, channel与原支付订单channel不一致,可能是切换了支付方式. orderId: {order.Id}");
            if (order.Attach != attach)
                Logger.LogWarning($"预付单支付成功, attach与原支付订单attach不一致,可能是切换了支付方式. orderId: {order.Id}");
            if (order.Status == PrepayOrderStatus.Succeed)
            {
                Logger.LogWarning($"预付单支付成功, 但订单状态为已支付成功, 可能是多次回调或者完成了支付查询. orderId: {order.Id}");
                return;
            }
            else if (order.Status != PrepayOrderStatus.Waiting)
            {
                Logger.LogWarning($"预付单支付成功, 但订单状态为{order.Status}. orderId: {order.Id}");
                return;
            }

            var tran = DbContext.Database.BeginTransaction();
            try
            {
                order.Status = PrepayOrderStatus.Succeed;
                order.PayTime = DateTime.Now.GetLongDate();
                order.TransactionId = transactionId;
                order.TraderId = traderId;
                order.PayChannel = channel;
                order.PayNotifyOriginData = payData;
                order.MchId = mchId;
                order.AppId = appId;
                order.Attach = attach;

                await DbContext.SaveChangesAsync();
                var paidData = new OrderPaidData()
                {
                    Id = order.Id,
                    Amount = order.Amount,
                    PayTime = order.PayTime.Value,
                    Sn = order.Sn,
                    SourceId = order.SourceId,
                    SourceSn = order.SourceSn,
                    SourceType = order.SourceType,
                    TransactionId = transactionId,
                    TraderId = traderId,
                    UserId = order.UserId,
                    UserName = order.UserName,
                    UserPhone = order.UserPhone,
                    OrderAttach = order.Attach,
                    Channel = order.PayChannel.Value
                };

                var dataProvider = ServiceProvider.GetServices<IPrepayOrderDataProvider>()
                    .SingleOrDefault(p => p.SourceType == order.SourceType);
                if (dataProvider == null)
                    throw new Exception("无效来源");
                await dataProvider.PaySuccess(paidData);

                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                Logger.LogError(ex, "订单付款完成处理失败");
                throw;
            }
        }

        public async Task<PrepayOrderDto> GetBySn(string sn, string userId)
        {
            return await DbContext.Set<PrepayOrders>()
                .Where(m => m.Sn == sn)
                .WhereIf(!userId.IsNullOrWhiteSpace(), r => r.UserId == userId)
                .QueryTo<PrepayOrderDto>()
                .FirstOrDefaultAsync();
        }

        public async Task<PrepayOrderDto> GetById(int id, string userId)
        {
            return await DbContext.Set<PrepayOrders>()
                .Where(m => m.Id == id)
                .WhereIf(!userId.IsNullOrWhiteSpace(), r => r.UserId == userId)
                .QueryTo<PrepayOrderDto>()
                .FirstOrDefaultAsync();
        }

        public async Task<(long total, List<PrepayOrderDto> rows)> Get(PrepayOrderQueryInput input)
        {
            int? begin = null, end = null;
            if (input.BeginDate.HasValue)
                begin = input.BeginDate.Value.Date.GetIntDate();
            if (input.EndDate.HasValue)
                end = input.EndDate.Value.Date.AddDays(1).GetIntDate();

            var query =
                DbContext.Set<PrepayOrders>()
                    .WhereIf(!input.UserId.IsNullOrWhiteSpace(), r => r.UserId == input.UserId)
                    .WhereIf(input.Status.HasValue, r => r.Status == input.Status)
                    .WhereIf(begin.HasValue, r => r.CreateTime >= begin)
                    .WhereIf(end.HasValue, r => r.CreateTime < end)
                    .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), r => r.UserName.Contains(input.Keyword)
                                                                       || r.UserPhone.Contains(input.Keyword)
                                                                       || r.Sn.Contains(input.Keyword));

            var total = query.Count();
            var rows =
                await
                    query.OrderByDescending(r => r.Id)
                        .DoPage(input.PageIndex, input.PageSize)
                        .QueryTo<PrepayOrderDto>()
                        .ToListAsync();
            return (total, rows);
        }

        public async Task Cancel(int id)
        {
            using (Locker.Lock(Lockers.PrepayOrderStatusChanged.Format(id), 3))
            {
                var order = await DbContext.Set<PrepayOrders>().FirstOrDefaultAsync(r => r.Id == id);
                if (order == null)
                    throw new Exception("订单不存在");

                order.Status = PrepayOrderStatus.Canceled;
                await DbContext.SaveChangesAsync();
            }
        }

        public async Task<PayResult> QueryPayResult(string userId, string prepayOrderSn)
        {
            var order = await DbContext.Set<PrepayOrders>().AsNoTracking()
                .Where(r => r.Sn == prepayOrderSn && r.UserId == userId)
                .Select(o => new
                {
                    o.CurrentPayLocalTradeNo,
                    o.Status,
                    o.Amount,
                    o.AppId,
                    o.TraderId,
                    o.TransactionId,
                    o.MchId,
                    o.Attach,
                    o.PayChannel,
                    o.PayNotifyOriginData
                })
                .FirstOrDefaultAsync();
            if (order == null)
                throw new Exception($"订单不存在");
            // 没有请求支付数据,就没有支付单号,返回false
            if (order.CurrentPayLocalTradeNo.IsNullOrWhiteSpace())
                return new PayResult
                {
                    Success = false
                };

            if (order.Status == PrepayOrderStatus.Succeed)
                return new PayResult
                {
                    Amount = order.Amount,
                    Attach = order.Attach,
                    Channel = order.PayChannel!.Value,
                    Message = "success",
                    RealPayAmount = order.Amount,
                    AppId = order.AppId,
                    LocalTradeNo = order.CurrentPayLocalTradeNo,
                    Success = true,
                    TraderId = order.TraderId,
                    TransactionId = order.TransactionId,
                    MchId = order.MchId,
                    PayData = order.PayNotifyOriginData
                };

            var handler = Payments.FirstOrDefault(r => r.Channel == order.PayChannel);
            if (handler == null)
                Logger.LogError($"错误的支付通道: {order.PayChannel}");

            var payResult = await handler.PayQuery(order.AppId, order.MchId, order.CurrentPayLocalTradeNo);
            if (payResult.Success)
            {
                await Paid(order.CurrentPayLocalTradeNo, payResult.TransactionId, payResult.MchId, payResult.AppId,
                    payResult.Attach, payResult.TraderId, payResult.Channel, payResult.PayData);
                return payResult;
            }

            return new PayResult {Success = false};
        }

        /// <summary>
        /// 发起退款
        /// </summary>
        /// <param name="sourceSn"></param>
        /// <param name="amount"></param>
        /// <param name="refundReason"></param>
        /// <returns></returns>
        public async Task Refund(string sourceSn, int amount, string refundReason)
        {
            if (amount <= 0)
                throw new Exception($"金额错误");
            var order = await DbContext.Set<PrepayOrders>().Include(r => r.RefundOrders)
                .FirstOrDefaultAsync(r => r.SourceSn == sourceSn);
            if (order == null || order.Status != PrepayOrderStatus.Succeed)
                throw new Exception("支付状态错误");
            if (order.PayAmount <= order.RefundOrders.Sum(r => r.RefundAmount))
                throw new Exception("订单已全部退款完成");
            if (amount > order.PayAmount)
                throw new Exception("退款金额错误");

            var mch = MchOptions.Get(order.MchId);
            if (mch == null)
                throw new Exception($"商户数据异常,无法退款,mchId:{order.MchId}");

            var payment = Payments.FirstOrDefault(r => r.Channel == order.PayChannel.Value);
            if (payment == null)
                throw new Exception($"支付通道错误,order channel:{order.PayChannel}");

            string localRefundNo = Guid.NewGuid().ToString("n");
            using var locker = Locker.Lock(Lockers.RefundOrderCreate.Format(order.Id), 3);
            using var tran = DbContext.Database.BeginTransaction();
            try
            {
                RefundOrders refundOrder = new RefundOrders
                {
                    PrepayOrderId = order.Id,
                    CreateTime = DateTime.Now.GetLongDate(),
                    LocalRefundNo = localRefundNo,
                    RefundStatus = RefundStatus.RefundProcessing,
                    RefundAmount = amount
                };
                DbContext.Add(refundOrder);
                await DbContext.SaveChangesAsync();

                // 发起退款请求
                await payment.Refund(order.AppId, order.MchId, order.CurrentPayLocalTradeNo, localRefundNo,
                    order.Amount, amount, refundReason);
                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                throw ex;
            }
        }

        /// <summary>
        /// 退款结果
        /// </summary>
        /// <param name="localRefundNo">本地退款单号</param>
        /// <param name="refundStatus">退款状态</param>
        /// <param name="realRefundAmount">实际退款金额</param>
        /// <param name="successTime">成功退款时间</param>
        /// <param name="failReason">退款失败的原因</param>
        /// <param name="originData">http原始数据</param>
        /// <returns></returns>
        public async Task DoRefundResult(string refundNo, string localRefundNo, RefundStatus refundStatus,
            int realRefundAmount,
            long? successTime, string failReason, string originData)
        {
            if (refundStatus == RefundStatus.RefundProcessing)
                return;

            var refund = await DbContext.Set<RefundOrders>().Include(r => r.PrepayOrder)
                .FirstOrDefaultAsync(r => r.LocalRefundNo == localRefundNo);
            if (refund == null)
                throw new Exception($"退款订单不存在,localRefundNo: {localRefundNo}");

            if (refund.RefundStatus != RefundStatus.RefundProcessing)
                throw new Exception($"退款异常,状态错误,localRefundNo: {localRefundNo},数据库状态: {refund.RefundStatus}");

            refund.RefundStatus = refundStatus;
            refund.SuccessTime = successTime;
            refund.RealRefundAmount = realRefundAmount;
            refund.FailReason = failReason;
            refund.OriginalData = originData;
            refund.RefundNo = refundNo;

            using var locker = Locker.Lock(Lockers.RefundOrderCreate.Format(refund.Id), 3);
            using var tran = DbContext.Database.BeginTransaction();
            try
            {
                await DbContext.SaveChangesAsync();
                await EventPublisher.PublishAsync(new RefundOrderResultEvent
                {
                    FailReason = failReason,
                    RealRefundAmount = realRefundAmount,
                    SourceType = refund.PrepayOrder.SourceType,
                    SourceSn = refund.PrepayOrder.SourceSn,
                    Status = refundStatus,
                    SuccessTime = successTime
                });
                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                throw ex;
            }
        }

        public async Task<RefundData> RefundQuery(string localRefundNo)
        {
            var refundOrder = await DbContext.Set<RefundOrders>().Include(r => r.PrepayOrder)
                .FirstOrDefaultAsync(r => r.LocalRefundNo == localRefundNo);
            if (refundOrder == null)
                throw new Exception($"订单不存在");

            var payment = Payments.Single(r => r.Channel == refundOrder.PrepayOrder.PayChannel);
            return await payment.RefundQuery(refundOrder.PrepayOrder.AppId, refundOrder.PrepayOrder.MchId,
                refundOrder.PrepayOrder.Sn, refundOrder.LocalRefundNo);
        }
    }
}