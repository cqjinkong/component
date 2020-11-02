using System.Threading.Tasks;
using Shashlik.Cap;

namespace Jinkong.Mail.Cap
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
