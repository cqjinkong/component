using Shashlik.Kernel.Attributes;

namespace Jinkong.TencentFaceId
{
    /// <summary>
    /// 腾讯云对象存储cos配置
    /// </summary>
    [AutoOptions("Jinkong.FaceId")]
    public class TencentFaceIdOptions
    {
        /// <summary>
        /// appid
        /// </summary>
        public string SecretId { get; set; }

        /// <summary>
        /// SecretKey
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// 区域
        /// </summary>
        public string Region { get; set; }

        public string RuleId { get; set; }

        public bool UseEmpty { get; set; }
    }
}