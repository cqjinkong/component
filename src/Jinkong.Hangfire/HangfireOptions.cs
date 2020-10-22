using Shashlik.Kernel.Autowired.Attributes;

namespace Jinkong.Hangfire
{
    [AutoOptions("Jinkong.Hangfire")]
    public class HangfireOptions
    {
        public bool Enable { get; set; } = true;

        public bool EnableDashboard { get; set; }
    }
}