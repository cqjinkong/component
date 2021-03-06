﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Shashlik.Kernel;
using Shashlik.Utils.Extensions;

namespace Jinkong.GeoCoder
{
    /// <summary>
    /// 地理编码
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// 增加地理编码接口
        /// </summary>
        /// <param name="kernelBuilder"></param>
        /// <param name="configuration">GeocoderOptions配置节点,null则使用默认参数,里面的key和areas也一样优先使用传入的配置,否则使用默认配置</param>
        /// <returns></returns>
        public static IKernelServices AddGeoCoder(this IKernelServices kernelBuilder,
            IConfiguration configuration = null)
        {
            var services = kernelBuilder.Services;
            if (configuration != null)
                services.Configure<GeoCoderOptions>(configuration);
            else
            {
                services.Configure<GeoCoderOptions>(r =>
                {
                    var configurationOption = configuration?.Get<GeoCoderOptions>();

                    using (var stream =
                        typeof(IAreaService).Assembly.GetManifestResourceStream("Jinkong.GeoCoder.data.json"))
                    {
                        var str = stream.ReadToString();
                        var options = JsonConvert.DeserializeObject<GeoCoderOptions>(str);
                        if (configurationOption != null && !configurationOption.Key.IsNullOrWhiteSpace())
                            r.Key = configurationOption.Key;
                        else
                            r.Key = options.Key;

                        if (configurationOption != null && !configurationOption.Areas.IsNullOrEmpty())
                            r.Areas = configurationOption.Areas;
                        else
                            r.Areas = options.Areas;
                    }
                });
            }

            return kernelBuilder;
        }
    }
}