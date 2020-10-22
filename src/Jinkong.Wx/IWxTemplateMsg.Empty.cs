using Microsoft.Extensions.Logging;
using Shashlik.Utils.Extensions;

namespace Jinkong.Wx
{
    /// <summary>
    /// 空的微信模板消息
    /// </summary>
    public class EmptyWxTemplateMsg : IWxTemplateMsg
    {
        public EmptyWxTemplateMsg(ILogger<EmptyWxTemplateMsg> logger)
        {
            Logger = logger;
        }

        private ILogger<EmptyWxTemplateMsg> Logger { get; }

        public void SendWithCode(string openid, string templateCode, object data, string appName = null)
        {
            Logger.LogInformation($"send empty wx template message: {openid}/{templateCode}/{data.ToJson()}/{appName}");
        }
    }
}