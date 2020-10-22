using Shashlik.Kernel.Autowired.Attributes;

namespace Jinkong.Wx.AspNetCore
{
    /// <summary>
    /// wx api参数
    /// </summary>
    [AutoOptions("Jinkong.Wx.AspNetCore")]
    public class WxAspNetCoreOptions
    {

        /// <summary>
        /// 获取jssdk 配置对象
        /// </summary>
        public string WxJsSdk { get; set; } = "/wxapi/jssdk";

        /// <summary>
        /// 微信H5登录url
        /// </summary>
        public string OAuthUrl { get; set; } = "wxapi/oauthurl";

        /// <summary>
        /// 接受微信服务器的消息/事件推送
        /// </summary>
        public string WxServerResponse { get; set; } = "/wxapi/response";
    }
}