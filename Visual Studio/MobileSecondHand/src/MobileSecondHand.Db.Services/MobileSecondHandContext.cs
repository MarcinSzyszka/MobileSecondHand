using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using MobileSecondHand.Db.Models;
using MobileSecondHand.Db.Models.Advertisement;
using MobileSecondHand.Db.Models.Advertisement.Keywords;

namespace MobileSecondHand.Db.Services
{
    public class MobileSecondHandContext : IdentityDbContext<ApplicationUser>
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
			builder.Entity<CategoryKeywordToAdvertisement>().HasKey(x => new { x.CategoryKeywordId, x.AdvertisementItemId });
			builder.Entity<ColorKeywordToAdvertisement>().HasKey(x => new { x.ColorKeywordId, x.AdvertisementItemId });
			// Customize the ASP.NET Identity model and override the defaults if needed.
			// For example, you can rename the ASP.NET Identity table names and more.
			// Add your customizations after calling base.OnModelCreating(builder);
		}

        public DbSet<ApplicationUser> ApplicationUser { get; set;}
		public DbSet<AdvertisementItem> AdvertisementItem { get; set; }
		public DbSet<AdvertisementPhoto> AdvertisementPhoto { get; set; }
		public DbSet<CategoryKeyword> CategoryKeyword { get; set; }
		public DbSet<ColorKeyword> ColorKeyword { get; set; }
		public DbSet<ColorKeywordToAdvertisement> ColorKeywordToAdvertisement { get; set; }
		public DbSet<CategoryKeywordToAdvertisement> CategoryKeywordToAdvertisement { get; set; }


	}
}
