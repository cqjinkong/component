using System;

// ReSharper disable CheckNamespace
namespace Jinkong.Payment
{
    /// <summary>
    /// 预付订单支付记录
    /// </summary>
    public class PrepayOrderPaymentLogs
    {
        public int Id { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// 预付订单
        /// </summary>
        public PrepayOrders Order { get; set; }

        /// <summary>
        /// 支付平台对应的应用appid
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        public string MchId { get; set; }

        /// <summary>
        /// 交易平台流水号
        /// </summary>
        public string TransactionId { get; set; }

        /// <summary>
        /// 交易平台用户ID
        /// </summary>
        public string TraderId { get; set; }

        /// <summary>
        /// 本地交易单号,就是这次记录的单号,系统唯一
        /// </summary>
        public string LocalTradeNo { get; set; }

        /// <summary>
        /// 支付通道
        /// </summary>
        public PrepayChannel Channel { get; set; }

        /// <summary>
        /// 支付方式名称
        /// </summary>
        public PayType PayType => Channel.Convert2Type();

        /// <summary>
        /// 是否已付款
        /// </summary>
        public bool IsPaid
        {
            get
            {
                return PayTime != null;
            }
        }

        /// <summary>
        /// 需要支付的金额
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// 支付数据
        /// </summary>
        public string PayData { get; set; }

        /// <summary>
        /// 回调数据
        /// </summary>
        public string NotifyData { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public long CreateTime { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public long ExpireTime { get; set; }

        /// <summary>
        /// 支付时间，null为没有支付
        /// </summary>
        public long? PayTime { get; set; }

        /// <summary>
        /// 并发控制
        /// </summary>
        public DateTime ConcurrencyStamp { get; set; }
    }
}
