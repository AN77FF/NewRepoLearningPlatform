using System;
using System.Threading;
using System.Threading.Tasks;
using Platform_Learning_Test.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Platform_Learning_Test.Data.Context.Factory
{
    public class ApplicationContextFactory : IApplicationContextFactory
    {
        private readonly DbContextOptions<ApplicationContext> _options;
        private readonly ILogger<ApplicationContextFactory> _logger;

        public ApplicationContextFactory(
            DbContextOptions<ApplicationContext> options,
            ILogger<ApplicationContextFactory> logger)
        {
            _options = options;
            _logger = logger;
        }

        public ApplicationContext CreateContext()
        {
            try
            {
                _logger.LogDebug("Creating new ApplicationContext instance (sync)");
                return new ApplicationContext(_options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create ApplicationContext (sync)");
                throw;
            }
        }

        public async Task<ApplicationContext> CreateContextAsync(CancellationToken ct = default)
        {
            try
            {
                _logger.LogDebug("Creating new ApplicationContext instance (async)");
                var context = new ApplicationContext(_options);

                if (!await context.Database.CanConnectAsync(ct))
                {
                    throw new Exception("Database connection failed");
                }

                return context;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create ApplicationContext (async)");
                throw;
            }
        }
    }
}