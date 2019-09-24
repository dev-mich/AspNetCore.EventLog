using System;
using AspNetCore.EventLog.Interfaces;
using AspNetCore.EventLog.PostgreSQL.Configuration;
using AspNetCore.EventLog.PostgreSQL.Stores;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.EventLog.PostgreSQL.Infrastructure
{
    public class PostgreSqlExtension : IExtension
    {
        private readonly Action<PostgreSqlOptions> _setupOptions;

        public PostgreSqlExtension(Action<PostgreSqlOptions> setupOptions)
        {
            _setupOptions = setupOptions;
        }


        public void AddServices(IServiceCollection services)
        {
            services.Configure(_setupOptions);

            var options = new PostgreSqlOptions();
            _setupOptions(options);

            services.AddEntityFrameworkNpgsql();

            services.AddTransient<DbContextFactory>();
            services.AddTransient<IDbMigrator, PostgreSQLMigrator>();


            // add stores
            services.AddTransient<IPublishedStore, PublishedStore>();
            services.AddTransient<IReceivedStore, ReceivedStore>();

        }
    }
}
