#nullable enable
using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Jinkong.AliyunOss
{
    public static class Extensions
    {
        public static IAliyunOssProvider? GetAliyunOssProvider(this IServiceProvider serviceProvider, string bucketName)
        {
            return serviceProvider.GetServices<IAliyunOssProvider>()
                .FirstOrDefault(r => r.BucketName == bucketName);
        }
    }
}