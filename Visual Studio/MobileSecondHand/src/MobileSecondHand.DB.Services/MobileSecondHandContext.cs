using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MobileSecondHand.DB.Models;
using MobileSecondHand.DB.Models.Advertisement;
using MobileSecondHand.DB.Models.Advertisement.Keywords;
using MobileSecondHand.DB.Models.Authentication;
using MobileSecondHand.DB.Models.Chat;

namespace MobileSecondHand.DB.Services {
	public class MobileSecondHandContext : IdentityDbContext<ApplicationUser> {
		public MobileSecondHandContext(DbContextOptions<MobileSecondHandContext> options)
		: base(options) {
			this.ChangeTracker.AutoDetectChangesEnabled = false;
			this.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
		}

		protected override void OnModelCreating(ModelBuilder builder) {
			base.OnModelCreating(builder);
			builder.Entity<CategoryKeywordToAdvertisement>().HasKey(x => new { x.CategoryKeywordId, x.AdvertisementItemId });

			builder.Entity<CategoryKeywordToAdvertisement>()
			   .HasOne(pt => pt.CategoryKeyword)
			   .WithMany(p => p.Advertisements)
			   .HasForeignKey(pt => pt.CategoryKeywordId);

			builder.Entity<CategoryKeywordToAdvertisement>()
				.HasOne(pt => pt.AdvertisementItem)
				.WithMany(t => t.CategoryKeywords)
				.HasForeignKey(pt => pt.AdvertisementItemId);


			builder.Entity<ColorKeywordToAdvertisement>().HasKey(x => new { x.ColorKeywordId, x.AdvertisementItemId });

			builder.Entity<ColorKeywordToAdvertisement>()
			   .HasOne(pt => pt.ColorKeyword)
			   .WithMany(p => p.Advertisements)
			   .HasForeignKey(pt => pt.ColorKeywordId);

			builder.Entity<ColorKeywordToAdvertisement>()
				.HasOne(pt => pt.AdvertisementItem)
				.WithMany(t => t.ColorKeywords)
				.HasForeignKey(pt => pt.AdvertisementItemId);

			builder.Entity<UserToConversation>().HasKey(x => new { x.UserId, x.ConversationId });

			builder.Entity<UserToConversation>()
			   .HasOne(pt => pt.User)
			   .WithMany(p => p.Conversations)
			   .HasForeignKey(pt => pt.UserId);

			builder.Entity<UserToConversation>()
				.HasOne(pt => pt.Conversation)
				.WithMany(t => t.Users)
				.HasForeignKey(pt => pt.ConversationId);
		}

		public DbSet<ApplicationUser> ApplicationUser { get; set; }
		public DbSet<AdvertisementItem> AdvertisementItem { get; set; }
		public DbSet<AdvertisementPhoto> AdvertisementPhoto { get; set; }
		public DbSet<CategoryKeyword> CategoryKeyword { get; set; }
		public DbSet<ColorKeyword> ColorKeyword { get; set; }
		public DbSet<ColorKeywordToAdvertisement> ColorKeywordToAdvertisement { get; set; }
		public DbSet<CategoryKeywordToAdvertisement> CategoryKeywordToAdvertisement { get; set; }
		public DbSet<MobileSecondHand.DB.Models.Chat.Conversation> Conversation { get; set; }
		public DbSet<UserToConversation> UserToConversation { get; set; }
		public DbSet<ChatMessage> ChatMessage { get; set; }
	}
}
