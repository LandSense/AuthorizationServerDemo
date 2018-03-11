namespace LandSenseAuthentication
{
	using System;
	using System.Net.Http;
	using System.Net.Http.Headers;
	using System.Security.Claims;
	using System.Text;
	using System.Threading.Tasks;
	using Microsoft.AspNetCore.Authentication;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Http;
	using Microsoft.Extensions.Options;
	using Newtonsoft.Json.Linq;

	// You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
	public class LandSenseMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly LandSenseOptions _options;

		public LandSenseMiddleware(RequestDelegate next, IOptions<LandSenseOptions> options)
		{
			this._next = next;
			this._options = options.Value;
		}

		public Task Invoke(HttpContext httpContext)
		{
			string accessToken = httpContext.GetTokenAsync("access_token").Result;

			if (httpContext.User.Identity.IsAuthenticated && !string.IsNullOrEmpty(accessToken))
			{
				HttpClient client = new HttpClient();
				byte[] byteArray = Encoding.Default.GetBytes($"{this._options.ClientId}:{this._options.ClientSecret}");
				client.DefaultRequestHeaders.Authorization =
					new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

				Uri uri = new Uri($"{this._options.UserInfoEndpoint}?access_token={accessToken}");
				HttpResponseMessage responseMessage = client.GetAsync(uri).Result;
				responseMessage.EnsureSuccessStatusCode();

				string userInfoResponse = responseMessage.Content.ReadAsStringAsync().Result;

				JObject user = JObject.Parse(userInfoResponse);

				ClaimsPrincipal principal = httpContext.User;

				ClaimsIdentity identity = (ClaimsIdentity)principal.Identity;

				foreach (JProperty prop in (JToken)user)
				{
					identity.AddClaim(new Claim(prop.Name, (string)prop.Value));
				}
			}

			return this._next(httpContext);
		}
	}

	// Extension method used to add the middleware to the HTTP request pipeline.
	public static class LandSenseMiddlewareExtensions
	{
		public static IApplicationBuilder UseLandSenseAuthenticationMiddleware(this IApplicationBuilder builder, LandSenseOptions options)
		{
			return builder.UseMiddleware<LandSenseMiddleware>(Options.Create(options));
		}
	}
}