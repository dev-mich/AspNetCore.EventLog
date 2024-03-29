﻿using AspNetCore.EventLog.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace AspNetCore.EventLog.PostgreSQL.EntityConfigurations
{
    class PublishedEntityConfiguration : IEntityTypeConfiguration<Published>
    {
        public void Configure(EntityTypeBuilder<Published> builder)
        {

            builder.ToTable("EventLog_Published");

            builder.HasKey(l => l.Id);

            builder.HasIndex(l => l.CreationTime);

            builder.Property(l => l.EventName).IsRequired().HasMaxLength(50);

            builder.Property(l => l.Content).HasColumnType("jsonb").IsRequired();

            builder.Property(l => l.CreationTime).IsRequired();

            builder.Property(l => l.EventState).IsRequired();

            builder.Property(x => x.ConcurrencyToken)
                .HasColumnName("xmin")
                .HasColumnType("xid")
                .ValueGeneratedOnAddOrUpdate()
                .IsConcurrencyToken();

        }
    }
}
