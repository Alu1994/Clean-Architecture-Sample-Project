﻿// <auto-generated />
using System;
using CleanArchitectureSampleProject.Infrastructure.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CleanArchitectureSampleProject.Repository.Migrations
{
    [DbContext(typeof(ProductDataContext))]
    [Migration("20241201004747_AddsSellEntitiesOnDB")]
    partial class AddsSellEntitiesOnDB
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Entities.Category", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreationDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValue(new DateTime(2024, 12, 1, 0, 47, 46, 730, DateTimeKind.Utc).AddTicks(3071));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Product", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("CategoryId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreationDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValue(new DateTime(2024, 12, 1, 0, 47, 46, 730, DateTimeKind.Utc).AddTicks(4751));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer");

                    b.Property<decimal>("Value")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells.Entities.SellItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreationDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValue(new DateTime(2024, 12, 1, 0, 47, 46, 730, DateTimeKind.Utc).AddTicks(8006));

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uuid");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer");

                    b.Property<Guid>("SellId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.HasIndex("SellId");

                    b.ToTable("SellItems");
                });

            modelBuilder.Entity("CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells.Sell", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreationDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValue(new DateTime(2024, 12, 1, 0, 47, 46, 730, DateTimeKind.Utc).AddTicks(6458));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("TotalValue")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.ToTable("Sells");
                });

            modelBuilder.Entity("CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Product", b =>
                {
                    b.HasOne("CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Entities.Category", "Category")
                        .WithMany("Products")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells.Entities.SellItem", b =>
                {
                    b.HasOne("CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Product", "Product")
                        .WithMany("Items")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells.Sell", "Sell")
                        .WithMany("Items")
                        .HasForeignKey("SellId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");

                    b.Navigation("Sell");
                });

            modelBuilder.Entity("CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Entities.Category", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Product", b =>
                {
                    b.Navigation("Items");
                });

            modelBuilder.Entity("CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells.Sell", b =>
                {
                    b.Navigation("Items");
                });
#pragma warning restore 612, 618
        }
    }
}
