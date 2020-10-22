using System;
// ReSharper disable CheckNamespace

namespace Jinkong.Payment
{
    public static class Extensions
    {
        /// <summary>
        /// 转换为支付类型
        /// </summary>
        /// <param name="prepayChannel"></param>
        /// <returns></returns>
        public static PayType Convert2Type(this PrepayChannel prepayChannel)
        {
            switch (prepayChannel)
            {
                case PrepayChannel.AliPayH5:
                    return PayType.AliPay;
                case PrepayChannel.WxNative:
                case PrepayChannel.WxH5:
                case PrepayChannel.WxApp:
                case PrepayChannel.WxJs:
                    return PayType.WxPay;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
