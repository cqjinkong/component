using System.Collections.Generic;
// ReSharper disable CheckNamespace

namespace Jinkong.Wx
{
    public class WxAppSettings
    {
        /// <summary>
        /// 微信应用id
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 密钥
        /// </summary>
        public string AppSecret { get; set; }

        /// <summary>
        /// token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// EncodingAESKey
        /// </summary>
        public string EncodingAESKey { get; set; }

        /// <summary>
        /// 应用类型
        /// </summary>
        public WxAppType AppType { get; set; }

        /// <summary>
        /// 是不是默认的应用,比如有多个公众号,或多个小程序时
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// app自定义标签,可用于定义这个app用于哪些clientId
        /// </summary>
        public List<string> Tags { get; set; }
    }
}