using System;
using System.IO;
using System.Security.Cryptography;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MobileSecondHand.API.Models.Security;
using MobileSecondHand.API.Services.Advertisement;
using MobileSecondHand.API.Services.Advertisement.Keywords;
using MobileSecondHand.API.Services.Authentication;
using MobileSecondHand.API.Services.CacheServices;
using MobileSecondHand.API.Services.OutsideApisManagers;

namespace MobileSecondHand.API.Services.Configuration {
	public class ApiServicesBootstrapper {
		public static void RegisterServices(IServiceCollection services) {
			RegisterTokenAuthorizationOptions(services);
			services.AddTransient<IApplicationUserManager, ApplicationUserManager>();
			services.AddTransient<IApplicationSignInManager, ApplicationSignInManager>();
			services.AddTransient<IFacebookApiManager, FacebookApiManager>();
			services.AddTransient<IAdvertisementItemPhotosService, AdvertisementItemPhotosService>();
			services.AddTransient<IAdvertisementItemService, AdvertisementItemService>();
			services.AddTransient<IIdentityService, IdentityService>();
			services.AddTransient<IKeywordsService, KeywordsService>();
			services.AddTransient<IChatHubCacheService, ChatHubCacheService>();
		}

		private static void RegisterTokenAuthorizationOptions(IServiceCollection services) {
			//in production keep RSACryptoServiceProvider in save place
			RSACryptoServiceProvider keyService = new RSACryptoServiceProvider(2048);
			var xmlString = String.Empty;
			var path = @"C:\Users\marcianno\Documents\Key\RsaProvider.txt";
			using (StreamReader sw = new StreamReader(path)) {
				xmlString = sw.ReadToEnd();
			}

			keyService.FromXmlString(xmlString);
			RsaSecurityKey key = new RsaSecurityKey(keyService.ExportParameters(true));

			var tokenOptions = new TokenAuthorizationOptions {
				Audience = "MobileSecondHandApp",
				Issuer = "MarcinSzyszka",
				SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256Signature)
			};
			services.AddSingleton<TokenAuthorizationOptions>(tokenOptions);
		}
	}
}
