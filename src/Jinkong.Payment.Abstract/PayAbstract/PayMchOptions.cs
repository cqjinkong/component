using System.Collections.Generic;
using System.Linq;
using Shashlik.Kernel.Attributes;
using Shashlik.Utils.Extensions;

// ReSharper disable CheckNamespace

namespace Jinkong.Payment
{
    /// <summary>
    /// 商户配置数据
    /// </summary>
    [AutoOptions("Jinkong.Payment")]
    public class PayMchOptions
    {
        /// <summary>
        /// 商户数据,Key,自定义的商户名称
        /// </summary>
        public IDictionary<string, List<MchConfig>> Mchs { get; set; }

        /// <summary>
        /// 根据商户id获取
        /// </summary>
        /// <param name="mchId"></param>
        /// <returns></returns>
        public MchConfig Get(string mchId)
        {
            return Mchs.Values.SelectMany(r => r).FirstOrDefault(r => r.MchId == mchId);
        }

        /// <summary>
        /// 根据商户名称获取商户配置
        /// </summary>
        /// <param name="mchName"></param>
        /// <returns></returns>
        public MchConfig GetByName(string mchName, PayType payType)
        {
            return Mchs.GetOrDefault(mchName)?.SingleOrDefault(r => r.PayType == payType);
        }
    }
}