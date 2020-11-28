using System;
using System.Threading;
using System.Threading.Tasks;
using Shashlik.Kernel.Dependency;

// ReSharper disable CheckNamespace

namespace Sbt.Invoice.Service
{
    [Transient]
    public interface ITimer : IDisposable
    {
        TimeSpan Interval { get; }

        Task Execute(CancellationToken cancellationToken);

        void Start() { }

        void Stop() { }
    }
}