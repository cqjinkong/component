using Microsoft.Extensions.Options;
using Senparc.Weixin.MP.Containers;

namespace Jinkong.Wx
{
    class DefaultWxAccessToken : IWxAccessToken
    {
        public DefaultWxAccessToken(IOptionsMonitor<WxOptions> options)
        {
            Options = options;
        }

        private IOptionsMonitor<WxOptions> Options { get; }

        public string DefaultMpToken => AccessTokenContainer.GetAccessToken(Options.CurrentValue.GetDefaultMp().AppId);

        public string MpToken(string appName) =>
            AccessTokenContainer.GetAccessToken(Options.CurrentValue.Get(appName).AppId);
    }
}