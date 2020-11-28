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
using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Dependency;
using Shashlik.Utils.Extensions;

namespace Jinkong.Mail
{
    [ConditionOnProperty(typeof(bool), "Jinkong.Mail.AliyunDm.Enable", true, DefaultValue = true)]
    [ConditionDependsOn(typeof(IDistributedCache))]
    [Order(210)]
    [Singleton]
    public class AliyunMail : IMail
    {
        private AliyunDmOptions Options { get; }
        private IAcsClient Client { get; }
        private IDistributedCache Cache { get; }
        private ILogger<AliyunMail> Logger { get; }
        private IMailLimit Limit { get; }

        public AliyunMail(IOptions<AliyunDmOptions> options, ILogger<AliyunMail> logger, IDistributedCache cache, IMailLimit limit)
        {
            Options = options.Value;
            Logger = logger;
            IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", Options.AccessId, Options.AccessKey);
            Client = new DefaultAcsClient(profile);
            Cache = cache;
            Limit = limit;
        }

        private const string CachePrefix = "MAIL_LIMIT:";
        private const int OneDaySeconds = 60 * 60 * 24;

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
                throw;
            }
        }

        public void LimitSend(string address, string subject, string content)
        {
            if (!LimitCheck(address, subject))
            {
                Logger.LogError($"次数限制，发送失败 address:{address}, subject:{subject}, content:{content}");
                return;
            }

            try
            {
                Send(address, subject, content);
                Limit.SendDone(address, subject);
            }
            catch (ClientException e)
            {
                Logger.LogError(e, "阿里云邮件发送失败");
                throw;
            }
        }

        public bool LimitCheck(string address, string subject)
        {
            return Limit.CanSend(address, subject);
        }
    }
}