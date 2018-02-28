namespace AuthorizationServerDemo
{
	using Microsoft.AspNetCore.Authentication.Cookies;
	using Microsoft.AspNetCore.Authentication.OpenIdConnect;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.AspNetCore.Http;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.IdentityModel.Protocols.OpenIdConnect;

	public class Startup
	{
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseAuthentication();

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
					options.ClientId = "68578259-89db-7656-916b-b3f70c03e7bd@as.landsense.eu";
					options.ClientSecret = "f433f6d5cea5a185a9fde263ce722ad8d42ae287c3d00b7d99d73f17a772b8b4";

					options.Authority = "https://as.landsense.eu/";

					options.ResponseType = OpenIdConnectResponseType.Code;

					options.SaveTokens = true;

					// Not working with the encrypted UserInfo-Endpoint response
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