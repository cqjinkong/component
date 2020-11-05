﻿using Jinkong.Mail.EventBus;
using Shashlik.EventBus;
using Shashlik.Kernel.Dependency;
using Shashlik.Response;

namespace Jinkong.Mail.EventBus
{
    /// <summary>
    /// 邮件发送
    /// </summary>
    public interface IMailSender
    {
        /// <summary>
        /// 发送普通HTML邮件
        /// </summary>
        /// <param name="address"></param>
        /// <param name="subject"></param>
        /// <param name="content"></param>
        /// <param name="transactionContext"></param>
        void Send(string address, string subject, string content, TransactionContext transactionContext);
    }

    public class DefaultMailSender : IMailSender, ISingleton
    {
        private IEventPublisher EventPublisher { get; }
        private IMail Mail { get; }

        public DefaultMailSender(IEventPublisher eventPublisher, IMail mail)
        {
            EventPublisher = eventPublisher;
            Mail = mail;
        }

        public void Send(string address, string subject, string content, TransactionContext transactionContext)
        {
            if (!Mail.LimitCheck(address, subject))
            {
                throw ResponseException.LogicalError("操作过于频繁");
            }

            EventPublisher.PublishAsync(new SendMailEvent()
            {
                Address = address,
                Subject = subject,
                Content = content
            }, transactionContext).Wait();
        }
    }
}