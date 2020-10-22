using System.ComponentModel.DataAnnotations;

namespace Jinkong.SecurityProxyApi
{
    public class ResponseObj
    {
        /// <summary>
        /// 请求体(密文)
        /// </summary>
        [Required]
        public string Body { get; set; }

        /// <summary>
        /// 请求体签名(对明文的签名)
        /// </summary>
        [Required]
        public string Signature { get; set; }
    }
}
