using EntityLayer.DTOs.Countries;
using EntityLayer.DTOs.Persons;
using EntityLayer.Entities;
using EntityLayer.Enums;
using ServiceLayer.Interfaces;
using ServiceLayer.Services;
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
		public PersonServiceTest(ITestOutputHelper outputHelper)
		{
			_personService = new PersonService();
			_countryService = new CountryService();
			_outputHelper = outputHelper;
		}
		public async Task<CountryAddResponse> DefaultCountryAdd(string name)
		{
			CountryAddRequest country = new() { Name = name };
		CountryAddResponse countryAdd =await _countryService.AddCountry(country);
			return countryAdd;
		}
		public async Task<PersonResponse> AddOnePerson(string country)
		{
			CountryAddResponse country1=await DefaultCountryAdd(country);
			PersonAddRequest request = new() { Name = "anar", Address = "test", BirthDate = DateTime.Parse("2000-01-01"), CountryId = country1.Id, Email = "test@mail.com", Gender = GenderOptions.Male, ReceiveNewsLetters = true };
			PersonResponse person =await _personService.AddPerson(request);
			return person;

		}
		public async Task<List<PersonResponse>> PersonAddDefault()
		{
			
			
			CountryAddResponse countryAdd=await DefaultCountryAdd("Japan");
			CountryAddResponse countryAdd1 =await DefaultCountryAdd("USA");
			List<PersonAddRequest> requests = new()
			{
			 new() { Name = "fanar", Address = "email@sample.com", BirthDate = DateTime.Parse("2000-01-01"), CountryId = countryAdd.Id, Email = "email@sample.com", Gender = GenderOptions.Male, ReceiveNewsLetters = true }
				,
			 new() { Name = "anar", Address = "femail@sample.com", BirthDate = DateTime.Parse("2001-01-01"), CountryId = countryAdd1.Id, Email = "femail@sample.com", Gender = GenderOptions.Female, ReceiveNewsLetters = true }

			};
			List<PersonResponse> responses = new();
			foreach (PersonAddRequest request in requests)
			{
				responses.Add(await _personService.AddPerson(request));
			}
			return responses;
		}
		#region AddPerson
		[Fact]
		public async Task AddPerson_NullArgument()
		{
			PersonAddRequest request = null;
			await Assert.ThrowsAsync<ArgumentNullException>(async () =>
			{
				await _personService.AddPerson(request);
			});
		}
		[Fact]
		public async Task AddPerson_NullName()
		{
			PersonAddRequest request = new() { Name = null };
			await Assert.ThrowsAsync<ArgumentException>(async () =>
			{
				await _personService.AddPerson(request);
			});
		}
		[Fact]
		public async Task AddPerson_Proper()
		{
			PersonAddRequest request = new() { Name = "anar", Address = "test", BirthDate = DateTime.Parse("2000-01-01"), CountryId = Guid.NewGuid(), Email = "test@mail.com", Gender = GenderOptions.Male, ReceiveNewsLetters = true };
			PersonResponse response =await _personService.AddPerson(request);
			List<PersonResponse> responses =await _personService.GetAllPersons();
			Assert.True(response.Id != Guid.Empty);
			Assert.Contains(response, responses);
		}
		#endregion
		#region GetPersonById
		[Fact]
		public async Task GetPersonById_NullId()
		{
			Guid? id = null;
			PersonResponse? response =await _personService.GetPersonById(id);
			Assert.Null(response);
		}
		[Fact]
		public async Task GetPersonById_Proper()
		{
			CountryAddRequest crequest = new() { Name = "Japan" };
			CountryAddResponse country =await _countryService.AddCountry(crequest);
			PersonAddRequest request = new() { Name = "anar", Address = "email@sample.com", BirthDate = DateTime.Parse("2000-01-01"), CountryId = country.Id, Email = "email@sample.com", Gender = GenderOptions.Male, ReceiveNewsLetters = true };
			PersonResponse response =await _personService.AddPerson(request);
			PersonResponse response1 =await _personService.GetPersonById(response.Id);
			Assert.Equal(response1, response);

		}
		#endregion
		#region GetAllPersons
		[Fact]
		public async Task GetAllPersons_EmptyList()
		{
			List<PersonResponse> responses =await _personService.GetAllPersons();
			Assert.Empty(responses);
		}
		[Fact]
		public async Task GetAllPersons_Proper()
		{

			var responses =await PersonAddDefault();
			_outputHelper.WriteLine("Expected results");
			foreach (PersonResponse response in responses)
			{
				_outputHelper.WriteLine(response.ToString());
			}
			List<PersonResponse> people =await _personService.GetAllPersons();
			_outputHelper.WriteLine("Actual results");
			foreach (PersonResponse personResponse in people)
			{
				_outputHelper.WriteLine(personResponse.ToString());
			}
			foreach (PersonResponse response in people)
			{
				Assert.Contains(response, responses);
			}

		}
		#endregion
		#region GetFilteredPersons
		[Fact]
		public async Task GetFilteredPersons_EmptySearch()
		{

			List<PersonResponse> responses =await PersonAddDefault();
			_outputHelper.WriteLine("Expected results");
			foreach (PersonResponse response in responses)
			{
				_outputHelper.WriteLine(response.ToString());
			}
			List<PersonResponse> people =await _personService.GetFilteredPersons(nameof(Person.Name), "");
			_outputHelper.WriteLine("Actual results");
			foreach (PersonResponse personResponse in people)
			{
				_outputHelper.WriteLine(personResponse.ToString());
			}
			foreach (PersonResponse response in people)
			{
				Assert.Contains(response, responses);
			}

		}
		[Fact]
		public async Task GetFilteredPersons_SearchByPersonName()
		{

			List<PersonResponse> responses =await PersonAddDefault();
			_outputHelper.WriteLine("Expected results");
			foreach (PersonResponse response in responses)
			{
				_outputHelper.WriteLine(response.ToString());
			}
			List<PersonResponse> people =await _personService.GetFilteredPersons(nameof(Person.Name), "ar");
			_outputHelper.WriteLine("Actual results");
			foreach (PersonResponse personResponse in people)
			{
				_outputHelper.WriteLine(personResponse.ToString());
			}
			foreach (PersonResponse response in people)
			{
				if (response.Name != null && response.Name.Contains("ar", StringComparison.OrdinalIgnoreCase))
				{

					Assert.Contains(response, responses);
				}
			}

		}
		#endregion
		#region GetSortedPersons
		[Fact]
		public async Task GetSortedPersons_DESC()
		{

			List<PersonResponse> responses =await PersonAddDefault();
			_outputHelper.WriteLine("Expected results");
			foreach (PersonResponse response in responses)
			{
				_outputHelper.WriteLine(response.ToString());
			}
			List<PersonResponse> allPersons =await _personService.GetAllPersons();
			List<PersonResponse> sortedpeople =await _personService.GetSortedPersons(allPersons, nameof(Person.Name), SortOrderOptions.DESC);

			_outputHelper.WriteLine("Actual results");
			foreach (PersonResponse personResponse in sortedpeople)
			{
				_outputHelper.WriteLine(personResponse.ToString());
			}
			responses = responses.OrderByDescending(response => response.Name).ToList();
			for (int i = 0; i < responses.Count; i++)
			{
				Assert.Equal(responses[i], sortedpeople[i]);
			}

		}
		[Fact]
		public async Task GetSortedPersons_ASC()
		{

			List<PersonResponse> responses =await PersonAddDefault();
			_outputHelper.WriteLine("Expected results");
			foreach (PersonResponse response in responses)
			{
				_outputHelper.WriteLine(response.ToString());
			}
			List<PersonResponse> allPersons =await _personService.GetAllPersons();
			List<PersonResponse> sortedpeople =await _personService.GetSortedPersons(allPersons, nameof(Person.Name), SortOrderOptions.ASC);

			_outputHelper.WriteLine("Actual results");
			foreach (PersonResponse personResponse in sortedpeople)
			{
				_outputHelper.WriteLine(personResponse.ToString());
			}
			responses = responses.OrderBy(response => response.Name).ToList();
			for (int i = 0; i < responses.Count; i++)
			{
				Assert.Equal(responses[i], sortedpeople[i]);
			}

		}
		#endregion
		#region UpdatePerson
		[Fact]
		public async Task UpdatePerson_NullArgument()
		{
			PersonUpdateRequest request = null;
			await Assert.ThrowsAsync<ArgumentNullException>(async () =>
			{
				await _personService.UpdatePerson(request);
			});
		}
		[Fact]
		public async Task UpdatePerson_InvalidId()
		{
			PersonUpdateRequest request = new PersonUpdateRequest() { Id = Guid.NewGuid() };
			await Assert.ThrowsAsync<ArgumentException>(async () =>
			{
			await	_personService.UpdatePerson(request);
			});
		}
		[Fact]
		public async Task UpdatePerson_InvalidName()
		{
			PersonResponse person =await AddOnePerson("Japan");
			PersonUpdateRequest updateRequest = person.ToPersonUpdateRequest();
			updateRequest.Name = null;
			await Assert.ThrowsAsync<ArgumentException>(async () =>
			{
			await	_personService.UpdatePerson(updateRequest);
			});
		}
		[Fact]
		public async Task UpdatePerson_Proper()
		{
			PersonResponse person =await AddOnePerson("Japan");
			PersonUpdateRequest updateRequest = person.ToPersonUpdateRequest();
			updateRequest.Name = "Fanar";
			updateRequest.Email = "fanar@gmail.com";
			PersonResponse responseAfterUpdate =await	_personService.UpdatePerson(updateRequest);
			PersonResponse updateResponse =await _personService.GetPersonById(responseAfterUpdate.Id);
			Assert.Equal(responseAfterUpdate, updateResponse);
		}
		#endregion
		#region DeletePerson
		[Fact]
		public async Task DeletePerson_ValidId()
		{
			PersonResponse person =await AddOnePerson("Japan");
			bool isDeleted=await _personService.DeletePerson(person.Id);
			Assert.True(isDeleted);
		}
		[Fact]
		public async Task DeletePerson_InvalidId()
		{
			
			bool isDeleted =await _personService.DeletePerson(Guid.NewGuid());
			Assert.False(isDeleted);
		}
		#endregion
	}
}
