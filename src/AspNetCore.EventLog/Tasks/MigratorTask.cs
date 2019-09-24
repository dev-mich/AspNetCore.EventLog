using System;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.EventLog.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AspNetCore.EventLog.Tasks
{
    public class MigratorTask : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public MigratorTask(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            if (stoppingToken.IsCancellationRequested)
                return;

            using (var scope = _scopeFactory.CreateScope())
            {
                var migrator = scope.ServiceProvider.GetRequiredService<IDbMigrator>();

                await migrator.MigrateAsync();
            }


        }
    }
}
