﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Jinkong.Mail.EventBus;
using Shashlik.EventBus;

namespace Jinkong.Mail.EventBus
{
    public class SendMailEventForExecuteHandler : IEventHandler<SendMailEvent>
    {
        private IMail Mail { get; }
        public SendMailEventForExecuteHandler(IMail mail)
        {
            Mail = mail;
        }

        public Task Execute(SendMailEvent @event, IDictionary<string, string> items)
        {
            Mail.LimitSend(@event.Address, @event.Subject, @event.Content);
            return Task.CompletedTask;
        }
    }
}
