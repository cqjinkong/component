using System.Collections.Generic;


// ReSharper disable CheckNamespace
namespace Jinkong.Payment
{
    public class PrepayOrders
    {
        /// <summary>
        /// 预付单id
        /// </summary>       
        public int Id { get; set; }

        /// <summary>
        /// 支付标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 预付单编号
        /// </summary>
        public string Sn { get; set; }

        /// <summary>
        /// 业务单编号
        /// </summary>
        public string SourceSn { get; set; }

        /// <summary>
        /// 当前支付订单号,每一次请求新的支付数据就要刷新,然后关闭上一个支付订单,避免重复支付
        /// </summary>
        public string CurrentPayLocalTradeNo { get; set; }

        /// <summary>
        /// 业务单ID
        /// </summary>
        public string SourceId { get; set; }

        /// <summary>
        /// 业务单来源类型
        /// </summary>
        public string SourceType { get; set; }

        /// <summary>
        /// 商品标题(支付的什么)
        /// </summary>
        public string GoodsTitle { get; set; }

        /// <summary>
        /// 下单人员id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 下单人员姓名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 下单人员手机号
        /// </summary>
        public string UserPhone { get; set; }

        /// <summary>
        /// 下单人员扩展属性
        /// </summary>
        public string UserExtends { get; set; }

        /// <summary>
        /// 订单总额
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// 折扣金额
        /// </summary>
        public int Discount { get; set; }

        /// <summary>
        /// 需要支付的金额
        /// </summary>
        public int PayAmount => Amount - Discount;

        /// <summary>
        /// 折扣信息
        /// </summary>
        public List<string> DiscountInfo { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public long CreateTime { get; set; }

        /// <summary>
        /// 支付时间
        /// </summary>
        public long? PayTime { get; set; }

        /// <summary>
        /// 商户名称,创建预付单时就需要确定使用哪个商户号进行支付
        /// </summary>
        public string MchName { get; set; }

        /// <summary>
        /// 商户id,这笔预付单要使用哪个商户号进行支付,最终使用的哪个商户id进行支付的
        /// </summary>
        public string MchId { get; set; }

        /// <summary>
        /// 支付成功后用的appid
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 支付通道
        /// </summary>
        public PrepayChannel? PayChannel { get; set; }

        public PayType? PayType => PayChannel?.Convert2Type();

        /// <summary>
        /// 支付平台单号
        /// </summary>
        public string TransactionId { get; set; }

        ///// <summary>
        ///// 本地支付单号
        ///// </summary>
        //public string LocalTradeNo { get; set; }

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

        /// <summary>
        /// 最后请求支付数据的时间
        /// </summary>
        public long? LastPrepayTime { get; set; }

        /// <summary>
        /// 预付单原始数据
        /// </summary>
        public string PrepayOriginData { get; set; }

        /// <summary>
        /// 支付回调原始数据
        /// </summary>
        public string PayNotifyOriginData { get; set; }

        ///// <summary>
        ///// 支付数据记录
        ///// </summary>
        //public List<PrepayOrderPaymentLogs> PaymentLogs { get; set; }

        /// <summary>
        /// 退款记录
        /// </summary>
        public List<RefundOrders> RefundOrders { get; set; }
    }
}
