using System.Collections.Generic;
using System.Threading.Tasks;

// ReSharper disable CheckNamespace

namespace Jinkong.Payment
{
    /// <summary>
    /// 预付订单管理
    /// </summary>
    public interface IPrepayOrderManager
    {
        /// <summary>
        /// 创建预付订单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PrepayOrderDto> Add(AddPrepayOrderInput input);

        /// <summary>
        /// 请求订单支付数据
        /// </summary>
        /// <param name="sn">订单SN</param>
        /// <param name="payType"></param>
        /// <param name="clientId">支付客户端ID</param>
        /// <returns></returns>
        Task<AppPrepayDto> Prepay(string userId, OrderPrepayInput input);

        /// <summary>
        /// 支付结果查询
        /// </summary>
        /// <param name="prepayOrderSn"></param>
        /// <returns></returns>
        Task<PayResult> QueryPayResult(string userId, string prepayOrderSn);

        /// <summary>
        ///  订单支付成功
        /// </summary>
        /// <param name="localTradeNo">本地支付单号</param>
        /// <param name="transactionId">交易平台交易单号</param>
        /// <param name="mchId">商户id</param>
        /// <param name="appId">支付应用appid</param>
        /// <param name="attach">附加数据</param>
        /// <param name="traderId">支付平台userid</param>
        /// <param name="channel">支付通道</param>
        /// <param name="payData">原始回调数据</param>
        /// <returns></returns>
        Task Paid(string localTradeNo, string transactionId, string mchId, string appId, string attach,
            string traderId, PrepayChannel channel, string payData);

        /// <summary>
        /// 申请退款
        /// </summary>
        /// <param name="sourceSn">业务单来源sn</param>
        /// <param name="amount">退款金额</param>
        /// <param name="refundReason">退款原因</param>
        /// <returns></returns>
        Task Refund(string sourceSn, int amount, string refundReason);

        /// <summary>
        /// 退款结果写入
        /// </summary>
        /// <param name="refundNo">支付平台</param>
        /// <param name="localRefundNo">本地</param>
        /// <param name="refundStatus">退款状态</param>
        /// <param name="realRefundAmount">实际退款金额</param>
        /// <param name="successTime">退款成功时间</param>
        /// <param name="failReason">退款失败原因</param>
        /// <param name="originData">原始的退款数据</param>
        /// <returns></returns>
        Task DoRefundResult(string refundNo, string localRefundNo, RefundStatus refundStatus, int realRefundAmount,
            long? successTime, string failReason, string originData);

        /// <summary>
        /// 退款订单查询
        /// </summary>
        /// <param name="localRefundNo"></param>
        /// <returns></returns>
        Task<RefundData> RefundQuery(string localRefundNo);

        /// <summary>
        /// 订单查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<(long total, List<PrepayOrderDto> rows)> Get(PrepayOrderQueryInput input);

        /// <summary>
        /// 按ID查询订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<PrepayOrderDto> GetById(int id, string userId);

        /// <summary>
        /// 按SN查询订单
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        Task<PrepayOrderDto> GetBySn(string sn, string userId);

        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task Cancel(int id);
    }
}