using System.Collections.Generic;

// ReSharper disable CheckNamespace
namespace Jinkong.Wx
{
    /// <summary>
    /// 格式化微信模板消息配置
    /// </summary>
    public class WxTemplateMsgOptions
    {
        /// <summary>
        /// 所有的模板配置
        /// </summary>
        public List<WxTemplateMsgModel> Templates { get; set; } = new List<WxTemplateMsgModel>();
    }
}