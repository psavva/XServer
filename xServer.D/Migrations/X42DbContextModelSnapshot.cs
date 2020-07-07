﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using x42.Feature.Database.Context;

namespace x42.Migrations
{
    [DbContext(typeof(X42DbContext))]
    partial class X42DbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("x42.Feature.Database.Tables.ProfileData", b =>
                {
                    b.Property<string>("KeyAddress")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Signature")
                        .HasColumnType("text");

                    b.Property<string>("TransactionId")
                        .HasColumnType("text");

                    b.HasKey("KeyAddress");

                    b.HasIndex("KeyAddress")
                        .IsUnique();

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("profile");
                });

            modelBuilder.Entity("x42.Feature.Database.Tables.ServerData", b =>
                {
                    b.Property<string>("KeyAddress")
                        .HasColumnType("text");

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("PublicAddress")
                        .HasColumnType("text");

                    b.HasKey("KeyAddress");

                    b.HasIndex("KeyAddress")
                        .IsUnique();

                    b.ToTable("server");
                });

            modelBuilder.Entity("x42.Feature.Database.Tables.ServerNodeData", b =>
                {
                    b.Property<string>("KeyAddress")
                        .HasColumnType("text");

                    b.Property<bool>("Active")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("LastSeen")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("NetworkAddress")
                        .HasColumnType("text");

                    b.Property<long>("NetworkPort")
                        .HasColumnType("bigint");

                    b.Property<int>("NetworkProtocol")
                        .HasColumnType("integer");

                    b.Property<long>("Priority")
                        .HasColumnType("bigint");

                    b.Property<bool>("Relayed")
                        .HasColumnType("boolean");

                    b.Property<string>("Signature")
                        .HasColumnType("text");

                    b.Property<int>("Tier")
                        .HasColumnType("integer");

                    b.HasKey("KeyAddress");

                    b.HasIndex("KeyAddress")
                        .IsUnique();

                    b.ToTable("servernode");
                });
#pragma warning restore 612, 618
        }
    }
}
