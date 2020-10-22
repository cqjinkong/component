using System;
using System.Collections.Generic;
using System.Text;
using Shashlik.Kernel.Autowired.Attributes;

namespace Jinkong.AliVideo
{
    [AutoOptions("Jinkong.AliVideo")]
    public class AliVideoOptions
    {
        public bool Enable { get; set; }

        /// <summary>
        /// 访问密钥ID
        /// </summary>
        public string AccessId { get; set; }

        /// <summary>
        /// 访问密钥
        /// </summary>
        public string AccessKey { get; set; }

        public string AccountId { get; set; }

        public string VodCallbackPrivateKey { get; set; }

        public string RegionId { get; set; }
    }
}