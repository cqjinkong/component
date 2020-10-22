// ReSharper disable CheckNamespace

namespace Jinkong.Payment
{
    /// <summary>
    /// 支付输入参数
    /// </summary>
    public class PayParams
    {
        /// <summary>
        /// 本地预付单ID
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// 本地预付单SN
        /// </summary>
        public string OrderSn { get; set; }

        /// <summary>
        /// 本地交易单号
        /// </summary>
        public string LocalTradeNo { get; set; }

        /// <summary>
        /// 需要支付的金额
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// 订单标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 附加数据
        /// </summary>
        public string Attach { get; set; }

        /// <summary>
        /// 支付通道,枚举PrePayChannel
        /// </summary>
        public PrepayChannel Channel { get; set; }

        ///// <summary>
        ///// 回调域名(需要带https/http)
        ///// </summary>
        //public string Host { get; set; }

        /// <summary>
        /// 客户端IP
        /// </summary>
        public string ClientIp { get; set; }

        /// <summary>
        /// 支付后的重定向地址(H5支付需要)
        /// </summary>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// 三方交易平台用户id(微信openid/支付seller_id)
        /// </summary>
        public string TraderId { get; set; }

        /// <summary>
        /// appid
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 商户id
        /// </summary>
        public string MchId { get; set; }
    }
}
