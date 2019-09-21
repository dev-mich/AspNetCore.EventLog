using AspNetCore.EventLog.Entities;
using AspNetCore.EventLog.PostgreSQL.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.EventLog.PostgreSQL
{
    public class PostgresDbContext: DbContext
    {
        public PostgresDbContext(DbContextOptions<PostgresDbContext> options) : base(options)
        {
        }

        public DbSet<Published> Published { get; set; }

        public DbSet<Received> Received { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new PublishedEntityConfiguration());
            builder.ApplyConfiguration(new ReceivedEntityConfiguration());

        }

    }
}
