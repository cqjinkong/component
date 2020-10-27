using Shashlik.Kernel;

// ReSharper disable CheckNamespace

namespace Jinkong.NLogger
{
    public class NLoggerAutowire : IServiceAutowire
    {
        public void Configure(IKernelServices kernelService)
        {
            kernelService.AddNLogWithMysql(kernelService.RootConfiguration.GetSection("Logging:NLog"),
                autoMigration: true);
        }
    }
}