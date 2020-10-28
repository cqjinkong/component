using System.Collections.Generic;

namespace Jinkong.Mail
{
    public class MailLimit
    {
        public int Day { get; set; }

        public List<Record> Records { get; set; }

        public class Record
        {
            public int Hour { get; set; }

            public int Minute { get; set; }
        }
    }
}
