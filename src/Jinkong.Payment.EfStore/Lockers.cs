using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Payment
{
    public class Lockers
    {
        public const string PrepayOrderCreate = "PREPAY_ORDER_CREATE_{0}";
        public const string PrepayOrderStatusChanged = "PREPAY_ORDER_STATUS_CHANGED_{0}";
        public const string PrepayOrderGetPayData = "PREPAY_ORDER_GET_PAY_DATA_{0}";
        public const string RefundOrderCreate = "REFUND_ORDER_CREATE_{0}";
        public const string RefundOrderStatusChanged = "REFUND_ORDER_STATUS_CHANGED_{0}";
    }
}
