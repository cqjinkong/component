using System;
using System.Collections.Generic;
using System.Linq;
using Shashlik.Utils.Extensions;

namespace Jinkong.Enums
{
    /// <summary>
    /// 枚举服务
    /// </summary>
    internal class DefaultEnumService : IEnumService
    {
        Lazy<List<EnumModel>> enumModels;
        public DefaultEnumService(Lazy<List<EnumModel>> enumModels)
        {
            this.enumModels = enumModels;
        }

        /// <summary>
        /// 获取所有的枚举
        /// </summary>
        /// <returns></returns>
        public List<EnumModel> GetAll()
        {
            return enumModels.Value;
        }

        public EnumItem GetItem(string name, int enumValue)
        {
            return GetItems(name).FirstOrDefault(r => r.Value == enumValue);
        }

        public EnumItem GetItem(string name, string enumCode)
        {
            return GetItems(name).FirstOrDefault(r => r.Code == enumCode);
        }

        /// <summary>
        /// 获取某个枚举的所有枚举项
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public List<EnumItem> GetItems(string name)
        {
            return enumModels.Value.FirstOrDefault(r => r.Name == name)?.Items;
        }

        /// <summary>
        /// 验证枚举值是否正确
        /// </summary>
        /// <param name="name"></param>
        /// <param name="code"></param>
        public bool IsValid(string name, string code)
        {
            if (code.IsNullOrWhiteSpace())
                return false;
            return GetItems(name)?.Any(r => r.Code == code) ?? false;
        }

        /// <summary>
        /// 验证枚举值是否正确
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public bool IsValid(string name, int value)
        {
            return GetItems(name)?.Any(r => r.Value == value) ?? false;
        }
    }
}
