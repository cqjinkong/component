// ReSharper disable CheckNamespace
namespace Jinkong.Payment
{
    /// <summary>
    /// 退款订单
    /// </summary>
    public class RefundOrders
    {
        public int Id { get; set; }

        public int PrepayOrderId { get; set; }

        public PrepayOrders PrepayOrder { get; set; }

        /// <summary>
        /// 本地的退款单号
        /// </summary>
        public string LocalRefundNo { get; set; }

        /// <summary>
        /// 交易平台的退款单号
        /// </summary>
        public string RefundNo { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public long CreateTime { get; set; }

        /// <summary>
        /// 退款成功的时间
        /// </summary>
        public long? SuccessTime { get; set; }

        /// <summary>
        /// 退款失败的原因
        /// </summary>
        public string FailReason { get; set; }

        /// <summary>
        /// 退款金额
        /// </summary>
        public long RefundAmount { get; set; }

        /// <summary>
        /// 实际退款金额(去除代金券之类的)
        /// </summary>
        public long RealRefundAmount { get; set; }

        /// <summary>
        /// 退款状态
        /// </summary>
        public RefundStatus RefundStatus { get; set; }

        /// <summary>
        /// 交易平台的退款原始数据
        /// </summary>
        public string OriginalData { get; set; }
    }
}
