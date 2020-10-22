using System;

// ReSharper disable CheckNamespace

namespace Jinkong.Payment
{
    /// <summary>
    /// 订单付款数据
    /// </summary>
    public class OrderPaidData
    {
        /// <summary>
        /// 订单id
        /// </summary>       
        public int Id { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string Sn { get; set; }

        /// <summary>
        /// 业务单编号
        /// </summary>
        public string SourceSn { get; set; }

        /// <summary>
        /// 业务单ID
        /// </summary>
        public string SourceId { get; set; }

        /// <summary>
        /// 业务单类名称
        /// </summary>
        public string SourceType { get; set; }

        /// <summary>
        /// 订单总额
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// 支付时间
        /// </summary>
        public long PayTime { get; set; }

        /// <summary>
        /// 交易平台的交易单号
        /// </summary>
        public string TransactionId { get; set; }

        /// <summary>
        /// 交易平台的用户ID
        /// </summary>
        public string TraderId { get; set; }

        /// <summary>
        /// 购买人
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 购买人姓名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 购买人手机
        /// </summary>
        public string UserPhone { get; set; }

        /// <summary>
        /// 订单附加数据
        /// </summary>
        public string OrderAttach { get; set; }

        public PrepayChannel Channel { get; set; }

        public PayType PayType => Channel.Convert2Type();
    }
}