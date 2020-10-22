using Senparc.NeuChar.Entities;

namespace Jinkong.Wx.AspNetCore
{
    /// <summary>
    /// 将响应进行转发
    /// </summary>
    public class ResponseRedirect : ResponseMessageBase
    {
        public string Host { get; set; }
    }
}
