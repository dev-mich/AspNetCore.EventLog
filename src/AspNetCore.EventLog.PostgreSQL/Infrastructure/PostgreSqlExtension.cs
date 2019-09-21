using AspNetCore.EventLog.Abstractions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.EventLog.PostgreSQL.Infrastructure
{
    public class PostgreSqlExtension : IExtension
    {
        private readonly string _connectionString;

        public PostgreSqlExtension(string connectionString)
        {
            _connectionString = connectionString;
        }


        public void AddServices(IServiceCollection services)
        {
            services.AddEntityFrameworkNpgsql().AddDbContext<PostgresDbContext>(opts =>
            {
                opts.UseNpgsql(_connectionString);
            });
        }
    }
}
