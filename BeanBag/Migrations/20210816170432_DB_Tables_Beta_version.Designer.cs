﻿// <auto-generated />
using System;
using BeanBag.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BeanBag.Migrations
{
    [DbContext(typeof(DBContext))]
    [Migration("20210816170432_DB_Tables_Beta_version")]
    partial class DB_Tables_Beta_version
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.6")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("BeanBag.Models.AIModel", b =>
                {
                    b.Property<Guid>("projectId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("projectName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("projectId");

                    b.ToTable("AIModels");
                });

            modelBuilder.Entity("BeanBag.Models.AIModelVersions", b =>
                {
                    b.Property<Guid>("iterationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("availableToUser")
                        .HasColumnType("bit");

                    b.Property<string>("iterationName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("predictionUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("projectId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("status")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("iterationId");

                    b.HasIndex("projectId");

                    b.ToTable("AIModelIterations");
                });

            modelBuilder.Entity("BeanBag.Models.Inventory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("createdDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("userId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Inventories");
                });

            modelBuilder.Entity("BeanBag.Models.Item", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("QRContents")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("colour")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("condition")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("entryDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("imageURL")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("inventoryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("isSold")
                        .HasColumnType("bit");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("price")
                        .HasColumnType("float");

                    b.Property<int>("quantity")
                        .HasColumnType("int");

                    b.Property<DateTime>("soldDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("inventoryId");

                    b.ToTable("Items");
                });

            modelBuilder.Entity("BeanBag.Models.UserRoles", b =>
                {
                    b.Property<string>("userId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("role")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("userId");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("BeanBag.Models.AIModelVersions", b =>
                {
                    b.HasOne("BeanBag.Models.AIModel", "AIModel")
                        .WithMany()
                        .HasForeignKey("projectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AIModel");
                });

            modelBuilder.Entity("BeanBag.Models.Item", b =>
                {
                    b.HasOne("BeanBag.Models.Inventory", "Inventory")
                        .WithMany()
                        .HasForeignKey("inventoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Inventory");
                });
#pragma warning restore 612, 618
        }
    }
}
