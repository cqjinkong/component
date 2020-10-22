using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shashlik.Utils.Extensions;
using Shashlik.Utils.Helpers;

namespace Jinkong.SecurityProxyApi.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class RootController : ControllerBase
    {
        public RootController(IOptions<Settings> options)
        {
            settings = options.Value;
        }

        Settings settings { get; }

        [HttpPost("/proxy")]
        [AllowAnonymous]
        public async Task<IActionResult> Proxy(RequestObj requestObj, [FromServices] ILogger<RootController> logger)
        {
            if (!settings.IpWhite.IsNullOrWhiteSpace() && settings.IpWhite != "*"
                && !settings.IpWhiteList.Value.Contains(HttpContext.Connection.RemoteIpAddress.ToString()))
                return BadRequest($"invalid ip address.");

            if (!settings.AllowTargets.IsNullOrWhiteSpace() && settings.AllowTargets != "*")
            {
                var uri = new Uri(requestObj.TargetUrl.Trim());

                if (!settings.AllowTargetList.Value.Contains($"{uri.Scheme}://{uri.Authority}", StringComparer.OrdinalIgnoreCase))
                {
                    return BadRequest($"invalid target.");
                }
            }

            try
            {
                var uri = new Uri(requestObj.TargetUrl);

                var decodedStr = SecretHelper.Decrypt(requestObj.Body, settings.LocalPrivateKey, requestObj.Encode);
                if (!SecretHelper.Verify(decodedStr, requestObj.Signature, settings.RemotePublicKey, requestObj.Encode))
                    return BadRequest("signature error.");
                var method = requestObj.Method ?? RestSharp.Method.POST;
                if (decodedStr == "empty")
                    decodedStr = "";
                var res = await HttpHelper.Invoke(method, requestObj.TargetUrl, decodedStr, requestObj.ContentType,
                       requestObj.QueryStrings, requestObj.Headers, requestObj.Cookies,
                       requestObj.Timeout ?? 60, proxy: null, encoding: Encoding.GetEncoding(requestObj.Encode));

                if (!res.IsSuccessful)
                {
                    logger.LogError($"request {requestObj.TargetUrl} response error, method: ${method}, http code:{res.StatusCode}, detail: {res.Content}");
                    return BadRequest(res.Content);
                }

                using (var ms = new MemoryStream(res.RawBytes))
                {
                    string response;
                    if (res.RawBytes.Length == 0)
                        response = "empty";
                    else
                        response = ms.ReadToString(Encoding.GetEncoding(requestObj.Encode));
                    var resBody = SecretHelper.Encrypt(response, settings.RemotePublicKey, requestObj.Encode);
                    var resSignture = SecretHelper.Sign(response, settings.LocalPrivateKey, requestObj.Encode);
                    return new JsonResult(new ResponseObj
                    {
                        Body = resBody,
                        Signature = resSignture
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("/ip")]
        [AllowAnonymous]
        public string ip()
        {
            return HttpContext.Connection.RemoteIpAddress.ToString();
        }
    }
}
