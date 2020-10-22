using Shashlik.Mapper;

// ReSharper disable CheckNamespace

namespace Jinkong.Payment
{
    public class PrepayOrderDto : IMapFrom<PrepayOrders>
    {
        /// <summary>
        /// 订单id
        /// </summary>       
        public int Id { get; set; }

        /// <summary>
        /// 订单标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string Sn { get; set; }

        /// <summary>
        /// 订单总额
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public long CreateTime { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public long ExpiresAt { get; set; }

        /// <summary>
        /// 支付时间
        /// </summary>
        public long? PayTime { get; set; }

        /// <summary>
        /// 支付通道
        /// </summary>
        public PrepayChannel? PayChannel { get; set; }

        /// <summary>
        /// 支付平台单号
        /// </summary>
        public string TransactionId { get; set; }

        /// <summary>
        /// 本地支付单号
        /// </summary>
        public string LocalTradeNo { get; set; }

        /// <summary>
        /// 订单附加数据
        /// </summary>
        public string Attach { get; set; }

        /// <summary>
        /// 订单备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 三方交易平台用户id(微信openid/支付uid)
        /// </summary>
        public string TraderId { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public PrepayOrderStatus Status { get; set; }
    }
}