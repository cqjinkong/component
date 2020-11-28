#nullable enable
using Shashlik.EventBus;
using Shashlik.Kernel.Dependency;
using Shashlik.Response;
using Shashlik.Utils.Extensions;

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
        void Send(string address, string subject, string content, ITransactionContext? transactionContext);
    }

    [Singleton]
    public class DefaultMailSender : IMailSender
    {
        private IEventPublisher EventPublisher { get; }
        private IMail Mail { get; }

        public DefaultMailSender(IEventPublisher eventPublisher, IMail mail)
        {
            EventPublisher = eventPublisher;
            Mail = mail;
        }

        public void Send(string address, string subject, string content, ITransactionContext? transactionContext)
        {
            if (subject.IsNullOrEmpty())
            {
                throw ResponseException.ArgError("邮件标题不能为空");
            }

            if (!Mail.LimitCheck(address, subject))
            {
                throw ResponseException.LogicalError("操作过于频繁");
            }

            EventPublisher.PublishAsync(new SendMailEvent() {Address = address, Subject = subject, Content = content}, transactionContext).Wait();
        }
    }
}