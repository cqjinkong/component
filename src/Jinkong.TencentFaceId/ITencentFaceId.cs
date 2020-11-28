using TencentCloud.Faceid.V20180301.Models;

namespace Jinkong.TencentFaceId
{
    /// <summary>
    /// 腾讯云cos文件操作接口
    /// </summary>
    public interface ITencentFaceId
    {
        /// <summary>
        /// 获取人身核验数据
        /// </summary>
        /// <returns></returns>
        DetectAuthResponse DetectAuthH5(string redirect, string ruleId = "");

        /// <summary>
        /// 获取人身核验信息
        /// </summary>
        /// <param name="bizToken"></param>
        /// <param name="ruleId"></param>
        /// <returns></returns>
        DetectInfo GetDetectInfo(string bizToken, string ruleId = "");

        /// <summary>
        /// 两要素认证
        /// </summary>
        /// <param name="idCard"></param>
        /// <param name="realName"></param>
        /// <returns></returns>
        public (bool success, string desc) E2(string idCard, string realName);

        /// <summary>
        /// 银行卡四要素校验
        /// </summary>
        /// <param name="idCard"></param>
        /// <param name="realName"></param>
        /// <param name="phone"></param>
        /// <param name="bankCard"></param>
        /// <returns></returns>
        public (bool success, string desc) E4(string idCard, string realName, string phone,
            string bankCard);

        /// <summary>
        /// 银行卡三要素校验
        /// </summary>
        /// <param name="idCard"></param>
        /// <param name="realName"></param>
        /// <param name="bankCard"></param>
        /// <returns></returns>
        public (bool success, string desc) E3(string idCard, string realName, string bankCard);
    }
}