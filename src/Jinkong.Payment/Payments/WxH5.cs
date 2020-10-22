using System;
using System.Threading.Tasks;
using Health.Payment.PayAbstract;
using Microsoft.Extensions.Hosting;
using Senparc.CO2NET;
using Shashlik.Kernel.Dependency;

// ReSharper disable CheckNamespace

namespace Jinkong.Payment
{
    /// <summary>
    /// 微信H5支付
    /// </summary>
    public class WxH5 : WxBase, ITransient
    {
        public WxH5(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override string Name => "微信H5";
        public override PrepayChannel Channel => PrepayChannel.WxH5;

        /// <summary>
        /// 获取支付数据
        /// </summary>
        /// <param name="payParams"></param>
        /// <returns></returns>
        public override async Task<PrepayData> GetPrepayData(PayParams payParams)
        {
            if (!Env.IsProduction())
                payParams.Amount = 1;

            var mch = GetMchData(payParams.MchId);
            var result = await WxPay.BuildH5(
                appId: payParams.AppId,
                mchId: mch.MchId,
                mchKey: mch.MchKey,
                title: payParams.Title,
                totalAmount: payParams.Amount,
                attach: payParams.Attach,
                orderId: payParams.OrderId.ToString(),
                localTradeNo: payParams.LocalTradeNo,
                clientIp: payParams.ClientIp,
                notifyUrl: $"{WxsdkOptions.Value.GlobalSettings.ThisHost.Trim().TrimEnd('/')}{PayResultNotifyUrl}",
                redirect_url: payParams.RedirectUrl);
            return new PrepayData(result.Result.MwebUrl, result.UnifiedorderResult.ResultXml);
        }

        public override Task Refund(string appId, string mchId, string localTranNo, string localRefundNo,
            int orderAmount, int refundAmount,
            string refundReason)
        {
            if (!Env.IsProduction())
            {
                orderAmount = 1;
                refundAmount = 1;
            }

            return base.Refund(appId, mchId, localTranNo, localRefundNo, orderAmount, refundAmount, refundReason);
        }
    }
}