using Shashlik.Kernel;
using Shashlik.Kernel.Autowired;

namespace Jinkong.GeoCoder
{
    public class GeoCoderConfigure : IAutowiredConfigureServices
    {
        public void ConfigureServices(IKernelServices kernelService)
        {
            kernelService.AddGeoCoder();
        }
    }
}