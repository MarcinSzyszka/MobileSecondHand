using System;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MobileSecondHand.Api.Models.Security;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Authentication.JwtBearer;
using Microsoft.AspNet.Cors.Infrastructure;
using MobileSecondHand.Db.Services.Configuration;
using MobileSecondHand.Db.Services;
using MobileSecondHand.Api.Services.Configuration;
using MobileSecondHand.Common.Configuration;

namespace MobileSecondHand {
	public class Startup {
		public Startup(IHostingEnvironment env) {
			// Set up configuration sources.

			var builder = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json")
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

			if (env.IsDevelopment()) {
				// For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
				builder.AddUserSecrets();

				// This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
				builder.AddApplicationInsightsSettings(developerMode: true);
			}

			builder.AddEnvironmentVariables();
			Configuration = builder.Build();
		}

		public IConfigurationRoot Configuration { get; set; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services) {
			// Add framework services.
			services.AddApplicationInsightsTelemetry(Configuration);

			services.AddEntityFramework()
				.AddSqlServer()
				.AddDbContext<MobileSecondHandContext>(options => {
					options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"]);
				});


			services.AddAuthorization(auth => {
				auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
					.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme‌​)
					.RequireAuthenticatedUser().Build());
			});

			DbServicesBootstrapper.RegisterServices(services);
			ApiServicesBootstrapper.RegisterServices(services);
			CommonServicesBootstrapper.RegisterServices(services);
			services.AddMvc();

			var policy = new CorsPolicy();
			policy.Headers.Add("*");
			policy.Methods.Add("*");
			policy.Origins.Add("*");

			services.AddCors(config => {
				config.AddPolicy("myPolicy", policy);
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, MobileSecondHandContext context, TokenAuthorizationOptions tokenAuthorizationOptions) {
			loggerFactory.AddConsole(Configuration.GetSection("Logging"));
			loggerFactory.AddDebug();

			app.UseApplicationInsightsRequestTelemetry();

			if (env.IsDevelopment()) {
				app.UseBrowserLink();
				app.UseDeveloperExceptionPage();
				app.UseDatabaseErrorPage();
			}
			else {
				app.UseExceptionHandler("/Home/Error");

				// For more details on creating database during deployment see http://go.microsoft.com/fwlink/?LinkID=615859
				try {
					using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
						.CreateScope()) {
						serviceScope.ServiceProvider.GetService<MobileSecondHandContext>()
							 .Database.Migrate();
					}
				} catch { }
			}

			app.UseIISPlatformHandler(options => options.AuthenticationDescriptions.Clear());

			app.UseApplicationInsightsExceptionTelemetry();

			app.UseStaticFiles();

			app.UseJwtBearerAuthentication(options => {
				// Basic settings - signing key to validate with, audience and issuer.
				options.TokenValidationParameters.IssuerSigningKey = tokenAuthorizationOptions.SigningCredentials.Key;
				options.TokenValidationParameters.ValidAudience = tokenAuthorizationOptions.Audience;
				options.TokenValidationParameters.ValidIssuer = tokenAuthorizationOptions.Issuer;
				// When receiving a token, check that we've signed it.
				options.TokenValidationParameters.ValidateSignature = true;

				// When receiving a token, check that it is still valid.
				options.TokenValidationParameters.ValidateLifetime = true;

				// This defines the maximum allowable clock skew - i.e. provides a tolerance on the 
				// token expiry time when validating the lifetime. As we're creating the tokens locally
				// and validating them on the same machines which should have synchronised 
				// time, this can be set to zero. Where external tokens are used, some leeway here 
				// could be useful.
				options.TokenValidationParameters.ClockSkew = TimeSpan.Zero;

			});

			app.UseIdentity();

			app.UseFacebookAuthentication(options => {
				options.AppId = Configuration["Authentication:Facebook:Mobile2ndHandAppId"];
				options.AppSecret = Configuration["Authentication:Facebook:Mobile2ndHandAppSecret"];
				options.Scope.Add("email");
			});


			// To configure external authentication please see http://go.microsoft.com/fwlink/?LinkID=532715

			app.UseMvc(routes => {
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});

			app.UseCors("myPolicy");
			context.Database.EnsureCreated();
		}

		// Entry point for the application.
		public static void Main(string[] args) => WebApplication.Run<Startup>(args);
	}
}
