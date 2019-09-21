using AspNetCore.EventLog.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace AspNetCore.EventLog.PostgreSQL.EntityConfigurations
{
    class ReceivedEntityConfiguration : IEntityTypeConfiguration<Received>
    {
        public void Configure(EntityTypeBuilder<Received> builder)
        {

            builder.ToTable("EventLog_Received");

            builder.HasKey(l => l.Id);

            builder.HasIndex(l => l.ReceivedTime);

            builder.Property(l => l.EventName).IsRequired().HasMaxLength(50);

            builder.Property(l => l.Content).HasColumnType("json").IsRequired();

            builder.Property(l => l.ReceivedTime).IsRequired();

            builder.Property(l => l.EventState).IsRequired().IsConcurrencyToken();

            builder.ForNpgsqlUseXminAsConcurrencyToken();

        }
    }
}
