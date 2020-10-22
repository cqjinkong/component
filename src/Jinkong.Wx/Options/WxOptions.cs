using System.Collections.Generic;
using System.Linq;
using Shashlik.Kernel.Autowired.Attributes;
using Shashlik.Utils.Extensions;

// ReSharper disable CheckNamespace

namespace Jinkong.Wx
{
    [AutoOptions("Jinkong.Wx")]
    public class WxOptions
    {
        public bool Enable { get; set; } = true;

        /// <summary>
        /// 是否使用空的模板消息
        /// </summary>
        public bool UseEmptyTemplateMsg { get; set; }

        /// <summary>
        /// key:自定义的应用名称
        /// </summary>
        public IDictionary<string, WxAppSettings> WxAppSettings { get; set; } = new Dictionary<string, WxAppSettings>();

        /// <summary>
        /// 全局的微信设置
        /// </summary>
        public WxGlobalOptions GlobalSettings { get; set; } = new WxGlobalOptions();

        /// <summary>
        /// 所有的模板配置
        /// </summary>
        public List<WxTemplateMsgModel> Templates { get; set; } = new List<WxTemplateMsgModel>();

        /// <summary>
        /// 获取默认的公众号配置
        /// </summary>
        /// <returns></returns>
        public WxAppSettings GetDefaultMp()
        {
            var mp = WxAppSettings.Values.Where(r => r.AppType == WxAppType.MP).ToList();
            if (mp.Count == 1)
                return mp[0];
            if (mp.Count == 0)
                return null;
            return mp.FirstOrDefault(r => r.IsDefault);
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="appName"></param>
        /// <returns></returns>
        public WxAppSettings Get(string appName)
        {
            return WxAppSettings.GetOrDefault(appName);
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="appName"></param>
        /// <returns></returns>
        public WxAppSettings GetByAppId(string appId)
        {
            return WxAppSettings.Values.FirstOrDefault(r => r.AppId == appId);
        }
    }
}