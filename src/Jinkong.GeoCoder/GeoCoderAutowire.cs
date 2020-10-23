using Shashlik.Kernel;

namespace Jinkong.GeoCoder
{
    public class GeoCoderAutowire : IServiceAutowire
    {
        public void Configure(IKernelServices kernelService)
        {
            kernelService.AddGeoCoder();
        }
    }
}