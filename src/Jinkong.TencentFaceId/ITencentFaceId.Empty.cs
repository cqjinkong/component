using Microsoft.Extensions.Options;
using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Dependency;

namespace Jinkong.TencentFaceId
{
    [ConditionOnProperty(typeof(bool), "Jinkong.FaceId.UseEmpty", true, DefaultValue = false)]
    [Singleton(typeof(DefaultTencentFaceId))]
    public class EmptyFaceId : DefaultTencentFaceId
    {
        public override (bool success, string desc) E2(string idCard, string realName)
        {
            return (true, "success");
        }

        public override (bool success, string desc) E3(string idCard, string realName, string bankCard)
        {
            return (true, "success");
        }

        public override (bool success, string desc) E4(string idCard, string realName, string phone, string bankCard)
        {
            return (true, "success");
        }

        public EmptyFaceId(IOptionsMonitor<TencentFaceIdOptions> options) : base(options)
        {
        }
    }
}