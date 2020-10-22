using System;
using System.Threading.Tasks;
using Health.Payment.PayAbstract;
using Microsoft.Extensions.Hosting;
using Shashlik.Kernel.Dependency;

// ReSharper disable CheckNamespace

namespace Jinkong.Payment
{
    public class WxNative : WxBase, ITransient
    {
        public WxNative(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override string Name => "微信Native";
        public override PrepayChannel Channel => PrepayChannel.WxNative;

        public override async Task<PrepayData> GetPrepayData(PayParams payEntity)
        {
            if (!Env.IsProduction())
                payEntity.Amount = 1;

            var mch = GetMchData(payEntity.MchId);
            var result = await WxPay.BuildWxPayQrCode(
                payEntity.AppId,
                mch.MchId,
                mch.MchKey,
                payEntity.Title,
                payEntity.Amount,
                payEntity.Attach,
                payEntity.OrderId.ToString(),
                payEntity.LocalTradeNo,
                $"{WxsdkOptions.Value.GlobalSettings.ThisHost}{PayResultNotifyUrl}");
            return new PrepayData(result.Result.QrcodeUrl, result.UnifiedorderResult.ResultXml);
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