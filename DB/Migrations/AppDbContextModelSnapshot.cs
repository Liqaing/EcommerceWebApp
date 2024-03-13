﻿// <auto-generated />
using EcommerceWebAppProject.DB.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EcommerceWebAppProject.DB.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.16")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("EcommerceWebAppProject.Models.Category", b =>
                {
                    b.Property<int>("CatId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CatId"));

                    b.Property<string>("CatName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CatId");

                    b.ToTable("Category");

                    b.HasData(
                        new
                        {
                            CatId = 1,
                            CatName = "Book"
                        },
                        new
                        {
                            CatId = 2,
                            CatName = "Clothes"
                        },
                        new
                        {
                            CatId = 3,
                            CatName = "Food"
                        });
                });

            modelBuilder.Entity("EcommerceWebAppProject.Models.Product", b =>
                {
                    b.Property<int>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductId"));

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OriginCountry")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("ProName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("catId")
                        .HasColumnType("int");

                    b.HasKey("ProductId");

                    b.HasIndex("catId");

                    b.ToTable("Product");

                    b.HasData(
                        new
                        {
                            ProductId = 1,
                            ImageUrl = "",
                            OriginCountry = "KH",
                            Price = 10.5m,
                            ProName = "A",
                            catId = 1
                        },
                        new
                        {
                            ProductId = 2,
                            ImageUrl = "",
                            OriginCountry = "US",
                            Price = 14.5m,
                            ProName = "B",
                            catId = 1
                        },
                        new
                        {
                            ProductId = 3,
                            ImageUrl = "",
                            OriginCountry = "KH",
                            Price = 5.5m,
                            ProName = "C",
                            catId = 2
                        });
                });

            modelBuilder.Entity("EcommerceWebAppProject.Models.Product", b =>
                {
                    b.HasOne("EcommerceWebAppProject.Models.Category", "category")
                        .WithMany()
                        .HasForeignKey("catId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("category");
                });
#pragma warning restore 612, 618
        }
    }
}
