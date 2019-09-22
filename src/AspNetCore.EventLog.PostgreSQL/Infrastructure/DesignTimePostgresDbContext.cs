using AspNetCore.EventLog.PostgreSQL.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;

namespace AspNetCore.EventLog.PostgreSQL.Infrastructure
{
    class DesignTimePostgresDbContext : IDesignTimeDbContextFactory<PostgresDbContext>
    {
        public PostgresDbContext CreateDbContext(string[] args)
        {

            var optionsBuilder = new DbContextOptionsBuilder<PostgresDbContext>();

            optionsBuilder.UseNpgsql("design_time");

            return new PostgresDbContext(optionsBuilder.Options, new OptionsWrapper<PostgreSqlOptions>(new PostgreSqlOptions()));

        }
    }
}
