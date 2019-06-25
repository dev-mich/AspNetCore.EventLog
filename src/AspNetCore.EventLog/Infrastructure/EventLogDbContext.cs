using AspNetCore.EventLog.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AspNetCore.EventLog.Infrastructure
{
    public class EventLogDbContext: DbContext
    {

        private readonly EventLogStoreOptions _eventLogOptions;



        public EventLogDbContext(DbContextOptions<EventLogDbContext> options, IOptions<EventLogStoreOptions> eventLogOptions) : base(options)
        {
            _eventLogOptions = eventLogOptions.Value;
        }


        public DbSet<EventLog> EventLogs { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // set default schema if provided
            if (!string.IsNullOrEmpty(_eventLogOptions.DefaultSchema))
                builder.HasDefaultSchema(_eventLogOptions.DefaultSchema);


            // event log table settings
            var entity = builder.Entity<EventLog>();

            entity.HasKey(l => l.Id);

            entity.Property(l => l.TransactionId).IsRequired();

            entity.HasIndex(l => l.CreationTime);

            entity.HasIndex(l => l.TransactionId);

            entity.Property(l => l.Content).HasColumnType("json").IsRequired();

            entity.Property(l => l.CreationTime).IsRequired();

            entity.Property(l => l.EventState).IsRequired().IsConcurrencyToken();

            entity.Property(l => l.EventAssemblyName).HasMaxLength(100).IsRequired();

            entity.Property(l => l.EventTypeName).HasMaxLength(350).IsRequired();


        }

    }
}
