﻿// <auto-generated />
using System;
using AspNetCore.EventLog.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace AspNetCore.EventLog.PostgreSQL.Migrations
{
    [DbContext(typeof(PostgresDbContext))]
    [Migration("20190921144308_AddRetryCount")]
    partial class AddRetryCount
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
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

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("json");

                    b.Property<DateTime>("CreationTime");

                    b.Property<string>("EventName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("EventState");

                    b.Property<Guid>("TransactionId")
                        .HasMaxLength(40);

                    b.Property<uint>("xmin")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid");

                    b.HasKey("Id");

                    b.HasIndex("CreationTime");

                    b.HasIndex("TransactionId");

                    b.ToTable("EventLog_Published");
                });

            modelBuilder.Entity("AspNetCore.EventLog.Entities.Received", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("json");

                    b.Property<string>("EventName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("EventState")
                        .IsConcurrencyToken();

                    b.Property<DateTime>("ReceivedTime");

                    b.Property<int>("RetryCount");

                    b.Property<uint>("xmin")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid");

                    b.HasKey("Id");

                    b.HasIndex("ReceivedTime");

                    b.ToTable("EventLog_Received");
                });
#pragma warning restore 612, 618
        }
    }
}
