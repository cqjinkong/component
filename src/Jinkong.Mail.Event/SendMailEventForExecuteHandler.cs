using System.Threading.Tasks;
using Shashlik.EventBus;

namespace Jinkong.Mail.Event
{
    public class SendMailEventForExecuteHandler : IEventHandler<SendMailEvent>
    {
        private IMail Mail { get; }
        public SendMailEventForExecuteHandler(IMail mail)
        {
            Mail = mail;
        }

        public Task Execute(SendMailEvent @event)
        {
            Mail.LimitSend(@event.Address, @event.Subject, @event.Content);
            return Task.CompletedTask;
        }
    }
}
