using Shashlik.Kernel;
using Shashlik.Kernel.Autowired;

// ReSharper disable CheckNamespace

namespace Jinkong.NLogger
{
    public class NLoggerConfigure : IAutowiredConfigureServices
    {
        public void ConfigureServices(IKernelServices kernelService)
        {
            kernelService.AddNLogWithMysql(kernelService.RootConfiguration.GetSection("Logging:NLog"),
                autoMigration: true);
        }
    }
}