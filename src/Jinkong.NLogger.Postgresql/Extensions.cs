using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog.Web;
using System.IO;
using System.Text;
using Shashlik.EfCore;
using Shashlik.Kernel;
using Shashlik.RazorFormat;
using Shashlik.Utils.Extensions;

// ReSharper disable CheckNamespace

namespace Jinkong.NLogger
{
    public static class NLoggerMysqlExtensions
    {
        /// <summary>
        /// 增加nlogger服务,需要在Program中调用UseNLog,使用postgres数据库
        /// </summary>
        /// <param name="kernelServices"></param>
        /// <param name="nLogXmlConfigContent">nlog 配置文件内容,空则使用内置配置</param>
        /// <param name="loggingConfigs">日志推送配置</param>
        /// <returns></returns>
        public static IKernelServices AddNLogWithMysql(
            this IKernelServices kernelServices,
            IConfigurationSection loggingConfigs,
            string nLogXmlConfigContent = null)
        {
            var services = kernelServices.Services;
            services.Configure<NLogOptions>(loggingConfigs);
            var loggingOptions = loggingConfigs.Get<NLogOptions>();

            #region 日志数据库

            kernelServices.Services.AddDbContext<LogDbContext>(dbOptions =>
            {
                dbOptions.UseNpgsql(loggingOptions.Conn,
                    builder => { builder.MigrationsAssembly(typeof(LogDbContext).Assembly.GetName().FullName); });
            });

            #endregion

            #region nlog 配置文件自动处理

            // 读取默认配置
            if (nLogXmlConfigContent.IsNullOrWhiteSpace())
            {
                using var stream =
                    typeof(LogDbContext).Assembly.GetManifestResourceStream(
                        $"Jinkong.NLogger.Postgresql.nlog.mysql.config");
                using var sm = new StreamReader(stream!);
                nLogXmlConfigContent = sm.ReadToEnd();
            }

            string filename = Path.Combine(Directory.GetCurrentDirectory(), "nlog.config");

            StringBuilder ignores = new StringBuilder();
            if (!loggingOptions.Ignores.IsNullOrEmpty())
            {
                foreach (var item in loggingOptions.Ignores)
                {
                    ignores.AppendLine($"\t\t\t\t<when condition=\"{item}\" action=\"Ignore\" />");
                }
            }

            // 格式化配置文件
            nLogXmlConfigContent = nLogXmlConfigContent.RazorFormat(new
            {
                loggingOptions.Email, loggingOptions.Conn, Ignores = ignores.ToString()
            });
            File.WriteAllText(filename, nLogXmlConfigContent, Encoding.UTF8);

            NLogBuilder.ConfigureNLog(filename);

            #endregion

            return kernelServices;
        }
    }
}