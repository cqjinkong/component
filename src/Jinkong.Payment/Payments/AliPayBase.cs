using System;
using Health.Payment;

// ReSharper disable CheckNamespace

namespace Jinkong.Payment
{
    public abstract class AliPayBase : PaymentBase
    {
        protected AliPayBase(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
