using EntityLayer.DTOs.Persons;
using EntityLayer.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;
using ServiceLayer.Interfaces;

namespace ContactManager.Controllers
{
	public class PersonsController : Controller
	{
		private readonly IPersonService _personService;
		private readonly ICountryService _countryService;

		public PersonsController(IPersonService personService, ICountryService countryService)
		{
			_personService = personService;
			_countryService = countryService;
		}

		[Route("persons/Index")]
		[Route("/")]
		public async Task<IActionResult> Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.Name), SortOrderOptions sortOrder = SortOrderOptions.ASC)
		{
			ViewBag.fields = new Dictionary<string, string>() {
				{nameof(PersonResponse.Name),"Person Name" },
				{nameof(PersonResponse.Email),"Email" },
				{nameof(PersonResponse.BirthDate),"Date of birth" },
				{nameof(PersonResponse.Address),"Address" },
				{nameof(PersonResponse.CountryId),"Country" },
				{nameof(PersonResponse.Gender),"Gender" },
			};
			ViewBag.sb = searchBy;
			ViewBag.ss = searchString;
			List<PersonResponse> responses = await _personService.GetFilteredPersons(searchBy, searchString);
			List<PersonResponse> sortedPersons = await _personService.GetSortedPersons(responses, sortBy, sortOrder);
			ViewBag.sortBy = sortBy;
			ViewBag.sortOrder = sortOrder.ToString();
			return View(sortedPersons);
		}
		[Route("persons/create")]
		[HttpGet]
		public async Task<IActionResult> Create()
		{
			var countries = await _countryService.GetAllCountries();
			ViewBag.Countries = countries.Select(c =>
			new SelectListItem { Text = c.Name, Value = c.Id.ToString() }).ToList();
			return View();
		}
		[Route("persons/create")]
		[HttpPost]
		public async Task<IActionResult> Create(PersonAddRequest request)
		{
			if (!ModelState.IsValid)
			{
				ViewBag.Countries = await _countryService.GetAllCountries();
				ViewBag.Errors = ModelState.Values.SelectMany(x => x.Errors).SelectMany(x => x.ErrorMessage).ToList();
				return View();
			}
			await _personService.AddPerson(request);
			return RedirectToAction("Index");
		}
		[HttpGet]
		[Route("persons/edit/{Id}")]
		public async Task<IActionResult> Edit(Guid Id)
		{
			var countries = await _countryService.GetAllCountries();
			ViewBag.Countries = countries.Select(c =>
			new SelectListItem { Text = c.Name, Value = c.Id.ToString() }).ToList();
			var result = await _personService.GetPersonById(Id);
			PersonUpdateRequest request = result.ToPersonUpdateRequest();
			return View(request);
		}
		[HttpPost]
		[Route("persons/edit/{Id}")]
		public async Task<IActionResult> Edit(PersonUpdateRequest request)
		{
			PersonResponse person = await _personService.GetPersonById(request.Id);
			if (person == null)
			{
				return RedirectToAction("Index");
			}
			if (!ModelState.IsValid)
			{
				var countries = await _countryService.GetAllCountries();
				ViewBag.Countries = countries.Select(c =>
				new SelectListItem { Text = c.Name, Value = c.Id.ToString() }).ToList();
				var result = await _personService.GetPersonById(request.Id);
				PersonUpdateRequest req = result.ToPersonUpdateRequest();
				return View(request);

			}
			var updatedResult = await _personService.UpdatePerson(request);
			return RedirectToAction("Index");
		}
		[HttpGet]
		[Route("persons/delete/{Id}")]
		public async Task<IActionResult> Delete(Guid Id)
		{
			var response = await _personService.GetPersonById(Id);
			if (response == null)
			{
				return RedirectToAction("Index");
			}
			return View(response);
		}
		[HttpPost]
		[Route("persons/delete/{Id}")]
		public async Task<IActionResult> Delete(PersonResponse person)
		{
			if (!ModelState.IsValid)
			{
				return View(person);
			}
			await _personService.DeletePerson(person.Id);
			return RedirectToAction("Index");
		}
		[Route("persons/personsPDF")]
		public async Task<IActionResult> PersonsPDF()
		{
			List<PersonResponse> people = await _personService.GetAllPersons();
			return new ViewAsPdf("PersonsPDF", people, ViewData)
			{
				PageMargins = new Margins { Bottom = 20, Top = 20, Right = 20, Left = 20 },
				PageOrientation = Orientation.Landscape
			};
		}
		[Route("persons/personsCSV")]
		public async Task<IActionResult> PersonsCSV()
		{
			var memory = await _personService.GetPersonsCSV();
			return File(memory, "application/octet-stream", "persons.csv");
		}
		[Route("persons/personsExcel")]
		public async Task<IActionResult> PersonsExcel()
		{
			var memory = await _personService.GetPersonsExcel();
			return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "persons.xlsx");
		}
	}
}
