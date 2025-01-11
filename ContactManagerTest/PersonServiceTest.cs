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
using Moq;
using System.Linq.Expressions;
using Xunit.Sdk;
namespace ContactManagerTest
{
	public class PersonServiceTest
	{
		private readonly IPersonService _personService;
		private readonly Mock<IPersonRepository> mockPersonRepository;
		private readonly IPersonRepository _personRepository;
		private readonly ITestOutputHelper _outputHelper;
		private readonly IFixture fixture;
		public PersonServiceTest(ITestOutputHelper outputHelper)
		{
			fixture = new Fixture();
			mockPersonRepository = new Mock<IPersonRepository>();
			_personRepository = mockPersonRepository.Object;
			_personService = new PersonService(_personRepository);
			_outputHelper = outputHelper;
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
			Person person = request.ToPerson();
			mockPersonRepository.Setup(t => t.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);
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
			Person person = request.ToPerson();
			PersonResponse personResponse = person.ToPersonResponse();
			mockPersonRepository.Setup(t => t.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);
			PersonResponse response = await _personService.AddPerson(request);
			personResponse.Id = response.Id;
			response.Id.Should().NotBe(Guid.Empty);
			response.Should().Be(personResponse);
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
			Person person = fixture.Build<Person>()
				.With(p => p.Email, "random@gmail.com")
				.With(p => p.Country, null as Country)
				.Create();
			PersonResponse personResponse = person.ToPersonResponse();
			mockPersonRepository.Setup(t => t.GetPersonById(It.IsAny<Guid>())).ReturnsAsync(person);
			PersonResponse response1 = await _personService.GetPersonById(person.Id);
			response1.Should().Be(personResponse);

		}
		#endregion
		#region GetAllPersons
		[Fact]
		public async Task GetAllPersons_EmptyList()
		{
			var persons = new List<Person>();
			mockPersonRepository.Setup(t => t.GetAllPersons()).ReturnsAsync(persons);
			List<PersonResponse> responses = await _personService.GetAllPersons();
			responses.Should().BeEmpty();
		}
		[Fact]
		public async Task GetAllPersons_Proper()
		{
			List<Person> persons = new List<Person>() {
			fixture.Build<Person>().With(t=>t.Email,"someone@gmail.com").With(t=>t.Country,null as Country).Create(),
			fixture.Build<Person>().With(t=>t.Email,"someone1@gmail.com").With(t=>t.Country,null as Country).Create(),
			fixture.Build<Person>().With(t=>t.Email,"someone2@gmail.com").With(t=>t.Country,null as Country).Create(),
			};
			List<PersonResponse> responses = persons.Select(t => t.ToPersonResponse()).ToList();
			_outputHelper.WriteLine("Expected results");
			foreach (PersonResponse response in responses)
			{
				_outputHelper.WriteLine(response.ToString());
			};
			mockPersonRepository.Setup(t => t.GetAllPersons()).ReturnsAsync(persons);
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

			List<Person> persons = new List<Person>() {
	fixture.Build<Person>()
	.With(temp => temp.Email, "someone_1@example.com")
	.With(temp => temp.Country, null as Country)
	.Create(),

	fixture.Build<Person>()
	.With(temp => temp.Email, "someone_2@example.com")
	.With(temp => temp.Country, null as Country)
	.Create(),

	fixture.Build<Person>()
	.With(temp => temp.Email, "someone_3@example.com")
	.With(temp => temp.Country, null as Country)
	.Create()
   };

			List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();


			//print person_response_list_from_add
			_outputHelper.WriteLine("Expected:");
			foreach (PersonResponse person_response_from_add in person_response_list_expected)
			{
				_outputHelper.WriteLine(person_response_from_add.ToString());
			}

			mockPersonRepository.Setup(temp => temp
			.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
			 .ReturnsAsync(persons);

			//Act
			List<PersonResponse> persons_list_from_search = await _personService.GetFilteredPersons(nameof(Person.Name), "");

			//print persons_list_from_get
			_outputHelper.WriteLine("Actual:");
			foreach (PersonResponse person_response_from_get in persons_list_from_search)
			{
				_outputHelper.WriteLine(person_response_from_get.ToString());
			}

			//Assert
			persons_list_from_search.Should().BeEquivalentTo(person_response_list_expected);
		}
		[Fact]
		public async Task GetFilteredPersons_SearchByPersonName()
		{

			List<Person> persons = new List<Person>() {
		fixture.Build<Person>()
	.With(temp => temp.Email, "someone_1@example.com")
	.With(temp => temp.Country, null as Country)
	.Create(),

	fixture.Build<Person>()
	.With(temp => temp.Email, "someone_2@example.com")
	.With(temp => temp.Country, null as Country)
	.Create(),

	fixture.Build<Person>()
	.With(temp => temp.Email, "someone_3@example.com")
	.With(temp => temp.Country, null as Country)
	.Create()
   };

			List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();


			//print person_response_list_from_add
			_outputHelper.WriteLine("Expected:");
			foreach (PersonResponse person_response_from_add in person_response_list_expected)
			{
				_outputHelper.WriteLine(person_response_from_add.ToString());
			}

			mockPersonRepository.Setup(temp => temp
			.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
			 .ReturnsAsync(persons);

			//Act
			List<PersonResponse> persons_list_from_search = await _personService.GetFilteredPersons(nameof(Person.Name), "sa");

			//print persons_list_from_get
			_outputHelper.WriteLine("Actual:");
			foreach (PersonResponse person_response_from_get in persons_list_from_search)
			{
				_outputHelper.WriteLine(person_response_from_get.ToString());
			}

			//Assert
			persons_list_from_search.Should().BeEquivalentTo(person_response_list_expected);

		}
		#endregion
		#region GetSortedPersons
		[Fact]
		public async Task GetSortedPersons_DESC()
		{

			List<Person> persons = new List<Person>() {
			fixture.Build<Person>().With(t=>t.Email,"someone@gmail.com").With(t=>t.Country,null as Country).Create(),
			fixture.Build<Person>().With(t=>t.Email,"someone1@gmail.com").With(t=>t.Country,null as Country).Create(),
			fixture.Build<Person>().With(t=>t.Email,"someone2@gmail.com").With(t=>t.Country,null as Country).Create(),
			};
			List<PersonResponse> responses = persons.Select(t => t.ToPersonResponse()).ToList();
			_outputHelper.WriteLine("Expected results");
			foreach (PersonResponse response in responses)
			{
				_outputHelper.WriteLine(response.ToString());
			}
			mockPersonRepository.Setup(t => t.GetAllPersons()).ReturnsAsync(persons);
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
			sortedpeople.Should().BeInDescendingOrder(p => p.Name);

		}
		[Fact]
		public async Task GetSortedPersons_ASC()
		{
			List<Person> persons = new List<Person>() {
			fixture.Build<Person>().With(t=>t.Email,"someone@gmail.com").With(t=>t.Country,null as Country).Create(),
			fixture.Build<Person>().With(t=>t.Email,"someone1@gmail.com").With(t=>t.Country,null as Country).Create(),
			fixture.Build<Person>().With(t=>t.Email,"someone2@gmail.com").With(t=>t.Country,null as Country).Create(),
			};
			List<PersonResponse> responses = persons.Select(t => t.ToPersonResponse()).ToList();
			_outputHelper.WriteLine("Expected results");
			foreach (PersonResponse response in responses)
			{
				_outputHelper.WriteLine(response.ToString());
			}
			mockPersonRepository.Setup(t => t.GetAllPersons()).ReturnsAsync(persons);
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
			sortedpeople.Should().BeInAscendingOrder(p => p.Name);

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
			Person person = fixture.Build<Person>().With(t => t.Name, null as string).With(t => t.Email, "someone@gmail.com").With(t => t.Country, null as Country)
				.With(t => t.Gender, "Male").Create();
			PersonResponse personResponse = person.ToPersonResponse();
			PersonUpdateRequest updateRequest = personResponse.ToPersonUpdateRequest();
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
			Person person = fixture.Build<Person>().With(t => t.Email, "someone@gmail.com").With(t => t.Country, null as Country)
				.With(t => t.Gender, "Male").Create();
			PersonResponse personResponse = person.ToPersonResponse();
			PersonUpdateRequest updateRequest = personResponse.ToPersonUpdateRequest();
			mockPersonRepository.Setup(c => c.GetPersonById(It.IsAny<Guid>())).ReturnsAsync(person);
			mockPersonRepository.Setup(t => t.UpdatePerson(It.IsAny<Person>())).ReturnsAsync(person);
			PersonResponse responseAfterUpdate = await _personService.UpdatePerson(updateRequest);
			responseAfterUpdate.Should().Be(personResponse);
		}
		#endregion
		#region DeletePerson
		[Fact]
		public async Task DeletePerson_ValidId()
		{
			Person person = fixture.Build<Person>().With(t => t.Email, "someone@gmail.com").With(t => t.Country, null as Country)
				.With(t => t.Gender, "Male").Create();
			PersonResponse personResponse = person.ToPersonResponse();
			mockPersonRepository.Setup(c => c.GetPersonById(It.IsAny<Guid>())).ReturnsAsync(person);
			mockPersonRepository.Setup(t => t.DeletePerson(It.IsAny<Guid>())).ReturnsAsync(true);
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
