using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Jinkong.Wx;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Senparc.CO2NET.Utilities;
using Senparc.Weixin.TenPay.V3;
using Shashlik.Utils.Extensions;

// ReSharper disable CheckNamespace

namespace Jinkong.Payment
{
    public abstract class WxBase : PaymentBase
    {
        protected WxBase(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            WxPay = serviceProvider.GetService<IWxPay>();
            WxsdkOptions = serviceProvider.GetService<IOptions<WxOptions>>();
        }

        protected IWxPay WxPay { get; }
        protected IOptions<WxOptions> WxsdkOptions { get; }

        public override Task<string> GetPayResultResponse(PayResult notify)
        {
            // 订单正确和错误的返回给微信的结果数据
            string errStr = @"
<xml>
    <return_code><![CDATA[FAIL]]></return_code>
    <return_msg><![CDATA[支付失败]]></return_msg>
</xml>";

            string rightStr = @"
<xml>
    <return_code><![CDATA[SUCCESS]]></return_code>
    <return_msg><![CDATA[支付成功]]></return_msg>
</xml>";
            return notify.Success ? Task.FromResult(rightStr) : Task.FromResult(errStr);
        }

        public override async Task<PayResult> PayResultNotify(
            IEnumerable<KeyValuePair<string, string>> query,
            IDictionary<string, string> header,
            IDictionary<string, string> form,
            Stream body)
        {
            var notifyResult = new PayResult()
            {
                Success = false
            };

            string xml = await body.ReadToStringAsync();
            if (xml.IsNullOrWhiteSpace())
            {
                notifyResult.Message = "微信支付回调,xml请求数据为空!";
                Logger.LogError(notifyResult.Message);
                return notifyResult;
            }

            ResponseHandler resHandler;
            OrderQueryResult result;
            try
            {
                // 获取微信回调数据
                // ResponseHandler读取body时没有seek,先seek下
                HttpContextAccessor.HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);
                resHandler = new ResponseHandler(HttpContextAccessor.HttpContext);

                Logger.LogInformation($"微信支付回调:" + xml);
                // 转换为对象
                result = new OrderQueryResult(xml);
            }
            catch (XmlException)
            {
                // xml格式异常直接不管
                notifyResult.Message = "微信支付回调,xml数据异常!";
                //logger.LogError(notifyResult.Message);
                return notifyResult;
            }
            catch (Exception ex)
            {
                notifyResult.Message = "微信支付回调,数据解析错误!";
                Logger.LogError(ex, notifyResult.Message);
                return notifyResult;
            }

            var secret = MchOptions.Value.Get(result.mch_id)?.MchKey;
            if (secret.IsNullOrWhiteSpace())
            {
                notifyResult.Message = $"微信支付回调,未找到商户密钥,mchId:{result.mch_id},result:{xml}";
                return notifyResult;
            }

            // 设置appkey,用于验证参数签名
            resHandler.SetKey(secret);
            // 参数签名验证
            if (!resHandler.IsTenpaySign())
            {
                notifyResult.Message = $"微信支付回调:签名错误,result:{xml}";
                Logger.LogError(notifyResult.Message);
                return notifyResult;
            }

            // 支付回调结果是否正确
            if (!result.IsReturnCodeSuccess() || !result.IsResultCodeSuccess())
            {
                notifyResult.Message = $"微信支付回调,支付失败:return error,result:{xml}";
                Logger.LogError(notifyResult.Message);
                return notifyResult;
            }

            notifyResult.Success = true;
            notifyResult.RealPayAmount = notifyResult.Amount = result.total_fee.ConvertTo<int>();
            notifyResult.LocalTradeNo = result.out_trade_no;
            notifyResult.TransactionId = result.transaction_id;
            notifyResult.Attach = result.attach;
            notifyResult.TraderId = result.openid;
            notifyResult.PayData = xml;
            notifyResult.MchId = result.mch_id;
            notifyResult.AppId = result.appid;

            return await Task.FromResult(notifyResult);
        }

        public override TimeSpan GetPayDataExpireTime()
        {
            //微信默认支付时间1小时
            return TimeSpan.FromHours(1);
        }

        protected PrepayChannel ConvertToChannel(string wxTradeType)
        {
            if (wxTradeType == "JSAPI")
                return PrepayChannel.WxJs;
            if (wxTradeType == "NATIVE")
                return PrepayChannel.WxNative;
            if (wxTradeType == "APP")
                return PrepayChannel.WxApp;
            if (wxTradeType == "MWEB")
                return PrepayChannel.WxH5;

            throw new Exception("未知的微信支付类型trade_type:" + wxTradeType);
        }

        public override async Task<PayResult> PayQuery(string appId, string mchId, string localTranNo)
        {
            await Task.CompletedTask;
            if (string.IsNullOrWhiteSpace(appId))
                throw new ArgumentException("appId can not be empty.", nameof(appId));
            if (string.IsNullOrWhiteSpace(mchId))
                throw new ArgumentException("mchId can not be empty.", nameof(mchId));
            if (string.IsNullOrWhiteSpace(localTranNo))
                throw new ArgumentException("localTranNo can not be empty.", nameof(localTranNo));

            try
            {
                var mch = GetMchData(mchId);
                if (WxPay.TryOrderQuery(appId, mch.MchId, mch.MchKey, localTranNo, out var orderResult) &&
                    orderResult.trade_state == "SUCCESS")
                {
                    return new PayResult
                    {
                        Success = true,
                        Amount = orderResult.total_fee.ConvertTo<int>(),
                        Attach = orderResult.attach,
                        Channel = ConvertToChannel(orderResult.trade_type),
                        LocalTradeNo = orderResult.out_trade_no,
                        Message = orderResult.return_msg,
                        RealPayAmount = orderResult.total_fee.ConvertTo<int>(),
                        TraderId = orderResult.openid,
                        TransactionId = orderResult.transaction_id,
                        PayData = orderResult.ResultXml,
                        MchId = orderResult.mch_id,
                        AppId = orderResult.appid
                    };
                }
                else
                {
                    return new PayResult
                    {
                        Success = false
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "微信支付结果查询错误");
                return new PayResult
                {
                    Success = false
                };
            }
        }

        public override async Task Refund(string appId, string mchId, string localTranNo, string localRefundNo,
            int orderAmount, int refundAmount, string refundReason)
        {
            var mch = MchOptions.Value.Get(mchId);
            if (mch == null)
                throw new Exception($"退款请求异常,不存在的商户id:{mchId}");

            var res = await WxPay.Refund(
                appId,
                mchId,
                mch.MchKey,
                localTranNo,
                localRefundNo,
                orderAmount,
                refundAmount,
                refundReason,
                $"{WxsdkOptions.Value.GlobalSettings.ThisHost.Trim().TrimEnd('/')}{RefundNotifyUrl}");
            if (!res.success)
                throw new Exception($"退款请求失败, xml: {res.originXml}");
        }

        public override async Task<RefundData> RefundQuery(string appId, string mchId, string localTranNo,
            string localRefundNo)
        {
            await Task.CompletedTask;
            var mch = MchOptions.Value.Get(mchId);
            if (mch == null)
                throw new Exception($"退款请求异常,不存在的商户id:{mchId}");

            if (WxPay.TryRefundQuery(appId, mchId, mch.MchKey, localRefundNo, out var res))
            {
                var item = res.List.FirstOrDefault(r => r.LocalRefundNo == localRefundNo);
                if (item == null)
                    return null;

                return new RefundData
                {
                    FailReason = "",
                    LocalRefundNo = localRefundNo,
                    RealRefundAmount = item.RealRefundAmount,
                    RefundNo = item.RefundId,
                    Status = ToStatus(item.Status),
                    SuccessTime = item.SuccessTime
                };
            }

            return null;
        }

        public override async Task<(bool success, RefundData refundDetail)> RefundResultNotify(
            IEnumerable<KeyValuePair<string, string>> query,
            IDictionary<string, string> header,
            IDictionary<string, string> form,
            Stream body)
        {
            await Task.CompletedTask;
            var xml = body.ReadToString();
            if (xml.IsNullOrWhiteSpace())
            {
                Logger.LogWarning("微信退款回调,xml数据为空");
                return (false, null);
            }

            ResponseHandler resHandler;
            try
            {
                // 获取微信回调数据
                resHandler = new ResponseHandler(HttpContextAccessor.HttpContext);

                string return_code = resHandler.GetParameter("return_code");
                string return_msg = resHandler.GetParameter("return_msg");
                if (return_code != "SUCCESS")
                {
                    return (false, null);
                }

                string appId = resHandler.GetParameter("appid");
                string mch_id = resHandler.GetParameter("mch_id");
                string nonce_str = resHandler.GetParameter("nonce_str");
                string req_info = resHandler.GetParameter("req_info");

                var secret = MchOptions.Value.Get(mch_id)?.MchKey;
                if (secret.IsNullOrWhiteSpace())
                {
                    Logger.LogError($"微信支付回调,未找到商户密钥,mchId:{mch_id},result:{xml}");
                    return (false, null);
                }

                var decodeReqInfo = TenPayV3Util.DecodeRefundReqInfo(req_info, secret);
                var decodeDoc = XDocument.Parse(decodeReqInfo);

                //获取接口中需要用到的信息
                string transaction_id = decodeDoc.Root.Element("transaction_id").Value;
                string out_trade_no = decodeDoc.Root.Element("out_trade_no").Value;
                string refund_id = decodeDoc.Root.Element("refund_id").Value;
                string out_refund_no = decodeDoc.Root.Element("out_refund_no").Value;
                int total_fee = int.Parse(decodeDoc.Root.Element("total_fee").Value);
                int? settlement_total_fee = decodeDoc.Root.Element("settlement_total_fee") != null
                    ? int.Parse(decodeDoc.Root.Element("settlement_total_fee").Value)
                    : null as int?;
                int refund_fee = int.Parse(decodeDoc.Root.Element("refund_fee").Value);
                int tosettlement_refund_feetal_fee = int.Parse(decodeDoc.Root.Element("settlement_refund_fee").Value);
                string refund_status = decodeDoc.Root.Element("refund_status").Value;
                string success_time = decodeDoc.Root.Element("success_time").Value;
                string refund_recv_accout = decodeDoc.Root.Element("refund_recv_accout").Value;
                string refund_account = decodeDoc.Root.Element("refund_account").Value;
                string refund_request_source = decodeDoc.Root.Element("refund_request_source").Value;

                var refundDetail = new RefundData
                {
                    FailReason = "",
                    LocalRefundNo = out_refund_no,
                    RefundNo = refund_id,
                    RealRefundAmount = tosettlement_refund_feetal_fee,
                    Status = ToStatus(refund_status),
                    SuccessTime = success_time.IsNullOrWhiteSpace()
                        ? null
                        : (long?) success_time.ConvertTo<DateTime>().GetLongDate(),
                    OriginData = decodeReqInfo
                };
                return (true, refundDetail);
            }
            catch (XmlException)
            {
                Logger.LogWarning("微信退款回调,xml数据解析错误");
                return (false, null);
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "微信退款回调数据处理异常");
                return (false, null);
            }
        }

        private static RefundStatus ToStatus(string refundStatus)
        {
            switch (refundStatus)
            {
                case "SUCCESS": return RefundStatus.RefundSuccess;
                case "CHANGE": return RefundStatus.RefundProcessing;
                case "REFUNDCLOSE": return RefundStatus.RefundError;
                default: throw new Exception("微信退款回调状态异常");
            }
        }

        private static RefundStatus ToStatus(WxRefundListData.RefundStatus refundStatus)
        {
            switch (refundStatus)
            {
                case WxRefundListData.RefundStatus.SUCCESS: return RefundStatus.RefundSuccess;
                case WxRefundListData.RefundStatus.PROCESSING: return RefundStatus.RefundProcessing;
                case WxRefundListData.RefundStatus.CHANGE: return RefundStatus.RefundError;
                case WxRefundListData.RefundStatus.REFUNDCLOSE: return RefundStatus.RefundClose;
                default: throw new Exception("微信退款回调状态异常");
            }
        }

        public override Task<string> GetRefundResultResponse(bool success, RefundData notify)
        {
            return Task.FromResult(string.Format(@"<xml>
<return_code><![CDATA[{0}]]></return_code>
<return_msg><![CDATA[{1}]]></return_msg>
</xml>", success ? "SUCCESS" : "FAIL", success ? "OK" : "FAIL"));
        }

        public override Task CloseOrder(string appId, string mchId, string localTranNo)
        {
            var mch = MchOptions.Value.Get(mchId);
            if (mch == null)
                throw new Exception($"关闭订单请求异常,不存在的商户id:{mchId}");

            WxPay.CloseOrder(appId, mchId, mch.MchKey, localTranNo);
            return Task.CompletedTask;
        }
    }
}