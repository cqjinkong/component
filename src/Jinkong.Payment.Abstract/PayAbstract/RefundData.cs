// ReSharper disable CheckNamespace
namespace Jinkong.Payment
{
    public class RefundData
    {
        /// <summary>
        /// 本地退款交易号
        /// </summary>
        public string LocalRefundNo { get; set; }

        /// <summary>
        /// 交易平台退款交易号
        /// </summary>
        public string RefundNo { get; set; }

        /// <summary>
        /// 实际退款金额
        /// </summary>
        public int RealRefundAmount { get; set; }

        /// <summary>
        /// 成功退款时间
        /// </summary>
        public long? SuccessTime { get; set; }

        /// <summary>
        /// 退款状态
        /// </summary>
        public RefundStatus Status { get; set; }

        /// <summary>
        /// 失败原因
        /// </summary>
        public string FailReason { get; set; }

        /// <summary>
        /// 回调数据/查询数据
        /// </summary>
        public string OriginData { get; set; }
    }
}
