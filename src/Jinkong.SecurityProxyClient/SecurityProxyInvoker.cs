using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using Jinkong.Utils;

namespace Jinkong.SecurityProxyClient
{
    /// <summary>
    /// 安全代理调用器
    /// </summary>
    public static class SecurityProxyInvoker
    {
        static SecurityProxyInvoker()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        /// <summary>
        /// 通过安全代理调用目标uri
        /// </summary>
        /// <param name="serverUrl">安全代理服务端地址</param>
        /// <param name="body">代理请求体内容,不支持文件</param>
        /// <param name="contentType">代理请求 contentType</param>
        /// <param name="targetUrl">代理调用调至</param>
        /// <param name="serverPublicKey">服务端公钥</param>
        /// <param name="clientPublicKey">本地公钥</param>
        /// <param name="clientPrivateKey">本地私钥</param>
        /// <param name="encoding">编码方式</param>
        /// <param name="method"></param>
        /// <param name="queryStrings"></param>
        /// <param name="headers"></param>
        /// <param name="cookies"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static async Task<string> Invoke(
            string serverUrl,
            string body,
            string contentType,
            string targetUrl,
            string serverPublicKey,
            string clientPublicKey,
            string clientPrivateKey,
            string encoding = "UTF-8",
            RestSharp.Method method = RestSharp.Method.POST,
            IEnumerable<KeyValuePair<string, string>> queryStrings = null,
            IDictionary<string, string> headers = null,
            IDictionary<string, string> cookies = null,
            int timeout = 60
        )
        {
            var enc = Encoding.GetEncoding(encoding);
            if (string.IsNullOrWhiteSpace(body))
                body = "empty";
            var encoded = SecretHelper.Encrypt(body, serverPublicKey, enc);
            var signature = SecretHelper.Sign(body, clientPrivateKey, enc);
            var request = new RequestObj
            {
                Body = encoded,
                Signature = signature,
                ContentType = contentType,
                TargetUrl = targetUrl,
                Encode = encoding,
                QueryStrings = queryStrings,
                Cookies = cookies,
                Headers = headers,
                Method = method,
                Timeout = timeout
            };

            var resObj = await HttpHelper.PostJson<ResponseObj>(serverUrl, request, timeout: 120);
            var resStr = SecretHelper.Decrypt(resObj.Body, clientPrivateKey, enc);
            if (!SecretHelper.Verify(resStr, resObj.Signature, serverPublicKey, enc))
                throw new Exception("安全代理调用结果签名验证错误");
            return resStr == "empty" ? "" : resStr;
        }

        public class ResponseObj
        {
            /// <summary>
            /// 请求体(密文)
            /// </summary>
            public string Body { get; set; }

            /// <summary>
            /// 请求体签名(对明文的签名)
            /// </summary>
            public string Signature { get; set; }
        }

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

            [Required] public string ContentType { get; set; }

            public RestSharp.Method Method { get; set; }

            public IDictionary<string, string> Headers { get; set; }
            public IDictionary<string, string> Cookies { get; set; }

            public IEnumerable<KeyValuePair<string, string>> QueryStrings { get; set; }

            public int? Timeout { get; set; }
        }
    }
}