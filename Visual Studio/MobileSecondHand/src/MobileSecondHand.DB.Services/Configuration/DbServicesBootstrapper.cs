using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MobileSecondHand.DB.Models;
using MobileSecondHand.DB.Models.Authentication;
using MobileSecondHand.DB.Services.Advertisement;
using MobileSecondHand.DB.Services.Advertisement.Categories;
using MobileSecondHand.DB.Services.Advertisement.Keywords;
using MobileSecondHand.DB.Services.Chat;

namespace MobileSecondHand.DB.Services.Configuration {
	public class DbServicesBootstrapper
    {
		public static void RegisterServices(IServiceCollection services) {
			services.AddIdentity<ApplicationUser, IdentityRole>(options => {
				options.Password = new PasswordOptions {
					RequireDigit = true,
					RequiredLength = 6,
					RequireNonAlphanumeric = false,
					RequireLowercase = false,
					RequireUppercase = false
				};
			})
			.AddEntityFrameworkStores<MobileSecondHandContext>()
			.AddDefaultTokenProviders();

			services.AddTransient<IAdvertisementItemDbService, AdvertisementItemDbService>();
			services.AddTransient<IKeywordsDbService, KeywordsDbService>();
			services.AddTransient<IMobileSecondHandContextOptions, MobileSecondHandContextOptions>();
			services.AddTransient<IConversationDbService, ConversationDbService>();
			services.AddTransient<ICategoryDbService, CategoryDbService>();
			


		}
	}
}
