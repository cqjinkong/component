using System;
using System.Threading.Tasks;
using Health.Payment.PayAbstract;
using Microsoft.Extensions.Hosting;
using Shashlik.Utils.Extensions;

// ReSharper disable CheckNamespace

namespace Jinkong.Payment
{
    public class WxApp : WxBase, Shashlik.Kernel.Dependency.ITransient
    {
        public WxApp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override string Name => "微信APP";
        public override PrepayChannel Channel => PrepayChannel.WxApp;

        public override async Task<PrepayData> GetPrepayData(PayParams payParams)
        {
            if (!Env.IsProduction())
                payParams.Amount = 1;

            var mch = GetMchData(payParams.MchId);
            var result = await WxPay.BuildApp(
                payParams.AppId,
                mch.MchId,
                mch.MchKey,
                payParams.Title,
                payParams.Amount,
                payParams.Attach,
                payParams.OrderId.ToString(),
                payParams.LocalTradeNo,
                payParams.ClientIp,
                $"{WxsdkOptions.Value.GlobalSettings.ThisHost.Trim().TrimEnd('/')}{PayResultNotifyUrl}");
            return new PrepayData(result.Result.ToJsonWithCamelCase(),
                result.UnifiedorderResult.ResultXml);
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