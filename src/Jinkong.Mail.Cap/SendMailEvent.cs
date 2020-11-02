using Shashlik.Cap;

namespace Jinkong.Mail.Cap
{
    public class SendMailEvent : IEvent
    {
        public string Address { get; set; }

        public string Subject { get; set; }

        public string Content { get; set; }
    }
}