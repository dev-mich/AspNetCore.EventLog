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


        public DbSet<EventLog> Published { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // set default schema if provided
            if (!string.IsNullOrEmpty(_eventLogOptions.DefaultSchema))
                builder.HasDefaultSchema(_eventLogOptions.DefaultSchema);


            // event log table settings
            var entity = builder.Entity<EventLog>();

            entity.ToTable("EventLog_Published");

            entity.HasKey(l => l.Id);

            entity.HasIndex(l => l.CreationTime);

            entity.HasIndex(l => l.PublisherName);

            entity.Property(l => l.PublisherName).IsRequired().HasMaxLength(150);

            entity.Property(l => l.EventName).IsRequired().HasMaxLength(50);

            entity.Property(l => l.Content).HasColumnType("json").IsRequired();

            entity.Property(l => l.CreationTime).IsRequired();

            entity.Property(l => l.EventState).IsRequired().IsConcurrencyToken();


        }

    }
}
