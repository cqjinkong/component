using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Jinkong.RC.Config.Demo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseRCConfiguration((host, config) =>
                {
                    var server = config.GetValue<string>("RCConfig:Server");
                    var appId = config.GetValue<string>("RCConfig:AppId");
                    var appKey = config.GetValue<string>("RCConfig:AppKey");
                    return new RCConfigSource(server, appId, appKey);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseUrls("http://*:6001")
                        .UseStartup<Startup>()
                        ;
                });
    }
}