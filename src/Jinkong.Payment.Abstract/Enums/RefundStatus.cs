using System.ComponentModel;

// ReSharper disable CheckNamespace

namespace Jinkong.Payment
{
    /// <summary>
    /// 退款状态
    /// </summary>
    public enum RefundStatus
    {
        /// <summary>
        /// 退款成功
        /// </summary>
        [Description("退款成功")] RefundSuccess = 1,
        /// <summary>
        /// 退款关闭
        /// </summary>
        [Description("退款关闭")] RefundClose = 2,
        /// <summary>
        /// 退款处理中
        /// </summary>
        [Description("退款处理中")] RefundProcessing = 3,
        /// <summary>
        /// 退款异常
        /// </summary>
        [Description("退款异常")] RefundError = 4
    }
}