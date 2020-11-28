using Shashlik.Kernel.Dependency;

// ReSharper disable CheckNamespace

namespace Jinkong.Payment
{
    [Singleton]
    public interface IPaySnBuilder
    {
        string BuildSn();
    }
}