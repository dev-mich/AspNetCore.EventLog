using System;
using System.Reflection;
using AspNetCore.EventLog.Abstractions.DependencyInjection;
using AspNetCore.EventLog.Abstractions.Persistence;
using AspNetCore.EventLog.PostgreSQL.Configuration;
using Microsoft.EntityFrameworkCore;
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

            services.AddEntityFrameworkNpgsql().AddDbContext<PostgresDbContext>(opts =>
            {
                opts.UseNpgsql(options.ConnectionString, n => n.MigrationsAssembly(Assembly.GetAssembly(typeof(PostgresDbContext)).FullName));
            });

            services.AddTransient<IDbMigrator, PostgreSQLMigrator>();
        }
    }
}
