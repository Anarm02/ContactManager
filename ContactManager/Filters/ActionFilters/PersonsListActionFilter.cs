using ContactManager.Controllers;
using EntityLayer.DTOs.Persons;
using EntityLayer.Enums;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactManager.Filters.ActionFilters
{
	public class PersonsListActionFilterFactoryAttribute : Attribute, IFilterFactory
	{
		public bool IsReusable =>false;

		public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
		{
			var filter=new PersonsListActionFilter();
			return filter;
		}
	}

	public class PersonsListActionFilter : IActionFilter
	{
		

		public  void OnActionExecuted(ActionExecutedContext context)
		{
			PersonsController personsController = (PersonsController)context.Controller;
			IDictionary<string, object?>? parameters = (IDictionary<string, object?>?)context.HttpContext.Items["arguments"];
			if (parameters is not null)
			{
				if (parameters.ContainsKey("searchBy"))
				{

					personsController.ViewData["sb"] = Convert.ToString(parameters["searchBy"]);
				}
				if (parameters.ContainsKey("searchString"))
				{

					personsController.ViewData["ss"] = Convert.ToString(parameters["searchString"]);
				}
				if (parameters.ContainsKey("sortBy"))
				{

					personsController.ViewData["sortBy"] = Convert.ToString(parameters["sortBy"]);
				}
				else
				{
					personsController.ViewData["sortBy"] = nameof(PersonResponse.Name);

				}
				if (parameters.ContainsKey("sortOrder"))
				{

					personsController.ViewData["sortOrder"] = Convert.ToString(parameters["sortOrder"]);
				}
				else
				{
					personsController.ViewData["sortOrder"] = nameof(SortOrderOptions.ASC);

				}
			}
			personsController.ViewBag.fields = new Dictionary<string, string>() {
				{nameof(PersonResponse.Name),"Person Name" },
				{nameof(PersonResponse.Email),"Email" },
				{nameof(PersonResponse.BirthDate),"Date of birth" },
				{nameof(PersonResponse.Address),"Address" },
				{nameof(PersonResponse.CountryId),"Country" },
				{nameof(PersonResponse.Gender),"Gender" },
			};
		}

		public  void OnActionExecuting(ActionExecutingContext context)
		{

			context.HttpContext.Items["arguments"] = context.ActionArguments;
			if (context.ActionArguments.ContainsKey("searchBy"))
			{
				string? searchBy = Convert.ToString(context.ActionArguments["searchBy"]);
				if (!string.IsNullOrEmpty(searchBy))
				{
					var searchOptions = new List<string>() {
					nameof(PersonResponse.Name),
					nameof(PersonResponse.Gender),
					nameof(PersonResponse.Email),
					nameof(PersonResponse.BirthDate),
					nameof(PersonResponse.CountryId),
					nameof(PersonResponse.Address),

					};
					if (searchOptions.Any(temp => temp == searchBy) == false)
					{

						context.ActionArguments["searchBy"] = nameof(PersonResponse.Name);


					}
				}
			}

		}
	}
}
