using Shashlik.Kernel;

namespace Jinkong.Bank
{
    public class BankAutowire : IServiceAutowire
    {
        public void Configure(IKernelServices kernelService)
        {
            kernelService.AddBank();
        }
    }
}