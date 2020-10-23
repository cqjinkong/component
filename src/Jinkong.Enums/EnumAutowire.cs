using Shashlik.Kernel;

namespace Jinkong.Enums
{
    public class EnumAutowire : IServiceAutowire
    {
        public void Configure(IKernelServices kernelService)
        {
            kernelService.AddEnumsByConvention();
        }
    }
}