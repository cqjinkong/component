﻿using Senparc.NeuChar.Entities;

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
