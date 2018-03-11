namespace AuthorizationServerDemo
{
	using LandSenseAuthentication;
	using Microsoft.AspNetCore.Authentication.Cookies;
	using Microsoft.AspNetCore.Authentication.OpenIdConnect;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.IdentityModel.Protocols.OpenIdConnect;

	public class Startup
	{
		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder().SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true).AddEnvironmentVariables();

			Configuration = builder.Build();
		}

		public IConfigurationRoot Configuration { get; }

		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseAuthentication();

			app.UseLandSenseAuthenticationMiddleware(new LandSenseOptions
			{
				ClientId = Configuration["LandSenseCredentials:ClientId"],
				ClientSecret = Configuration["LandSenseCredentials:ClientSecret"],
				UserInfoEndpoint = "https://as.landsense.eu/oauth/userinfo"
			});

			app.UseMvc();
		}

		public void ConfigureServices(IServiceCollection services)
		{
			// See https://github.com/aspnet/Security/tree/dev/samples/OpenIdConnectSample

			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
			})
			.AddCookie()
			.AddOpenIdConnect(options =>
			{
				options.ClientId = Configuration["LandSenseCredentials:ClientId"];
				options.ClientSecret = Configuration["LandSenseCredentials:ClientSecret"];

				options.Authority = "https://as.landsense.eu/";

				options.ResponseType = OpenIdConnectResponseType.Code;

				options.SaveTokens = true;

				// This is done via LandSenseAuthenticationMiddleware
				////options.GetClaimsFromUserInfoEndpoint = true;

				options.Scope.Add("openid");
				options.Scope.Add("profile");
				options.Scope.Add("email");
				options.Scope.Add("landsense");
			});

			services.AddMvc();
		}
	}
}