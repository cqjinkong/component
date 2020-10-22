using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Jinkong.SecurityProxyApi
{
    public class RequestObj
    {
        /// <summary>
        /// 目标url
        /// </summary>
        [Required]
        [Url]
        public string TargetUrl { get; set; }

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

        /// <summary>
        /// 编码方式
        /// </summary>
        [Required]
        public string Encode { get; set; }

        [Required]
        public string ContentType { get; set; }

        public RestSharp.Method? Method { get; set; }

        public IDictionary<string, string> Headers { get; set; }
        public IDictionary<string, string> Cookies { get; set; }

        public IEnumerable<KeyValuePair<string, string>> QueryStrings { get; set; }

        public int? Timeout { get; set; }
    }
}
