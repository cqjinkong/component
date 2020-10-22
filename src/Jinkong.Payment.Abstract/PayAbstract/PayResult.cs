// ReSharper disable CheckNamespace
namespace Jinkong.Payment
{
    /// <summary>
    /// 支付回结果
    /// </summary>
    public class PayResult
    {
        /// <summary>
        /// 商户id
        /// </summary>
        public string MchId { get; set; }

        /// <summary>
        /// 支付完成的应用appid
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 是否成功支付
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// 实际支付金额
        /// </summary>
        public int RealPayAmount { get; set; }

        /// <summary>
        /// 支付平台流水号
        /// </summary>
        public string TransactionId { get; set; }

        /// <summary>
        /// 本地订单编号,orderSn
        /// </summary>
        public string LocalTradeNo { get; set; }

        /// <summary>
        /// 支付问题，未成功回调时显示提示
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 附加数据
        /// </summary>
        public string Attach { get; set; }

        /// <summary>
        /// 交易用户ID(微信openid/支付宝uid等)
        /// </summary>
        public string TraderId { get; set; }

        /// <summary>
        /// 实际支付通道
        /// </summary>
        public PrepayChannel Channel { get; set; }

        /// <summary>
        /// 原始回调数据
        /// </summary>
        public string PayData { get; set; }
    }
}
