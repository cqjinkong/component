using System.ComponentModel;
// ReSharper disable CheckNamespace

namespace Jinkong.Payment
{
    /// <summary>
    /// 预支付订单支付通道
    /// </summary>
    public enum PrepayChannel
    {
        /// <summary>
        /// 支付宝H5
        /// </summary>
        [Description("支付宝H5")] AliPayH5 = 1001,

        /// <summary>
        /// 微信Native支付
        /// </summary>
        [Description("微信Native支付")] WxNative = 2001,

        /// <summary>
        /// 微信H5
        /// </summary>
        [Description("微信H5")] WxH5 = 2002,

        /// <summary>
        /// 微信APP支付
        /// </summary>
        [Description("微信APP支付")] WxApp = 2003,

        /// <summary>
        /// 微信Js支付
        /// </summary>
        [Description("微信JS支付")] WxJs = 2004,
    }
}