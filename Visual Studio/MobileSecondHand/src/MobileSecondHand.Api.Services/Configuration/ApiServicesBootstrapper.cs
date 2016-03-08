using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MobileSecondHand.Api.Models.Security;
using MobileSecondHand.Api.Services.Advertisement;
using MobileSecondHand.Api.Services.Authentication;
using MobileSecondHand.Api.Services.OutsideApisManagers;

namespace MobileSecondHand.Api.Services.Configuration {
	public class ApiServicesBootstrapper {
		public static void RegisterServices(IServiceCollection services) {
			RegisterTokenAuthorizationOptions(services);
			services.AddTransient<IApplicationUserManager, ApplicationUserManager>();
			services.AddTransient<IApplicationSignInManager, ApplicationSignInManager>();
			services.AddTransient<IFacebookApiManager, FacebookApiManager>();
			services.AddTransient<IAdvertisementItemPhotosUploader, AdvertisementItemPhotosUploader>();
			
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
			services.AddInstance<TokenAuthorizationOptions>(tokenOptions);
		}
	}
}
