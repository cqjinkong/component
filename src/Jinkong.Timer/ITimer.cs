using System;
using System.Threading;
using System.Threading.Tasks;
// ReSharper disable CheckNamespace

namespace Sbt.Invoice.Service
{
    public interface ITimer : Shashlik.Kernel.Dependency.ITransient, IDisposable
    {
        TimeSpan Interval { get; }

        Task Execute(CancellationToken cancellationToken);

        void Start() { }

        void Stop() { }
    }
}
