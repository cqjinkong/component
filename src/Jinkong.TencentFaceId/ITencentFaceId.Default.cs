using Microsoft.Extensions.Options;
using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Dependency;
using Shashlik.Utils.Extensions;
using TencentCloud.Common;
using TencentCloud.Common.Profile;
using TencentCloud.Faceid.V20180301;
using TencentCloud.Faceid.V20180301.Models;

// ReSharper disable StringLiteralTypo
// ReSharper disable UseObjectOrCollectionInitializer

namespace Jinkong.TencentFaceId
{
    /// <summary>
    /// 腾讯云cos文件操作接口
    /// </summary>
    [ConditionOnProperty(typeof(bool), "Jinkong.FaceId.UseEmpty", false, DefaultValue = false)]
    [Singleton]
    public class DefaultTencentFaceId : ITencentFaceId
    {
        public DefaultTencentFaceId(IOptionsMonitor<TencentFaceIdOptions> options)
        {
            this.Options = options;
        }

        private IOptionsMonitor<TencentFaceIdOptions> Options { get; }

        private Credential GetCredential()
        {
            return new Credential {SecretId = Options.CurrentValue.SecretId, SecretKey = Options.CurrentValue.SecretKey};
        }

        public DetectAuthResponse DetectAuthH5(string redirect, string ruleId = "")
        {
            if (string.IsNullOrEmpty(ruleId))
            {
                ruleId = Options.CurrentValue.RuleId;
            }

            ClientProfile clientProfile = new ClientProfile();
            HttpProfile httpProfile = new HttpProfile();
            httpProfile.Endpoint = ("faceid.tencentcloudapi.com");
            clientProfile.HttpProfile = httpProfile;

            FaceidClient client = new FaceidClient(GetCredential(), Options.CurrentValue.Region, clientProfile);
            DetectAuthRequest req = new DetectAuthRequest();
            string strParams = new {RuleId = ruleId, RedirectUrl = redirect}.ToJson();
            req = DetectAuthRequest.FromJsonString<DetectAuthRequest>(strParams);
            return client.DetectAuthSync(req);
        }

        public DetectInfo GetDetectInfo(string bizToken, string ruleId = "")
        {
            if (string.IsNullOrEmpty(ruleId))
            {
                ruleId = Options.CurrentValue.RuleId;
            }

            ClientProfile clientProfile = new ClientProfile();
            HttpProfile httpProfile = new HttpProfile();
            httpProfile.Endpoint = "faceid.tencentcloudapi.com";
            clientProfile.HttpProfile = httpProfile;

            FaceidClient client = new FaceidClient(GetCredential(), Options.CurrentValue.Region, clientProfile);
            GetDetectInfoRequest req = new GetDetectInfoRequest();
            string strParams = new {RuleId = ruleId, BizToken = bizToken}.ToJson();
            req = GetDetectInfoRequest.FromJsonString<GetDetectInfoRequest>(strParams);
            GetDetectInfoResponse resp = client.GetDetectInfoSync(req);
            var res = resp.DetectInfo.DeserializeJson<DetectInfo>();
            res.DetectInfoContent = resp.DetectInfo;
            return res;
        }

        public virtual (bool success, string desc) E2(string idCard, string realName)
        {
            ClientProfile clientProfile = new ClientProfile();
            HttpProfile httpProfile = new HttpProfile();
            httpProfile.Endpoint = "faceid.tencentcloudapi.com";
            clientProfile.HttpProfile = httpProfile;

            FaceidClient client = new FaceidClient(GetCredential(), Options.CurrentValue.Region, clientProfile);
            string strParams = new {IdCard = idCard, Name = realName}.ToJson();
            var req = IdCardVerificationRequest.FromJsonString<IdCardVerificationRequest>(strParams);
            var resp = client.IdCardVerificationSync(req);
            return (resp.Result == "0", resp.Description);
        }

        /// <summary>
        /// 腾讯云四要素认证
        /// </summary>
        /// <param name="idCard"></param>
        /// <param name="realName"></param>
        /// <param name="phone"></param>
        /// <param name="bankCard"></param>
        /// <returns></returns>
        public virtual (bool success, string desc) E4(string idCard, string realName, string phone,
            string bankCard)
        {
            ClientProfile clientProfile = new ClientProfile();
            HttpProfile httpProfile = new HttpProfile();
            httpProfile.Endpoint = "faceid.tencentcloudapi.com";
            clientProfile.HttpProfile = httpProfile;

            FaceidClient client = new FaceidClient(GetCredential(), Options.CurrentValue.Region, clientProfile);
            string strParams = new {IdCard = idCard, Name = realName, Phone = phone, BankCard = bankCard}.ToJson();
            var req = BankCard4EVerificationRequest.FromJsonString<BankCard4EVerificationRequest>(strParams);
            var resp = client.BankCard4EVerificationSync(req);
            return (resp.Result == "0", resp.Description);
        }

        public virtual (bool success, string desc) E3(string idCard, string realName, string bankCard)
        {
            ClientProfile clientProfile = new ClientProfile();
            HttpProfile httpProfile = new HttpProfile();
            httpProfile.Endpoint = "faceid.tencentcloudapi.com";
            clientProfile.HttpProfile = httpProfile;

            FaceidClient client = new FaceidClient(GetCredential(), Options.CurrentValue.Region, clientProfile);
            string strParams = new {IdCard = idCard, Name = realName, BankCard = bankCard}.ToJson();
            var req = BankCardVerificationRequest.FromJsonString<BankCardVerificationRequest>(strParams);
            var resp = client.BankCardVerificationSync(req);
            return (resp.Result == "0", resp.Description);
        }
    }
}