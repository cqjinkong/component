using Shashlik.Kernel;
using Shashlik.Kernel.Autowired;

namespace Jinkong.Enums
{
    public class EnumConfigure:IAutowiredConfigureServices
    {
        public void ConfigureServices(IKernelServices kernelService)
        {
            kernelService.AddEnumsByConvention();
        }
    }
}