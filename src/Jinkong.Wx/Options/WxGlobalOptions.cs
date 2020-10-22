// ReSharper disable CheckNamespace

namespace Jinkong.Wx
{
    /// <summary>
    /// 全局设置
    /// </summary>
    public class WxGlobalOptions
    {
        /// <summary>
        /// 当前域名(api域名,微信回调用)
        /// </summary>
        public string ThisHost { get; set; }

        /// <summary>
        /// 代理 主机
        /// </summary>
        public string ProxyHost { get; set; }
        /// <summary>
        /// 代理 端口
        /// </summary>
        public int? ProxyPort { get; set; }
        /// <summary>
        /// 代理 用户名 匿名则不填
        /// </summary>
        public string ProxyUserName { get; set; }
        /// <summary>
        /// 代理 密码匿名则不填
        /// </summary>
        public string ProxyPassword { get; set; }
    }
}
