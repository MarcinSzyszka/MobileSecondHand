using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Extensions.DependencyInjection;
using MobileSecondHand.Db.Models;

namespace MobileSecondHand.Db.Services.Configuration
{
    public class DbServicesBootstrapper
    {
		public static void RegisterServices(IServiceCollection services) {
			services.AddIdentity<ApplicationUser, IdentityRole>()
			.AddEntityFrameworkStores<MobileSecondHandContext>()
			.AddDefaultTokenProviders();
		}
	}
}
