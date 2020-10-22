

// ReSharper disable CheckNamespace

namespace Jinkong.Payment
{
    /// <summary>
    /// 商户配置,暂未考虑支付宝支付的配置格式,需要的时候再考虑
    /// </summary>
    public class MchConfig
    {
        /// <summary>
        /// 支付类型,微信/支付宝
        /// </summary>
        public PayType PayType { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        public string MchId { get; set; }

        /// <summary>
        /// 商户密钥
        /// </summary>
        public string MchKey { get; set; }

        /// <summary>
        /// 微信支付:证书路径
        /// </summary>
        public string WxCertPath { get; set; }

        /// <summary>
        /// 微信支付证书密码
        /// </summary>
        public string WxCertPwd { get; set; }
    }
}
