// ReSharper disable CheckNamespace

namespace Jinkong.Payment
{
    /// <summary>
    /// 支付回调成功事件
    /// </summary>
    public class PaySuccessInnerNotifyEvent : Shashlik.EventBus.IEvent
    {
        public PaySuccessInnerNotifyEvent(PayResult notify)
        {
            Notify = notify;
        }

        public PayType PayType => Notify.Channel.Convert2Type();

        /// <summary>
        /// 回调数据
        /// </summary>
        public PayResult Notify { get; }
    }
}
