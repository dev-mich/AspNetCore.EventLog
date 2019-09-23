using AspNetCore.EventLog.Entities;
using AspNetCore.EventLog.PostgreSQL.Configuration;
using AspNetCore.EventLog.PostgreSQL.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AspNetCore.EventLog.PostgreSQL
{
    public class PostgresDbContext: DbContext
    {
        private readonly string _schema;

        public PostgresDbContext(DbContextOptions<PostgresDbContext> options, IOptions<PostgreSqlOptions> setupOptions) : base(options)
        {
            _schema = setupOptions.Value.DefaultSchema;
        }

        public DbSet<Published> Published { get; set; }

        public DbSet<Received> Received { get; set; }

        public string Schema => _schema;

        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new PublishedEntityConfiguration());
            builder.ApplyConfiguration(new ReceivedEntityConfiguration());


            if (!string.IsNullOrEmpty(_schema))
                builder.HasDefaultSchema(_schema);
        }

    }
}
