using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Configuration;
using Shashlik.Kernel;
using Shashlik.Utils.Extensions;
using Shashlik.Utils.Helpers;

namespace Jinkong.Enums
{
    public static class Extensions
    {
        /// <summary>
        /// 增加枚举服务,自定义导入枚举数据
        /// </summary>
        /// <param name="kernelBuilder"></param>
        /// <param name="getOptionsFunc">可自定义的枚举描述</param>
        public static IKernelServices AddEnums(this IKernelServices kernelBuilder, Func<List<EnumModel>> getOptionsFunc)
        {
            if (getOptionsFunc == null)
                throw new ArgumentNullException(nameof(getOptionsFunc));

            var enums = new Lazy<List<EnumModel>>(() => getOptionsFunc());
            kernelBuilder.Services.AddSingleton<IEnumService>(new DefaultEnumService(enums));
            return kernelBuilder;
        }

        /// <summary>
        /// 增加枚举服务,使用约定特性<see cref="JinkongEnumDefinitionAttribute"/>约定注册枚举数据
        /// </summary>
        /// <param name="kernelBuilder"></param>
        /// <param name="configuration">可自定义的枚举描述节点配置,_Desc为枚举自身的描述(约定)</param>
        /// <param name="dependencyContext">依赖上下文,null使用默认上下文</param>
        /// <returns></returns>
        public static IKernelServices AddEnumsByConvention(this IKernelServices kernelBuilder,
            IConfiguration configuration = null, DependencyContext dependencyContext = null)
        {
            var enums = new Lazy<List<EnumModel>>(() => LoadEnumModels(configuration, dependencyContext));
            kernelBuilder.Services.AddSingleton<IEnumService>(new DefaultEnumService(enums));
            return kernelBuilder;
        }

        static List<EnumModel> LoadEnumModels(IConfiguration configuration, DependencyContext dependencyContext = null)
        {
            var assemblies = ReflectHelper.GetReferredAssemblies<IEnumService>(dependencyContext);
            HashSet<MemberInfo> members = new HashSet<MemberInfo>();
            foreach (var assembly in assemblies)
            foreach (var type in assembly.DefinedTypes.Where(r =>
                r.IsDefinedAttribute<JinkongEnumDefinitionAttribute>(false)))
                if (type.IsEnum)
                    members.Add(type);
                else if (type.IsClass)
                    type.GetMembers().Where(r => r is Type && ((Type)r).IsEnum).ForEachItem(r => members.Add(r));
            return members.Select(r => AsEnumModel(r, configuration)).ToList();
        }

        static EnumModel AsEnumModel(MemberInfo memberInfo, IConfiguration configuration)
        {
            var type = memberInfo as Type;
            var fields = type.GetFields();
            var items = Enum.GetNames(type).Select(code =>
            {
                var field = fields.First(f => f.Name == code);
                var desc = field.GetCustomAttribute<DescriptionAttribute>();

                return new EnumItem
                {
                    Code = code,
                    Text = configuration?.GetValue<string>($"{type.Name}:{code}") ?? desc?.Description ?? code,
                    Value = (int)Enum.Parse(type, code)
                };
            }).ToList();

            var enumDesc = type.GetCustomAttribute<DescriptionAttribute>();
            return new EnumModel
            {
                Name = type.Name,
                Desc = configuration?.GetValue<string>($"{type.Name}:_Desc") ?? enumDesc?.Description ?? type.Name,
                Items = items
            };
        }
    }
}