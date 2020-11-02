using Shashlik.Cap;
// ReSharper disable CheckNamespace

namespace Jinkong.Payment
{
    /// <summary>
    /// 退款结果通知
    /// </summary>
    public class RefundResultInnerNotifyEvent : IEvent
    {
        public RefundResultInnerNotifyEvent(RefundData notify)
        {
            Notify = notify;
        }

        public RefundData Notify { get; set; }
    }
}
