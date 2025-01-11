using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ContactManager.Controllers
{
	[AllowAnonymous]
	public class HomeController : Controller
	{
		[Route("/Error")]
		public IActionResult Error()
		{
			IExceptionHandlerPathFeature? feature=HttpContext.Features.Get<IExceptionHandlerPathFeature>();
			if (feature!=null && feature.Error != null)
			{
				ViewBag.Error = feature.Error.Message;
			}
			return View();
		}
	}
}
