using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Extensions.DependencyInjection;
using MobileSecondHand.Db.Models;
using MobileSecondHand.Db.Services.Advertisement;

namespace MobileSecondHand.Db.Services.Configuration
{
    public class DbServicesBootstrapper
    {
		public static void RegisterServices(IServiceCollection services) {
			services.AddIdentity<ApplicationUser, IdentityRole>(options => {
				options.Password = new Microsoft.AspNet.Identity.PasswordOptions {
					RequireDigit = true,
					RequiredLength = 6,
					RequireNonLetterOrDigit = false,
					RequireLowercase = false,
					RequireUppercase = false
				};
			})
			.AddEntityFrameworkStores<MobileSecondHandContext>()
			.AddDefaultTokenProviders();

			services.AddTransient<IAdvertisementItemDbService, AdvertisementItemDbService>();
		}
	}
}
