
// ReSharper disable CheckNamespace

namespace Jinkong.Payment
{
    /// <summary>
    /// 应用发起支付的支付数据
    /// </summary>
    public class AppPrepayDto
    {
        /// <summary>
        /// 支付调用数据，客户端需要根据不同的支付通道做不同的处理
        /// </summary>
        public string PayData { get; set; }
    }
}
