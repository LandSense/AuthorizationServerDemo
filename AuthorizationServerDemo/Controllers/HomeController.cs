namespace AuthorizationServerDemo.Controllers
{
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;

	[Route("")]
	public class HomeController : Controller
	{
		[HttpGet("")]
		public IActionResult Index()
		{
			return View();
		}

		[Authorize]
		[HttpGet("Secret")]
		public IActionResult Secret()
		{
			return View();
		}
	}
}