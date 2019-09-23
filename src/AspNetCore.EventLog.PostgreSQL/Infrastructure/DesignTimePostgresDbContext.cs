using AspNetCore.EventLog.PostgreSQL.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;
using Npgsql;

namespace AspNetCore.EventLog.PostgreSQL.Infrastructure
{
    class DesignTimePostgresDbContext : IDesignTimeDbContextFactory<PostgresDbContext>
    {
        public PostgresDbContext CreateDbContext(string[] args)
        {

            return new PostgresDbContext(new NpgsqlConnection(), new PostgreSqlOptions());

        }
    }
}
