using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MobileSecondHand.DB.Models;
using MobileSecondHand.DB.Models.Advertisement;
using MobileSecondHand.DB.Models.Advertisement.Keywords;
using MobileSecondHand.DB.Models.Authentication;

namespace MobileSecondHand.DB.Services {
	public class MobileSecondHandContext : IdentityDbContext<ApplicationUser> {
		public MobileSecondHandContext(DbContextOptions<MobileSecondHandContext> options)
		: base(options) {
		}

		protected override void OnModelCreating(ModelBuilder builder) {
			base.OnModelCreating(builder);
			builder.Entity<CategoryKeywordToAdvertisement>().HasKey(x => new { x.CategoryKeywordId, x.AdvertisementItemId });
			builder.Entity<ColorKeywordToAdvertisement>().HasKey(x => new { x.ColorKeywordId, x.AdvertisementItemId });

			builder.Entity<CategoryKeywordToAdvertisement>()
			   .HasOne(pt => pt.CategoryKeyword)
			   .WithMany(p => p.Advertisements)
			   .HasForeignKey(pt => pt.CategoryKeywordId);

			builder.Entity<CategoryKeywordToAdvertisement>()
				.HasOne(pt => pt.AdvertisementItem)
				.WithMany(t => t.CategoryKeywords)
				.HasForeignKey(pt => pt.AdvertisementItemId);

			builder.Entity<ColorKeywordToAdvertisement>()
			   .HasOne(pt => pt.ColorKeyword)
			   .WithMany(p => p.Advertisements)
			   .HasForeignKey(pt => pt.ColorKeywordId);

			builder.Entity<ColorKeywordToAdvertisement>()
				.HasOne(pt => pt.AdvertisementItem)
				.WithMany(t => t.ColorKeywords)
				.HasForeignKey(pt => pt.AdvertisementItemId);
		}

		public DbSet<ApplicationUser> ApplicationUser { get; set; }
		public DbSet<AdvertisementItem> AdvertisementItem { get; set; }
		public DbSet<AdvertisementPhoto> AdvertisementPhoto { get; set; }
		public DbSet<CategoryKeyword> CategoryKeyword { get; set; }
		public DbSet<ColorKeyword> ColorKeyword { get; set; }
		public DbSet<ColorKeywordToAdvertisement> ColorKeywordToAdvertisement { get; set; }
		public DbSet<CategoryKeywordToAdvertisement> CategoryKeywordToAdvertisement { get; set; }


	}
}
