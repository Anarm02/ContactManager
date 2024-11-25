﻿using AutoFixture;
using ContactManager.Controllers;
using DataLayer.Interfaces;
using EntityLayer.DTOs.Countries;
using EntityLayer.DTOs.Persons;
using EntityLayer.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OfficeOpenXml.Sorting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactManagerTest
{
	public class PersonsControllerTest
	{
		private  ICountryService countryService;
		private  IPersonService personService;
		private Mock<IPersonService> personServiceMock;
		private Mock<ICountryService> countryServiceMock;
		private Fixture fixture;
        public PersonsControllerTest()
        {
            countryServiceMock=new Mock<ICountryService>();
			personServiceMock=new Mock<IPersonService>();
			fixture = new Fixture();
			countryService=countryServiceMock.Object;
			personService=personServiceMock.Object;

        }
		#region Index
		[Fact]
		public async Task Index_Valid()
		{
			PersonsController personsController=new PersonsController(personService,countryService);
			List<PersonResponse> responses=fixture.Create<List<PersonResponse>>();
			personServiceMock.Setup(t => t.GetFilteredPersons(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(responses);
			personServiceMock.Setup(t => t.GetSortedPersons(It.IsAny<List<PersonResponse>>(), It.IsAny<string>(), It.IsAny<SortOrderOptions>())).ReturnsAsync(responses);
			IActionResult result=await personsController.Index(fixture.Create<string>(), fixture.Create<string>(), fixture.Create<string>(), fixture.Create<SortOrderOptions>());
			var viewResult=Assert.IsType<ViewResult>(result);
			viewResult.ViewData.Model.Should().BeAssignableTo<List<PersonResponse>>();
			viewResult.ViewData.Model.Should().Be(responses);
		}
		#endregion
		#region Create
		[Fact]
		public async Task Create_Invalid()
		{

			PersonAddRequest request=fixture.Create<PersonAddRequest>();
			PersonResponse response=fixture.Create<PersonResponse>();
			List<CountryAddResponse> countries=fixture.Create<List<CountryAddResponse>>();
			PersonsController personsController = new PersonsController(personService, countryService);
			countryServiceMock.Setup(t=>t.GetAllCountries()).ReturnsAsync(countries);
			personServiceMock.Setup(t=>t.AddPerson(It.IsAny<PersonAddRequest>())).ReturnsAsync(response);
			personsController.ModelState.AddModelError("Email", "Name can't be empty");
			IActionResult result = await personsController.Create(request);
			var viewResult = Assert.IsType<ViewResult>(result);
			viewResult.ViewData.Model.Should().BeAssignableTo<PersonAddRequest>();
			viewResult.ViewData.Model.Should().Be(request);
		}
		[Fact]
		public async Task Create_Valid()
		{

			PersonAddRequest request = fixture.Create<PersonAddRequest>();
			PersonResponse response = fixture.Create<PersonResponse>();
			List<CountryAddResponse> countries = fixture.Create<List<CountryAddResponse>>();
			PersonsController personsController = new PersonsController(personService, countryService);
			countryServiceMock.Setup(t => t.GetAllCountries()).ReturnsAsync(countries);
			personServiceMock.Setup(t => t.AddPerson(It.IsAny<PersonAddRequest>())).ReturnsAsync(response);
			IActionResult result = await personsController.Create(request);
			RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);
			redirectResult.ActionName.Should().Be("Index");
		}
		#endregion
	}
}