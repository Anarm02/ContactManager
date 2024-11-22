using AutoFixture;
using EntityFrameworkCoreMock;
using EntityLayer.DTOs.Countries;
using EntityLayer.DTOs.Persons;
using EntityLayer.Entities;
using EntityLayer.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using DataLayer.Context;
using DataLayer.Interfaces;
using DataLayer.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace ContactManagerTest
{
	public class PersonServiceTest
	{
		private readonly IPersonService _personService;
		private readonly ICountryService _countryService;
		private readonly ITestOutputHelper _outputHelper;
		private readonly IFixture fixture;
		public PersonServiceTest(ITestOutputHelper outputHelper)
		{
			fixture = new Fixture();
			var initialCountries = new List<Country>() { };
			var initialPersons = new List<Person>() { };
			DbContextMock<AppDbContext> mockDB = new DbContextMock<AppDbContext>(new DbContextOptionsBuilder<AppDbContext>().Options);
			var context = mockDB.Object;
			mockDB.CreateDbSetMock(c => c.Countries, initialCountries);
			mockDB.CreateDbSetMock(c => c.Persons, initialPersons);
			_countryService = new CountryService(null);
			_personService = new PersonService(null);
			_outputHelper = outputHelper;
		}
		private async Task<PersonResponse> AddOnePerson(string v)
		{
			CountryAddRequest crequest = fixture.Create<CountryAddRequest>();
			CountryAddResponse country = await _countryService.AddCountry(crequest);
			PersonAddRequest request = fixture.Build<PersonAddRequest>()
				.With(p => p.Email, "random@gmail.com")
				.With(p => p.CountryId, country.Id)
				.Create();
			PersonResponse response = await _personService.AddPerson(request);
			return response;
		}
		private async Task<List<PersonResponse>> PersonAddDefault()
		{
			CountryAddRequest crequest = fixture.Create<CountryAddRequest>();
			CountryAddRequest crequest1 = fixture.Create<CountryAddRequest>();
			CountryAddResponse country = await _countryService.AddCountry(crequest);
			CountryAddResponse country1 = await _countryService.AddCountry(crequest1);
			PersonAddRequest request = fixture.Build<PersonAddRequest>()
				.With(p => p.Name, "Anar")
				.With(p => p.Email, "random@gmail.com")
				.With(p => p.CountryId, country.Id)
				.Create();
			PersonResponse person = await _personService.AddPerson(request);
			PersonAddRequest request1 = fixture.Build<PersonAddRequest>()
				.With(p => p.Name, "Muhammar")
				.With(p => p.Email, "random1@gmail.com")
				.With(p => p.CountryId, country1.Id)
				.Create();
			PersonResponse person1 = await _personService.AddPerson(request1);

			List<PersonResponse> responses = new List<PersonResponse>() { person, person1 };
			return responses;
		}
		#region AddPerson
		[Fact]
		public async Task AddPerson_NullArgument()
		{
			PersonAddRequest request = null;
			Func<Task> func = (async () =>
			{
				await _personService.AddPerson(request);
			});
			await func.Should().ThrowAsync<ArgumentNullException>();
			//await Assert.ThrowsAsync<ArgumentNullException>;
		}
		[Fact]
		public async Task AddPerson_NullName()
		{
			PersonAddRequest request = fixture.Build<PersonAddRequest>()
				.With(p => p.Name, null as string)
				.Create();
			Func<Task> func = async () =>
			{
				await _personService.AddPerson(request);
			};
			await func.Should().ThrowAsync<ArgumentException>();

		}
		[Fact]
		public async Task AddPerson_Proper()
		{
			PersonAddRequest request = fixture.Build<PersonAddRequest>().With(p => p.Email, "random@gmail.com").Create();
			PersonResponse response = await _personService.AddPerson(request);
			List<PersonResponse> responses = await _personService.GetAllPersons();
			response.Id.Should().NotBe(Guid.Empty);
			//Assert.True(response.Id != Guid.Empty);
			responses.Should().Contain(response);
			//Assert.Contains(response, responses);
		}
		#endregion
		#region GetPersonById
		[Fact]
		public async Task GetPersonById_NullId()
		{
			Guid? id = null;
			PersonResponse? response = await _personService.GetPersonById(id);
			response.Should().BeNull();
			//Assert.Null(response);
		}
		[Fact]
		public async Task GetPersonById_Proper()
		{
			CountryAddRequest crequest = fixture.Create<CountryAddRequest>();
			CountryAddResponse country = await _countryService.AddCountry(crequest);
			PersonAddRequest request = fixture.Build<PersonAddRequest>()
				.With(p => p.Email, "random@gmail.com")
				.With(p => p.CountryId, country.Id)
				.Create();
			PersonResponse response = await _personService.AddPerson(request);
			PersonResponse response1 = await _personService.GetPersonById(response.Id);
			response1.Should().Be(response);

		}
		#endregion
		#region GetAllPersons
		[Fact]
		public async Task GetAllPersons_EmptyList()
		{
			List<PersonResponse> responses = await _personService.GetAllPersons();
			responses.Should().BeEmpty();
		}
		[Fact]
		public async Task GetAllPersons_Proper()
		{
			var responses = await PersonAddDefault();
			_outputHelper.WriteLine("Expected results");
			foreach (PersonResponse response in responses)
			{
				_outputHelper.WriteLine(response.ToString());
			}
			List<PersonResponse> people = await _personService.GetAllPersons();
			_outputHelper.WriteLine("Actual results");
			foreach (PersonResponse personResponse in people)
			{
				_outputHelper.WriteLine(personResponse.ToString());
			}

			responses.Should().BeEquivalentTo(people);

		}
		#endregion
		#region GetFilteredPersons
		[Fact]
		public async Task GetFilteredPersons_EmptySearch()
		{

			var responses = await PersonAddDefault();
			_outputHelper.WriteLine("Expected results");
			foreach (PersonResponse response in responses)
			{
				_outputHelper.WriteLine(response.ToString());
			}
			List<PersonResponse> people = await _personService.GetFilteredPersons(nameof(Person.Name), "");
			_outputHelper.WriteLine("Actual results");
			foreach (PersonResponse personResponse in people)
			{
				_outputHelper.WriteLine(personResponse.ToString());
			}

			responses.Should().BeEquivalentTo(people);
		}
		[Fact]
		public async Task GetFilteredPersons_SearchByPersonName()
		{

			var responses = await PersonAddDefault();
			_outputHelper.WriteLine("Expected results");
			foreach (PersonResponse response in responses)
			{
				_outputHelper.WriteLine(response.ToString());
			}
			List<PersonResponse> people = await _personService.GetFilteredPersons(nameof(Person.Name), "ar");
			_outputHelper.WriteLine("Actual results");
			foreach (PersonResponse personResponse in people)
			{
				_outputHelper.WriteLine(personResponse.ToString());
			}
			responses.Should().Contain(p => p.Name.Contains("ar", StringComparison.OrdinalIgnoreCase));

		}
		#endregion
		#region GetSortedPersons
		[Fact]
		public async Task GetSortedPersons_DESC()
		{

			var responses = await PersonAddDefault();
			_outputHelper.WriteLine("Expected results");
			foreach (PersonResponse response in responses)
			{
				_outputHelper.WriteLine(response.ToString());
			}
			List<PersonResponse> allPersons = await _personService.GetAllPersons();
			List<PersonResponse> sortedpeople = await _personService.GetSortedPersons(allPersons, nameof(Person.Name), SortOrderOptions.DESC);

			_outputHelper.WriteLine("Actual results");
			foreach (PersonResponse personResponse in sortedpeople)
			{
				_outputHelper.WriteLine(personResponse.ToString());
			}
			//responses = responses.OrderByDescending(response => response.Name).ToList();
			//for (int i = 0; i < responses.Count; i++)
			//{
			//	Assert.Equal(responses[i], sortedpeople[i]);
			//}
			responses.Should().BeInDescendingOrder(p => p.Name);

		}
		[Fact]
		public async Task GetSortedPersons_ASC()
		{

			List<PersonResponse> responses = await PersonAddDefault();
			_outputHelper.WriteLine("Expected results");
			foreach (PersonResponse response in responses)
			{
				_outputHelper.WriteLine(response.ToString());
			}
			List<PersonResponse> allPersons = await _personService.GetAllPersons();
			List<PersonResponse> sortedpeople = await _personService.GetSortedPersons(allPersons, nameof(Person.Name), SortOrderOptions.ASC);

			_outputHelper.WriteLine("Actual results");
			foreach (PersonResponse personResponse in sortedpeople)
			{
				_outputHelper.WriteLine(personResponse.ToString());
			}
			//responses = responses.OrderBy(response => response.Name).ToList();
			//for (int i = 0; i < responses.Count; i++)
			//{
			//	Assert.Equal(responses[i], sortedpeople[i]);
			//}
			responses.Should().BeInAscendingOrder(p => p.Name);

		}


		#endregion
		#region UpdatePerson
		[Fact]
		public async Task UpdatePerson_NullArgument()
		{
			PersonUpdateRequest request = null;
			Func<Task> func = (async () =>
			{
				await _personService.UpdatePerson(request);
			});
			await func.Should().ThrowAsync<ArgumentException>();
		}
		[Fact]
		public async Task UpdatePerson_InvalidId()
		{
			PersonUpdateRequest request = fixture.Create<PersonUpdateRequest>();
			Func<Task> func = (async () =>
			{
				await _personService.UpdatePerson(request);
			});
			await func.Should().ThrowAsync<ArgumentException>();
		}
		[Fact]
		public async Task UpdatePerson_InvalidName()
		{
			PersonResponse person = await AddOnePerson("Japan");
			PersonUpdateRequest updateRequest = person.ToPersonUpdateRequest();
			updateRequest.Name = null;
			Func<Task> func = (async () =>
			{
				await _personService.UpdatePerson(updateRequest);
			});
			await func.Should().ThrowAsync<ArgumentException>();
		}



		[Fact]
		public async Task UpdatePerson_Proper()
		{
			PersonResponse person = await AddOnePerson("Japan");
			PersonUpdateRequest updateRequest = person.ToPersonUpdateRequest();
			updateRequest.Name = "Fanar";
			updateRequest.Email = "fanar@gmail.com";
			PersonResponse responseAfterUpdate = await _personService.UpdatePerson(updateRequest);
			PersonResponse updateResponse = await _personService.GetPersonById(responseAfterUpdate.Id);
			responseAfterUpdate.Should().Be(updateResponse);
		}
		#endregion
		#region DeletePerson
		[Fact]
		public async Task DeletePerson_ValidId()
		{
			PersonResponse person = await AddOnePerson("Japan");
			bool isDeleted = await _personService.DeletePerson(person.Id);
			isDeleted.Should().BeTrue();
		}
		[Fact]
		public async Task DeletePerson_InvalidId()
		{

			bool isDeleted = await _personService.DeletePerson(Guid.NewGuid());
			isDeleted.Should().BeFalse();
		}
		#endregion
	}
}
