using AspNetCore.EventLog.Entities;
using AspNetCore.EventLog.PostgreSQL.Configuration;
using AspNetCore.EventLog.PostgreSQL.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AspNetCore.EventLog.PostgreSQL
{
    public class PostgresDbContext: DbContext
    {
        private readonly PostgreSqlOptions _options;

        public PostgresDbContext(DbContextOptions<PostgresDbContext> options, IOptions<PostgreSqlOptions> setupOptions) : base(options)
        {
            _options = setupOptions.Value;
        }

        public DbSet<Published> Published { get; set; }

        public DbSet<Received> Received { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new PublishedEntityConfiguration(_options.DefaultSchema));
            builder.ApplyConfiguration(new ReceivedEntityConfiguration(_options.DefaultSchema));


            if (!string.IsNullOrEmpty(_options.DefaultSchema))
                builder.HasDefaultSchema(_options.DefaultSchema);
        }

    }
}
