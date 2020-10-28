using System;
using System.Collections.Generic;
using System.Linq;
using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Profile;
using Jinkong.Mail.Aliyun;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shashlik.Kernel.Dependency;
using Shashlik.Utils.Extensions;

namespace Jinkong.Mail
{
    public class AliyunMail : IMail, ITransient
    {
        private AliyunDmOptions Options { get; }
        private IAcsClient Client { get; }
        private IDistributedCache Cache { get; }
        private ILogger<AliyunMail> Logger { get; }

        public AliyunMail(IOptions<AliyunDmOptions> options, ILogger<AliyunMail> logger, IDistributedCache cache)
        {
            Options = options.Value;
            Logger = logger;
            IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", Options.AccessId, Options.AccessKey);
            Client = new DefaultAcsClient(profile);
            Cache = cache;
        }

        const string CachePrefix = "MAIL_LIMIT:";
        const int OneDaySeconds = 60 * 60 * 24;

        public void Send(string address, string subject, string content)
        {
            var request = new SingleSendMailRequest
            {
                AccountName = Options.AccountName,
                FromAlias = Options.FromAlias,
                AddressType = 1,
                ReplyToAddress = true,
                ToAddress = address,
                Subject = subject,
                HtmlBody = content,
            };
            try
            {
                var response = Client.GetAcsResponse(request);
            }
            catch (ClientException e)
            {
                Logger.LogError(e, "阿里云邮件发送失败");
                throw e;
            }
        }

        public void LimitSend(string address, string subject, string content)
        {
            if (!LimitCheck(address, subject))
            {
                Logger.LogError($"次数限制，发送失败 address:{address},subject:{subject},content:{content}");
                return;
            }

            try
            {
                Send(address, subject, content);
                UpdateLimit(address, subject);
            }
            catch (ClientException e)
            {
                Logger.LogError(e, "阿里云邮件发送失败");
                throw;
            }
        }

        public bool LimitCheck(string address, string subject)
        {
            var limit = Options.Limits?.FirstOrDefault(r => r.Subject == subject);
            string key = CachePrefix + address + "_" + subject;
            var now = DateTime.Now.GetLongDate();
            var day = DateTime.Now.Day;
            var hour = DateTime.Now.Hour;
            var minute = DateTime.Now.Minute;
            var second = DateTime.Now.Second;

            if (limit != null && (limit.DayLimitCount.HasValue || limit.HourLimitCount.HasValue || limit.MinuteLimitCount.HasValue))
            {
                var smsLimit = Cache.GetObjectAsync<MailLimit>(key).GetAwaiter().GetResult();
                if (smsLimit == null)
                    return true;

                if (smsLimit.Day != day)
                    return true;

                if (limit.DayLimitCount.HasValue && smsLimit.Records.Count >= limit.DayLimitCount)
                {
                    return false;
                }

                if (limit.HourLimitCount.HasValue && smsLimit.Records.Count(r => r.Hour == hour) >= limit.HourLimitCount)
                {
                    return false;
                }

                if (limit.MinuteLimitCount.HasValue && smsLimit.Records.Count(r => r.Hour == hour && r.Minute == minute) >= limit.MinuteLimitCount)
                {
                    return false;
                }
            }

            return true;
        }

        private void UpdateLimit(string address, string subject)
        {
            var limit = Options.Limits?.FirstOrDefault(r => r.Subject == subject);
            if (limit == null)
                return;
            string key = CachePrefix + address + "_" + subject;
            var day = DateTime.Now.Day;
            var hour = DateTime.Now.Hour;
            var minute = DateTime.Now.Minute;
            var second = DateTime.Now.Second;

            var mailLimit = Cache.GetObjectAsync<MailLimit>(key).GetAwaiter().GetResult();
            if (mailLimit == null)
                mailLimit = new MailLimit
                {
                    Day = day,
                    Records = new List<MailLimit.Record>()
                };
            mailLimit.Records.Add(new MailLimit.Record
            {
                Hour = hour,
                Minute = minute
            });

            Cache.SetObjectAsync(key, mailLimit, DateTimeOffset.Now.Date.AddDays(1)).Wait();
        }
    }
}
