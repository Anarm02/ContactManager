using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Interfaces;

namespace ContactManager.Controllers
{
	[Route("[controller]")]
	public class CountriesController : Controller
	{
		private readonly ICountryService countryService;

		public CountriesController(ICountryService countryService)
		{
			this.countryService = countryService;
		}
		[HttpGet]
		[Route("[action]")]
		public IActionResult UploadFromExcel()
		{
			return View();
		}
		[HttpPost]
		[Route("[action]")]
		public async Task<IActionResult> UploadFromExcel(IFormFile excelFile)
		{
			if (excelFile == null || excelFile.Length == 0)
			{
				ViewBag.ErrorMessage = "Please select an xlsx file";
				return View();
			}
			if (!Path.GetExtension(excelFile.FileName).Equals(".xlsx",StringComparison.OrdinalIgnoreCase))
			{
				ViewBag.ErrorMessage = "This file type is not supported";
				return View();
			}
			int uploadcount=await countryService.UploadFromExcel(excelFile);
			ViewBag.Message = $"{uploadcount} countries uploaded";
			return View();
		}
	}
}
