using AspNetCore.EventLog.PostgreSQL.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.EventLog.PostgreSQL
{
    public class PostgresDbContext: DbContext
    {
        public PostgresDbContext(DbContextOptions<PostgresDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new PublishedEntityConfiguration());

        }

    }
}
