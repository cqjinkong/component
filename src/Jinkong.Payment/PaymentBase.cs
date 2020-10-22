using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Health.Payment.PayAbstract;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Jinkong.Payment
{
    /// <summary>
    /// 支付通道基类
    /// </summary>
    public abstract class PaymentBase : IPayment
    {
        protected PaymentBase(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            MchOptions = ServiceProvider.GetService<IOptionsSnapshot<PayMchOptions>>();
            Logger = ServiceProvider.GetService<ILoggerFactory>().CreateLogger(GetType());
            Env = serviceProvider.GetService<IHostEnvironment>();
            HttpContextAccessor = ServiceProvider.GetService<IHttpContextAccessor>();
        }

        protected IServiceProvider ServiceProvider { get; }
        protected IOptionsSnapshot<PayMchOptions> MchOptions { get; }
        protected ILogger Logger { get; }
        protected IHttpContextAccessor HttpContextAccessor { get; }
        protected IHostEnvironment Env { get; }

        /// <summary>
        /// 获取商户数据
        /// </summary>
        /// <param name="mchId"></param>
        /// <returns></returns>
        protected MchConfig GetMchData(string mchId)
        {
            var payMchOptions = ServiceProvider.GetService<IOptionsSnapshot<PayMchOptions>>();
            return payMchOptions.Value.Get(mchId);
        }


        public abstract string Name { get; }
        public abstract PrepayChannel Channel { get; }

        /// <summary>
        /// 统一回调接口处理，根据不同的通道转发给不同的类的Notify方法
        /// </summary>
        protected virtual string PayResultNotifyUrl => $"/pay/notify/{Channel}";

        /// <summary>
        /// 统一退款回调
        /// </summary>
        protected virtual string RefundNotifyUrl => $"/refund/notify/{Channel}";

        public abstract Task<string> GetPayResultResponse(PayResult notify);
        public abstract Task<PrepayData> GetPrepayData(PayParams payEntity);
        public abstract TimeSpan GetPayDataExpireTime();

        public abstract Task<PayResult> PayResultNotify(IEnumerable<KeyValuePair<string, string>> query,
            IDictionary<string, string> header, IDictionary<string, string> form, Stream body);

        public abstract Task<(bool success, RefundData refundDetail)> RefundResultNotify(
            IEnumerable<KeyValuePair<string, string>> query, IDictionary<string, string> header,
            IDictionary<string, string> form, Stream body);

        public abstract Task<PayResult> PayQuery(string appId, string mchId, string localTranNo);

        public abstract Task Refund(string appId, string mchId, string localTranNo, string localRefundNo,
            int orderAmount, int refundAmount, string refundReason);

        public abstract Task<RefundData> RefundQuery(string appId, string mchId, string localTranNo,
            string localRefundNo);

        public abstract Task<string> GetRefundResultResponse(bool success, RefundData notify);
        public abstract Task CloseOrder(string appId, string mchId, string localTranNo);
    }
}