using System.Collections.Generic;

namespace Jinkong.Enums
{
    public interface IEnumService
    {
        /// <summary>
        /// 获取所有的枚举
        /// </summary>
        /// <returns></returns>
        List<EnumModel> GetAll();
        /// <summary>
        /// 获取指定枚举
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        List<EnumItem> GetItems(string name);

        /// <summary>
        /// 获取枚举项
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        EnumItem GetItem(string name, int enumValue);

        /// <summary>
        /// 获取枚举项
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="enumCode"></param>
        /// <returns></returns>
        EnumItem GetItem(string name, string enumCode);

        /// <summary>
        /// 验证枚举code
        /// </summary>
        /// <param name="name"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        bool IsValid(string name, string code);
        /// <summary>
        /// 验证枚举值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool IsValid(string name, int value);
    }
}