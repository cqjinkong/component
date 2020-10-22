using System.ComponentModel;
// ReSharper disable CheckNamespace

namespace Jinkong.Payment
{
    /// <summary>
    /// 预付订单状态
    /// </summary>
    public enum PrepayOrderStatus
    {
        /// <summary>
        /// 等待付款
        /// </summary>
        [Description("等待付款")] Waiting = 1,
        /// <summary>
        /// 付款成功
        /// </summary>
        [Description("付款成功")] Succeed = 2,
        /// <summary>
        /// 付款失败
        /// </summary>
        [Description("付款失败")] Failed = 3,
        /// <summary>
        /// 取消付款
        /// </summary>
        [Description("取消付款")] Canceled = 4,
    }
}