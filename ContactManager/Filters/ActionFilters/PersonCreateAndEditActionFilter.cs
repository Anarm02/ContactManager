using Azure.Core;
using ContactManager.Controllers;
using DataLayer.Interfaces;
using DataLayer.Services;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactManager.Filters.ActionFilters
{
	public class PersonCreateAndEditActionFilter : IAsyncActionFilter
	{
		private readonly ICountryService countryService;

		public PersonCreateAndEditActionFilter(ICountryService countryService)
		{
			this.countryService = countryService;
		}

		public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			if (context.Controller is PersonsController personsController)
			{
				if (!personsController.ModelState.IsValid)
				{
					personsController.ViewBag.Countries = await countryService.GetAllCountries();
					personsController.ViewBag.Errors = personsController.ModelState.Values.SelectMany(x => x.Errors).SelectMany(x => x.ErrorMessage).ToList();
					var request = context.ActionArguments["request"];
					context.Result=personsController.View(request);
				}
				else
				{
					await next();
				}
			}
			else
			{
				await next();
			}
		}
	}
}
