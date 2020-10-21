using Shashlik.Kernel;
using Shashlik.Kernel.Autowired;

namespace Jinkong.Bank
{
    public class BankConfigure : IAutowiredConfigureServices
    {
        public void ConfigureServices(IKernelServices kernelService)
        {
            kernelService.AddBank();
        }
    }
}