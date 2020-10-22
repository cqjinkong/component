using Jinkong.Utils.Common;
using Jinkong.Utils.Extensions;
using Microsoft.Extensions.Options;
using TencentCloud.Common;
using TencentCloud.Common.Profile;
using TencentCloud.Faceid.V20180301;
using TencentCloud.Faceid.V20180301.Models;

// ReSharper disable StringLiteralTypo
// ReSharper disable UseObjectOrCollectionInitializer

namespace TencentFaceId.Sdk
{
    //TODO: 腾讯的接口调用频率限制都为20毫秒,如果并发量过大可能会异常,但实际并发测试好像又没问题,需要留意
    
    /// <summary>
    /// 腾讯云cos文件操作接口
    /// </summary>
    public class DefaultTencentFaceId : ITencentFaceId
    {
        public DefaultTencentFaceId(IOptions<TencentFaceIdOptions> options)
        {
            this.options = options.Value;
            cred = new Credential {SecretId = options.Value.SecretId, SecretKey = options.Value.SecretKey};
        }

        private TencentFaceIdOptions options { get; }
        private Credential cred { get; }

        public DetectAuthResponse DetectAuthH5(string redirect, string ruleId = "")
        {
            if (string.IsNullOrEmpty(ruleId))
            {
                ruleId = options.RuleId;
            }
            ClientProfile clientProfile = new ClientProfile();
            HttpProfile httpProfile = new HttpProfile();
            httpProfile.Endpoint = ("faceid.tencentcloudapi.com");
            clientProfile.HttpProfile = httpProfile;

            FaceidClient client = new FaceidClient(cred, options.Region, clientProfile);
            DetectAuthRequest req = new DetectAuthRequest();
            string strParams = JsonHelper.Serialize(new {RuleId = ruleId, RedirectUrl = redirect});
            req = DetectAuthRequest.FromJsonString<DetectAuthRequest>(strParams);
            return client.DetectAuthSync(req);
        }

        public DetectInfo GetDetectInfo(string bizToken, string ruleId = "")
        {
            if (string.IsNullOrEmpty(ruleId))
            {
                ruleId = options.RuleId;
            }
            ClientProfile clientProfile = new ClientProfile();
            HttpProfile httpProfile = new HttpProfile();
            httpProfile.Endpoint = "faceid.tencentcloudapi.com";
            clientProfile.HttpProfile = httpProfile;

            FaceidClient client = new FaceidClient(cred, options.Region, clientProfile);
            GetDetectInfoRequest req = new GetDetectInfoRequest();
            string strParams = JsonHelper.Serialize(new {RuleId = ruleId, BizToken = bizToken});
            req = GetDetectInfoRequest.FromJsonString<GetDetectInfoRequest>(strParams);
            GetDetectInfoResponse resp = client.GetDetectInfoSync(req);
            var res = JsonHelper.Deserialize<DetectInfo>(resp.DetectInfo);
            res.DetectInfoContent = resp.DetectInfo;
            return res;
        }

        public virtual (bool success, string desc) E2(string idCard, string realName)
        {
            ClientProfile clientProfile = new ClientProfile();
            HttpProfile httpProfile = new HttpProfile();
            httpProfile.Endpoint = "faceid.tencentcloudapi.com";
            clientProfile.HttpProfile = httpProfile;

            FaceidClient client = new FaceidClient(cred, "ap-chengdu", clientProfile);
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

            FaceidClient client = new FaceidClient(cred, "ap-chengdu", clientProfile);
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

            FaceidClient client = new FaceidClient(cred, "ap-chengdu", clientProfile);
            string strParams = new {IdCard = idCard, Name = realName, BankCard = bankCard}.ToJson();
            var req = BankCardVerificationRequest.FromJsonString<BankCardVerificationRequest>(strParams);
            var resp = client.BankCardVerificationSync(req);
            return (resp.Result == "0", resp.Description);
        }
    }
}