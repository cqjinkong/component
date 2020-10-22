using System;
using System.Collections.Generic;
using System.Linq;

namespace Jinkong.SecurityProxyApi
{
    public class Settings
    {
        /// <summary>
        /// server安全代理公钥
        /// </summary>
        public string LocalPublicKey { get; set; }

        /// <summary>
        /// server安全代理私钥
        /// </summary>
        public string LocalPrivateKey { get; set; }

        /// <summary>
        /// client公钥
        /// </summary>
        public string RemotePublicKey { get; set; }

        /// <summary>
        /// ip白名单
        /// </summary>
        public string IpWhite { get; set; }

        public Lazy<IEnumerable<string>> IpWhiteList => new Lazy<IEnumerable<string>>(() => IpWhite?.Split(",", StringSplitOptions.RemoveEmptyEntries)?.Select(r => r.Trim()).ToList());

        /// <summary>
        /// 允许的调用目标,英文逗号分割
        /// </summary>
        public string AllowTargets { get; set; }

        public Lazy<IEnumerable<string>> AllowTargetList => new Lazy<IEnumerable<string>>(() => AllowTargets?.Split(",", StringSplitOptions.RemoveEmptyEntries)?.Select(r => r.Trim()).ToList());
    }
}
