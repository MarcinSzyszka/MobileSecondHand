using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using MobileSecondHand.DB.Models;

namespace MobileSecondHand.Migrations
{
    [DbContext(typeof(MobileSecondHandContext))]
    [Migration("20161101100035_AddedWrongAdvertisementIssueModel")]
    partial class AddedWrongAdvertisementIssueModel
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1")
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

                    b.Property<int>("CategoryId");

                    b.Property<DateTime>("CreationDate");

                    b.Property<string>("Description");

                    b.Property<DateTime?>("ExpirationDate");

                    b.Property<bool>("IsBlockedByAdmin");

                    b.Property<bool>("IsOnlyForSell");

                    b.Property<double>("Latitude");

                    b.Property<double>("Longitude");

                    b.Property<int>("Price");

                    b.Property<int>("Size");

                    b.Property<string>("Title");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("UserId");

                    b.ToTable("AdvertisementItem");
                });

            modelBuilder.Entity("MobileSecondHand.DB.Models.Advertisement.AdvertisementPhoto", b =>
                {
                    b.Property<int>("AdvertisementPhotoId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AdvertisementItemId");

                    b.Property<bool>("IsMainPhoto");

                    b.Property<string>("PhotoName");

                    b.HasKey("AdvertisementPhotoId");

                    b.HasIndex("AdvertisementItemId");

                    b.ToTable("AdvertisementPhoto");
                });

            modelBuilder.Entity("MobileSecondHand.DB.Models.Advertisement.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Category");
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

            modelBuilder.Entity("MobileSecondHand.DB.Models.Advertisement.UserToFavouriteAdvertisement", b =>
                {
                    b.Property<string>("ApplicationUserId");

                    b.Property<int>("AdvertisementItemId");

                    b.HasKey("ApplicationUserId", "AdvertisementItemId");

                    b.HasIndex("AdvertisementItemId");

                    b.HasIndex("ApplicationUserId");

                    b.ToTable("UserToFavouriteAdvertisement");
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

                    b.Property<bool>("UserNameIsSetByHimself");

                    b.Property<string>("UserProfilePhotoName");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("MobileSecondHand.DB.Models.Chat.ChatMessage", b =>
                {
                    b.Property<int>("ChatMessageId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AuthorId");

                    b.Property<string>("Content");

                    b.Property<int>("ConversationId");

                    b.Property<DateTime>("Date");

                    b.Property<bool>("Received");

                    b.HasKey("ChatMessageId");

                    b.HasIndex("AuthorId");

                    b.HasIndex("ConversationId");

                    b.ToTable("ChatMessage");
                });

            modelBuilder.Entity("MobileSecondHand.DB.Models.Chat.Conversation", b =>
                {
                    b.Property<int>("ConversationId")
                        .ValueGeneratedOnAdd();

                    b.HasKey("ConversationId");

                    b.ToTable("Conversation");
                });

            modelBuilder.Entity("MobileSecondHand.DB.Models.Chat.UserToConversation", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<int>("ConversationId");

                    b.HasKey("UserId", "ConversationId");

                    b.HasIndex("ConversationId");

                    b.HasIndex("UserId");

                    b.ToTable("UserToConversation");
                });

            modelBuilder.Entity("MobileSecondHand.DB.Models.Feedback.WrongAdvertisementIssue", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AdvertisementId");

                    b.Property<string>("ConsideredByUserId");

                    b.Property<bool>("IsConsidered");

                    b.Property<string>("IssueAuthorId");

                    b.Property<string>("Reason");

                    b.HasKey("Id");

                    b.HasIndex("AdvertisementId");

                    b.HasIndex("ConsideredByUserId");

                    b.HasIndex("IssueAuthorId");

                    b.ToTable("WrongAdvertisementIssue");
                });

            modelBuilder.Entity("MobileSecondHand.DB.Models.Keywords.CategoryKeyword", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("CategoryKeyword");
                });

            modelBuilder.Entity("MobileSecondHand.DB.Models.Keywords.ColorKeyword", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("ColorKeyword");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("MobileSecondHand.DB.Models.Authentication.ApplicationUser")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("MobileSecondHand.DB.Models.Authentication.ApplicationUser")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MobileSecondHand.DB.Models.Authentication.ApplicationUser")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MobileSecondHand.DB.Models.Advertisement.AdvertisementItem", b =>
                {
                    b.HasOne("MobileSecondHand.DB.Models.Advertisement.Category", "Category")
                        .WithMany("Advertisements")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MobileSecondHand.DB.Models.Authentication.ApplicationUser", "User")
                        .WithMany("AdvertisementItems")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("MobileSecondHand.DB.Models.Advertisement.AdvertisementPhoto", b =>
                {
                    b.HasOne("MobileSecondHand.DB.Models.Advertisement.AdvertisementItem", "AdvertisementItem")
                        .WithMany("AdvertisementPhotos")
                        .HasForeignKey("AdvertisementItemId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MobileSecondHand.DB.Models.Advertisement.CategoryKeywordToAdvertisement", b =>
                {
                    b.HasOne("MobileSecondHand.DB.Models.Advertisement.AdvertisementItem", "AdvertisementItem")
                        .WithMany("CategoryKeywords")
                        .HasForeignKey("AdvertisementItemId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MobileSecondHand.DB.Models.Keywords.CategoryKeyword", "CategoryKeyword")
                        .WithMany("Advertisements")
                        .HasForeignKey("CategoryKeywordId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MobileSecondHand.DB.Models.Advertisement.ColorKeywordToAdvertisement", b =>
                {
                    b.HasOne("MobileSecondHand.DB.Models.Advertisement.AdvertisementItem", "AdvertisementItem")
                        .WithMany("ColorKeywords")
                        .HasForeignKey("AdvertisementItemId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MobileSecondHand.DB.Models.Keywords.ColorKeyword", "ColorKeyword")
                        .WithMany("Advertisements")
                        .HasForeignKey("ColorKeywordId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MobileSecondHand.DB.Models.Advertisement.UserToFavouriteAdvertisement", b =>
                {
                    b.HasOne("MobileSecondHand.DB.Models.Advertisement.AdvertisementItem", "AdvertisementItem")
                        .WithMany("UsersWhoAddedToFavourites")
                        .HasForeignKey("AdvertisementItemId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MobileSecondHand.DB.Models.Authentication.ApplicationUser", "ApplicationUser")
                        .WithMany("FavouriteAdvertisementItems")
                        .HasForeignKey("ApplicationUserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MobileSecondHand.DB.Models.Chat.ChatMessage", b =>
                {
                    b.HasOne("MobileSecondHand.DB.Models.Authentication.ApplicationUser", "Author")
                        .WithMany("ChatMessages")
                        .HasForeignKey("AuthorId");

                    b.HasOne("MobileSecondHand.DB.Models.Chat.Conversation", "Conversation")
                        .WithMany("Messages")
                        .HasForeignKey("ConversationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MobileSecondHand.DB.Models.Chat.UserToConversation", b =>
                {
                    b.HasOne("MobileSecondHand.DB.Models.Chat.Conversation", "Conversation")
                        .WithMany("Users")
                        .HasForeignKey("ConversationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MobileSecondHand.DB.Models.Authentication.ApplicationUser", "User")
                        .WithMany("Conversations")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MobileSecondHand.DB.Models.Feedback.WrongAdvertisementIssue", b =>
                {
                    b.HasOne("MobileSecondHand.DB.Models.Advertisement.AdvertisementItem", "Advertisement")
                        .WithMany()
                        .HasForeignKey("AdvertisementId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MobileSecondHand.DB.Models.Authentication.ApplicationUser", "ConsideredByUser")
                        .WithMany()
                        .HasForeignKey("ConsideredByUserId");

                    b.HasOne("MobileSecondHand.DB.Models.Authentication.ApplicationUser", "IssueAuthor")
                        .WithMany()
                        .HasForeignKey("IssueAuthorId");
                });
        }
    }
}
