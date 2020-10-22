using Shashlik.Kernel.Dependency;

// ReSharper disable CheckNamespace

namespace Jinkong.Payment
{
    public interface IPaySnBuilder : ISingleton
    {
        string BuildSn();
    }
}