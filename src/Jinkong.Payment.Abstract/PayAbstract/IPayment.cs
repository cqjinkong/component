using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Health.Payment.PayAbstract;

// ReSharper disable CheckNamespace

namespace Jinkong.Payment
{
    /// <summary>
    /// 支付通道接口
    /// 每个通道只需要实现二维码,URL,支付数据中的一个方法
    /// </summary>
    public interface IPayment
    {
        /// <summary>
        /// 支付通道名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 支付通道类型
        /// </summary>
        PrepayChannel Channel { get; }

        /// <summary>
        /// 获取支付数据供客户端调用
        /// </summary>
        /// <param name="payEntity"></param>
        /// <returns></returns>
        Task<PrepayData> GetPrepayData(PayParams payEntity);

        /// <summary>
        /// 获取支付数据有效时间(秒)
        /// </summary>
        /// <returns></returns>
        TimeSpan GetPayDataExpireTime();

        /// <summary>
        /// 支付回调
        /// </summary>
        /// <param name="context"></param>
        Task<PayResult> PayResultNotify(IEnumerable<KeyValuePair<string, string>> query, IDictionary<string, string> header, IDictionary<string, string> form, Stream body);

        /// <summary>
        /// 退款回调
        /// </summary>
        /// <param name="query"></param>
        /// <param name="header"></param>
        /// <param name="form"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        Task<(bool success, RefundData refundDetail)> RefundResultNotify(IEnumerable<KeyValuePair<string, string>> query, IDictionary<string, string> header, IDictionary<string, string> form, Stream body);

        /// <summary>
        /// 支付结果查询
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="mchId"></param>
        /// <param name="localTradeNo"></param>
        /// <returns></returns>
        Task<PayResult> PayQuery(string appId, string mchId, string localTradeNo);

        /// <summary>
        /// 退款接口
        /// </summary>
        /// <param name="localTranNo">本地交易单号</param>
        /// <param name="localRefundNo">本地退款单号</param>
        /// <param name="amount">退款金额</param>
        /// <param name="refundReason">退款原因</param>
        /// <returns></returns>
        Task Refund(string appId, string mchId, string localTranNo, string localRefundNo, int orderAmount, int refundAmount, string refundReason);

        /// <summary>
        /// 退款接口
        /// </summary>
        /// <param name="localTranNo">本地交易单号</param>
        /// <param name="localRefundNo">本地退款单号</param>
        /// <returns></returns>
        Task<RefundData> RefundQuery(string appId, string mchId, string localTranNo, string localRefundNo);

        /// <summary>
        /// 获取回调方需要返回的html数据
        /// </summary>
        /// <param name="notify"></param>
        /// <returns></returns>
        Task<string> GetPayResultResponse(PayResult notify);

        /// <summary>
        /// 获取回调方需要返回的html数据
        /// </summary>
        /// <param name="notify"></param>
        /// <returns></returns>
        Task<string> GetRefundResultResponse(bool success, RefundData notify);

        /// <summary>
        /// 关闭订单
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="mchId"></param>
        /// <param name="localTranNo"></param>
        /// <returns></returns>
        Task CloseOrder(string appId, string mchId, string localTranNo);
    }
}
