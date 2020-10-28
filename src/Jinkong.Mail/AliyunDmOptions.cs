using System.Collections.Generic;
using Shashlik.Kernel.Attributes;

namespace Jinkong.Mail
{
    [AutoOptions("Jinkong.Mail.AliyunDm")]
    public class AliyunDmOptions
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable { get; set; }
        /// <summary>
        /// 访问密钥ID
        /// </summary>
        public string AccessId { get; set; }
        /// <summary>
        /// 访问密钥
        /// </summary>
        public string AccessKey { get; set; }

        /// <summary>
        /// 控制台创建的发信地址
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// 发信人昵称
        /// </summary>
        public string FromAlias { get; set; }

        public List<_Limit> Limits { get; set; }
    }


    public class _Limit
    {
        public string Subject { get; set; }

        /// <summary>
        /// 每天可以发多少次,空不限制
        /// </summary>
        public int? DayLimitCount { get; set; }
        /// <summary>
        /// 每小时可以发多少次,空不限制
        /// </summary>
        public int? HourLimitCount { get; set; }
        /// <summary>
        /// 每分钟可以发多少次,空不限制
        /// </summary>
        public int? MinuteLimitCount { get; set; }
    }
}
