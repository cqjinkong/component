// ReSharper disable CheckNamespace

using Shashlik.Cap;

namespace Jinkong.Payment
{
    /// <summary>
    /// 退款出结果事件
    /// </summary>
    public class RefundOrderResultEvent : IEvent
    {
        /// <summary>
        /// 来源sn
        /// </summary>
        public string SourceSn { get; set; }

        /// <summary>
        /// 来源类型
        /// </summary>
        public string SourceType { get; set; }

        /// <summary>
        /// 退款状态
        /// </summary>
        public RefundStatus Status { get; set; }

        /// <summary>
        /// 实际退款金额
        /// </summary>
        public long RealRefundAmount { get; set; }

        /// <summary>
        /// 推荐成功的时间
        /// </summary>
        public long? SuccessTime { get; set; }

        /// <summary>
        /// 退款失败的原因
        /// </summary>
        public string FailReason { get; set; }
    }
}