#nullable enable
using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Jinkong.AliyunOss
{
    public static class Extensions
    {
        /// <summary>
        /// 根据存储桶名称获取oss提供类
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="bucketName"></param>
        /// <returns></returns>
        public static IAliyunOssProvider? GetAliyunOssProvider(this IServiceProvider serviceProvider, string bucketName)
        {
            return serviceProvider.GetServices<IAliyunOssProvider>()
                .FirstOrDefault(r => r.BucketName == bucketName);
        }
    }
}