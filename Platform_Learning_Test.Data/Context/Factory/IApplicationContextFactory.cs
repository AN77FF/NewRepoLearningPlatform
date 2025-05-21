using System;
using System.Threading;
using System.Threading.Tasks;
using Platform_Learning_Test.Data.Factory;

namespace Platform_Learning_Test.Data.Context.Factory
{
    public interface IApplicationContextFactory
    {
        ApplicationContext CreateContext();

        Task<ApplicationContext> CreateContextAsync(CancellationToken ct = default);
    }
}