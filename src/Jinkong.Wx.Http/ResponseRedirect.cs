using Senparc.NeuChar.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jinkong.Wx
{
    /// <summary>
    /// 将响应进行转发
    /// </summary>
    public class ResponseRedirect : ResponseMessageBase
    {
        public string Host { get; set; }
    }
}
