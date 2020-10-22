using Microsoft.Extensions.Options;

namespace TencentFaceId.Sdk
{
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

        public EmptyFaceId(IOptions<TencentFaceIdOptions> options) : base(options)
        {
        }
    }
}