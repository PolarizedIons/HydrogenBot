﻿// <auto-generated />
using System;
using HydrogenBot.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HydrogenBot.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("HydrogenBot.Database.DbModels.SubscriptionInfo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<ulong>("Channel")
                        .HasColumnType("bigint unsigned");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("MentionString")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("ModifiedAt")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("SubscriptionInfo");
                });

            modelBuilder.Entity("HydrogenBot.Database.DbModels.TwitchSubscription", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("ModifiedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("Online")
                        .HasColumnType("tinyint(1)");

                    b.Property<uint>("StreamerId")
                        .HasColumnType("int unsigned");

                    b.Property<Guid>("SubscriptionInfoId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("SubscriptionInfoId");

                    b.ToTable("TwitchSubscription");
                });

            modelBuilder.Entity("HydrogenBot.Database.DbModels.TwitterSubscription", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("ModifiedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid>("SubscriptionInfoId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("SubscriptionInfoId");

                    b.ToTable("TwitterSubscription");
                });

            modelBuilder.Entity("HydrogenBot.Database.DbModels.TwitchSubscription", b =>
                {
                    b.HasOne("HydrogenBot.Database.DbModels.SubscriptionInfo", "SubscriptionInfo")
                        .WithMany()
                        .HasForeignKey("SubscriptionInfoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("HydrogenBot.Database.DbModels.TwitterSubscription", b =>
                {
                    b.HasOne("HydrogenBot.Database.DbModels.SubscriptionInfo", "SubscriptionInfo")
                        .WithMany()
                        .HasForeignKey("SubscriptionInfoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
