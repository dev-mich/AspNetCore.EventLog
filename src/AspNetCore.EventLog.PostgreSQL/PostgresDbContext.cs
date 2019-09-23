using AspNetCore.EventLog.Entities;
using AspNetCore.EventLog.PostgreSQL.Configuration;
using AspNetCore.EventLog.PostgreSQL.EntityConfigurations;
using AspNetCore.EventLog.PostgreSQL.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Options;
using System.Data.Common;

namespace AspNetCore.EventLog.PostgreSQL
{
    public class PostgresDbContext: DbContext
    {
        private readonly PostgreSqlOptions _options;
        private readonly DbConnection _conn;

        //public PostgresDbContext(DbContextOptions<PostgresDbContext> options, IOptions<PostgreSqlOptions> setupOptions) : base(options)
        //{
        //    _options = setupOptions.Value;
        //}


        public PostgresDbContext(DbConnection conn, PostgreSqlOptions options)
        {
            _conn = conn;
            _options = options;
        }


        public DbSet<Published> Published { get; set; }

        public DbSet<Received> Received { get; set; }

        public string Schema => _options.DefaultSchema;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseNpgsql(_conn)
                .ReplaceService<IMigrationsAssembly, SchemaAwareMigrationAssembly>();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new PublishedEntityConfiguration());
            builder.ApplyConfiguration(new ReceivedEntityConfiguration());


            if (!string.IsNullOrEmpty(Schema))
                builder.HasDefaultSchema(Schema);
        }

    }
}
