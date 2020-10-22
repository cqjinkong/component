using System.ComponentModel;
// ReSharper disable CheckNamespace

namespace Jinkong.Payment
{
    /// <summary>
    /// 支付方式
    /// </summary>
    [Description("支付方式")]
    public enum PayType
    {
        /// <summary>
        /// 余额支付
        /// </summary>
        [Description("余额支付")] Balance = 1,

        /// <summary>
        /// 微信支付
        /// </summary>
        [Description("微信支付")] WxPay = 2,

        /// <summary>
        /// 支付宝支付
        /// </summary>
        [Description("支付宝支付")] AliPay = 4,

        /// <summary>
        /// 银联支付
        /// </summary>
        [Description("银联支付")] UnionPay = 8,
    }
}