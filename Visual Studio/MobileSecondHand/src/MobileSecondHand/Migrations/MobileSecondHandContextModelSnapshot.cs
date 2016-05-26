using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using MobileSecondHand.DB.Services;

namespace mobilesecondhand.Migrations
{
    [DbContext(typeof(MobileSecondHandContext))]
    partial class MobileSecondHandContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rc2-20901")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("MobileSecondHand.DB.Models.Advertisement.AdvertisementItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreationDate");

                    b.Property<string>("Description");

                    b.Property<DateTime?>("ExpirationDate");

                    b.Property<bool>("IsActive");

                    b.Property<bool>("IsOnlyForSell");

                    b.Property<double>("Latitude");

                    b.Property<double>("Longitude");

                    b.Property<int>("Price");

                    b.Property<string>("Title");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AdvertisementItem");
                });

            modelBuilder.Entity("MobileSecondHand.DB.Models.Advertisement.AdvertisementPhoto", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AdvertisementItemId");

                    b.Property<bool>("IsMainPhoto");

                    b.Property<string>("PhotoPath");

                    b.HasKey("Id");

                    b.HasIndex("AdvertisementItemId");

                    b.ToTable("AdvertisementPhoto");
                });

            modelBuilder.Entity("MobileSecondHand.DB.Models.Advertisement.CategoryKeywordToAdvertisement", b =>
                {
                    b.Property<int>("CategoryKeywordId");

                    b.Property<int>("AdvertisementItemId");

                    b.HasKey("CategoryKeywordId", "AdvertisementItemId");

                    b.HasIndex("AdvertisementItemId");

                    b.HasIndex("CategoryKeywordId");

                    b.ToTable("CategoryKeywordToAdvertisement");
                });

            modelBuilder.Entity("MobileSecondHand.DB.Models.Advertisement.ColorKeywordToAdvertisement", b =>
                {
                    b.Property<int>("ColorKeywordId");

                    b.Property<int>("AdvertisementItemId");

                    b.HasKey("ColorKeywordId", "AdvertisementItemId");

                    b.HasIndex("AdvertisementItemId");

                    b.HasIndex("ColorKeywordId");

                    b.ToTable("ColorKeywordToAdvertisement");
                });

            modelBuilder.Entity("MobileSecondHand.DB.Models.Advertisement.Keywords.CategoryKeyword", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("CategoryKeyword");
                });

            modelBuilder.Entity("MobileSecondHand.DB.Models.Advertisement.Keywords.ColorKeyword", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("ColorKeyword");
                });

            modelBuilder.Entity("MobileSecondHand.DB.Models.Authentication.ApplicationUser", b =>
                {
                    b.Property<string>("Id");

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedUserName")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("MobileSecondHand.DB.Models.Authentication.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("MobileSecondHand.DB.Models.Authentication.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MobileSecondHand.DB.Models.Authentication.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MobileSecondHand.DB.Models.Advertisement.AdvertisementItem", b =>
                {
                    b.HasOne("MobileSecondHand.DB.Models.Authentication.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("MobileSecondHand.DB.Models.Advertisement.AdvertisementPhoto", b =>
                {
                    b.HasOne("MobileSecondHand.DB.Models.Advertisement.AdvertisementItem")
                        .WithMany()
                        .HasForeignKey("AdvertisementItemId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MobileSecondHand.DB.Models.Advertisement.CategoryKeywordToAdvertisement", b =>
                {
                    b.HasOne("MobileSecondHand.DB.Models.Advertisement.AdvertisementItem")
                        .WithMany()
                        .HasForeignKey("AdvertisementItemId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MobileSecondHand.DB.Models.Advertisement.Keywords.CategoryKeyword")
                        .WithMany()
                        .HasForeignKey("CategoryKeywordId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MobileSecondHand.DB.Models.Advertisement.ColorKeywordToAdvertisement", b =>
                {
                    b.HasOne("MobileSecondHand.DB.Models.Advertisement.AdvertisementItem")
                        .WithMany()
                        .HasForeignKey("AdvertisementItemId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MobileSecondHand.DB.Models.Advertisement.Keywords.ColorKeyword")
                        .WithMany()
                        .HasForeignKey("ColorKeywordId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
