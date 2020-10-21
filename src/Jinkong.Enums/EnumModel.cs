using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jinkong.Enums
{
    /// <summary>
    /// 枚举类
    /// </summary>
    public class EnumModel
    {
        /// <summary>
        /// 枚举名称,区分大小写
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 枚举描述
        /// </summary>
        public string Desc { get; set; }
        /// <summary>
        /// 枚举项
        /// </summary>
        public List<EnumItem> Items { get; set; }
    }

    /// <summary>
    /// 枚举项
    /// </summary>
    public class EnumItem
    {
        /// <summary>
        /// 枚举编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 枚举值
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// 枚举文本
        /// </summary>
        public string Text { get; set; }
    }
}
