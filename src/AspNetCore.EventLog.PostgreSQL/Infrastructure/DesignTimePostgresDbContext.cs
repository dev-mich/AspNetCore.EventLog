using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AspNetCore.EventLog.PostgreSQL.Infrastructure
{
    class DesignTimePostgresDbContext : IDesignTimeDbContextFactory<PostgresDbContext>
    {
        public PostgresDbContext CreateDbContext(string[] args)
        {

            var optionsBuilder = new DbContextOptionsBuilder<PostgresDbContext>();

            optionsBuilder.UseNpgsql("design_time");

            return new PostgresDbContext(optionsBuilder.Options);

        }
    }
}
