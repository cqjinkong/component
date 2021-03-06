﻿using System.Collections.Generic;
using Shashlik.Kernel.Attributes;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable CollectionNeverUpdated.Global

// ReSharper disable CheckNamespace

namespace Jinkong.NLogger
{
    public class NLogOptions
    {
        /// <summary>
        /// 日志数据库连接字符串
        /// </summary>
        public string Conn { get; set; }

        /// <summary>
        /// 是否自动迁移
        /// </summary>
        public bool AutoMigration { get; set; }

        /// <summary>
        /// 忽略的logger,不记录
        /// </summary>
        public List<string> Ignores { get; set; }

        public EmailOptions Email { get; set; }

        /// <summary>
        /// http日志推送配置
        /// </summary>
        public class EmailOptions
        {
            /// <summary>
            /// 是否启用,默认false
            /// </summary>
            public string Enabled { get; set; }

            /// <summary>
            /// 推送等级
            /// </summary>
            public string Level { get; set; }

            public string From { get; set; }

            public string To { get; set; }

            public string SmtpServer { get; set; }
            public string SmtpPort { get; set; }
            public string SmtpUserName { get; set; }
            public string SmtpPassword { get; set; }
            public string Subject { get; set; }
        }
    }
}