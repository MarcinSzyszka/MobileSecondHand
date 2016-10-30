using System;
using System.IO;
using System.Security.Cryptography;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MobileSecondHand.API.Models.Security;
using MobileSecondHand.API.Services.Advertisement;
using MobileSecondHand.API.Services.Keywords;
using MobileSecondHand.API.Services.Authentication;
using MobileSecondHand.API.Services.CacheServices;
using MobileSecondHand.API.Services.Conversation;
using MobileSecondHand.API.Services.OutsideApisManagers;
using MobileSecondHand.API.Services.Properties;
using MobileSecondHand.API.Services.Categories;
using MobileSecondHand.API.Services.Photos;

namespace MobileSecondHand.API.Services.Configuration {
	public class ApiServicesBootstrapper {
		public static void RegisterServices(IServiceCollection services) {
			RegisterTokenAuthorizationOptions(services);
			services.AddTransient<IApplicationUserManager, ApplicationUserManager>();
			services.AddTransient<IApplicationSignInManager, ApplicationSignInManager>();
			services.AddTransient<IFacebookApiManager, FacebookApiManager>();
			services.AddTransient<IPhotosService, PhotosService>();
			services.AddTransient<IAdvertisementItemService, AdvertisementItemService>();
			services.AddTransient<IIdentityService, IdentityService>();
			services.AddTransient<IKeywordsService, KeywordsService>();
			services.AddTransient<IChatHubCacheService, ChatHubCacheService>();
			services.AddTransient<IConversationService, ConversationService>();
			services.AddTransient<ILastUsersChecksCacheService, LastUsersChecksCacheService>();
			services.AddTransient<ICategoryService, CategoryService>();
			


		}

		private static void RegisterTokenAuthorizationOptions(IServiceCollection services) {
			RSACryptoServiceProvider keyService = new RSACryptoServiceProvider(2048);
			keyService.FromXmlString(Resources.RsaProvider);
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
