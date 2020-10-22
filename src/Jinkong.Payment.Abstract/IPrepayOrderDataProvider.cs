using System.Threading.Tasks;

// ReSharper disable CheckNamespace

namespace Jinkong.Payment
{
    public interface IPrepayOrderDataProvider
    {
        string SourceType { get; }

        /// <summary>
        /// 支付提供类(用于下单),mchName是指使用什么商户名称来进行支付,满足多商户支付的场景
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="sourceSn">业务订单sn</param>
        /// <returns></returns>
        Task<(string sourceId, string title, string attach, int amount, string mchName)> Handle(int userId,
            string sourceSn);

        /// <summary>
        /// 能否支付,请求支付数据时验证
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="sourceSn">sn</param>
        /// <returns></returns>
        Task<(bool canPay, string errorReason)> CanPay(int userId, string sourceSn);

        /// <summary>
        /// 支付成功处理
        /// </summary>
        /// <param name="orderPaidData"></param>
        /// <returns></returns>
        Task PaySuccess(OrderPaidData orderPaidData);
    }
}