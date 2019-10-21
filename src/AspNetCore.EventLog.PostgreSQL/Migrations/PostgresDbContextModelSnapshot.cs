﻿// <auto-generated />
using System;
using AspNetCore.EventLog.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace AspNetCore.EventLog.PostgreSQL.Migrations
{
    [DbContext(typeof(PostgresDbContext))]
    partial class PostgresDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("AspNetCore.EventLog.Entities.Published", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<uint>("ConcurrencyToken")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnName("xmin")
                        .HasColumnType("xid");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<string>("CorrelationId");

                    b.Property<DateTime>("CreationTime");

                    b.Property<string>("EventName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("EventState");

                    b.Property<string>("ReplyTo");

                    b.HasKey("Id");

                    b.HasIndex("CreationTime");

                    b.ToTable("EventLog_Published");
                });

            modelBuilder.Entity("AspNetCore.EventLog.Entities.Received", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<uint>("ConcurrencyToken")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnName("xmin")
                        .HasColumnType("xid");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("json");

                    b.Property<string>("CorrelationId");

                    b.Property<string>("EventName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("EventState");

                    b.Property<int>("FailCount");

                    b.Property<DateTime>("ReceivedTime");

                    b.Property<string>("ReplyTo");

                    b.HasKey("Id");

                    b.HasIndex("ReceivedTime");

                    b.ToTable("EventLog_Received");
                });
#pragma warning restore 612, 618
        }
    }
}
