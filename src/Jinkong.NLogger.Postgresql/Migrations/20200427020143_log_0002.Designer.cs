﻿// <auto-generated />

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Jinkong.NLogger.Postgresql.Migrations
{
    [DbContext(typeof(LogDbContext))]
    [Migration("20200427020143_log_0002")]
    partial class log_0002
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Guc.NLogger.Loggers.ErrorLogs", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClientId")
                        .HasMaxLength(50);

                    b.Property<string>("ClientIp")
                        .HasMaxLength(64);

                    b.Property<string>("Exception");

                    b.Property<string>("Level")
                        .HasMaxLength(64);

                    b.Property<DateTime>("LogTime");

                    b.Property<string>("Logger")
                        .HasMaxLength(100);

                    b.Property<string>("Message");

                    b.Property<string>("Method")
                        .HasMaxLength(64);

                    b.Property<string>("RequestBody");

                    b.Property<string>("RequestFormData");

                    b.Property<string>("RequestQueryString");

                    b.Property<string>("RequestUrl");

                    b.Property<string>("User")
                        .HasMaxLength(64);

                    b.HasKey("Id");

                    b.ToTable("ErrorLogs");
                });

            modelBuilder.Entity("Guc.NLogger.Loggers.LoginLogs", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClientId")
                        .HasMaxLength(50);

                    b.Property<string>("ClientIp")
                        .HasMaxLength(100);

                    b.Property<string>("Level")
                        .HasMaxLength(64);

                    b.Property<DateTime>("LogTime");

                    b.Property<string>("Logger")
                        .HasMaxLength(100);

                    b.Property<string>("Message");

                    b.Property<string>("User")
                        .HasMaxLength(64);

                    b.HasKey("Id");

                    b.ToTable("LoginLogs");
                });

            modelBuilder.Entity("Guc.NLogger.Loggers.OperationLogs", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClientId")
                        .HasMaxLength(50);

                    b.Property<string>("ClientIp")
                        .HasMaxLength(64);

                    b.Property<string>("Level")
                        .HasMaxLength(64);

                    b.Property<DateTime>("LogTime");

                    b.Property<string>("Logger")
                        .HasMaxLength(100);

                    b.Property<string>("Message");

                    b.Property<string>("Method")
                        .HasMaxLength(64);

                    b.Property<string>("RequestBody");

                    b.Property<string>("RequestFormData");

                    b.Property<string>("RequestQueryString");

                    b.Property<string>("RequestUrl");

                    b.Property<string>("User")
                        .HasMaxLength(64);

                    b.HasKey("Id");

                    b.ToTable("OperationLogs");
                });

            modelBuilder.Entity("Guc.NLogger.Loggers.RequestLogs", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClientId")
                        .HasMaxLength(50);

                    b.Property<string>("ClientIp")
                        .HasMaxLength(64);

                    b.Property<string>("Exception");

                    b.Property<string>("Level")
                        .HasMaxLength(64);

                    b.Property<DateTime>("LogTime");

                    b.Property<string>("Logger")
                        .HasMaxLength(100);

                    b.Property<string>("Message");

                    b.Property<string>("Method")
                        .HasMaxLength(64);

                    b.Property<string>("Reponse");

                    b.Property<string>("RequestBody");

                    b.Property<string>("RequestFormData");

                    b.Property<string>("RequestQueryString");

                    b.Property<string>("RequestUrl");

                    b.Property<string>("User")
                        .HasMaxLength(64);

                    b.HasKey("Id");

                    b.ToTable("RequestLogs");
                });
#pragma warning restore 612, 618
        }
    }
}
