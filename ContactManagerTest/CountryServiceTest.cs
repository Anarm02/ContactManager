using AutoFixture;
using EntityFrameworkCoreMock;
using EntityLayer.DTOs.Countries;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using DataLayer.Context;
using DataLayer.Interfaces;
using DataLayer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;

namespace ContactManagerTest
{
	public class CountryServiceTest
	{
		private readonly ICountryService _countryService;
		private readonly Mock<ICountryRepository> _countriesRepositoryMock;
		private readonly ICountryRepository _countriesRepository;
		private readonly IFixture _fixture;
		public CountryServiceTest()
		{
			_fixture = new Fixture();
			_countriesRepositoryMock = new Mock<ICountryRepository>();
			_countriesRepository = _countriesRepositoryMock.Object;
			_countryService = new CountryService(_countriesRepository);
		}
		#region AddCountry
		[Fact]
		public async Task AddNullCountryRequest()
		{
			CountryAddRequest request = null;
			await Assert.ThrowsAsync<ArgumentNullException>(async () =>
			{
				await _countryService.AddCountry(request);
			});
		}
		[Fact]
		public async Task CountryNameNullRequest()
		{
			CountryAddRequest request = _fixture.Build<CountryAddRequest>().With(c => c.Name, null as string).Create();
			await Assert.ThrowsAsync<ArgumentException>(async () =>
			{
				await _countryService.AddCountry(request);
			});
		}
		[Fact]
		public async Task CountryNameDuplicateRequest()
		{
			CountryAddRequest first_country_request = _fixture.Build<CountryAddRequest>()
		.With(temp => temp.Name, "Test name").Create();
			CountryAddRequest second_country_request = _fixture.Build<CountryAddRequest>()
			  .With(temp => temp.Name, "Test name").Create();

			Country first_country = first_country_request.ToCountry();
			Country second_country = second_country_request.ToCountry();

			_countriesRepositoryMock
			 .Setup(temp => temp.AddCountry(It.IsAny<Country>()))
			 .ReturnsAsync(first_country);

			//Return null when GetCountryByCountryName is called
			_countriesRepositoryMock
			 .Setup(temp => temp.GetCountryByName(It.IsAny<string>()))
			 .ReturnsAsync(null as Country);

			CountryAddResponse first_country_from_add_country = await _countryService.AddCountry(first_country_request);

			//Act
			var action = async () =>
			{
				//Return first country when GetCountryByCountryName is called
				_countriesRepositoryMock.Setup(temp => temp.AddCountry(It.IsAny<Country>())).ReturnsAsync(first_country);

				_countriesRepositoryMock.Setup(temp => temp.GetCountryByName(It.IsAny<string>())).ReturnsAsync(first_country);

				await _countryService.AddCountry(second_country_request);
			};

			//Assert
			await action.Should().ThrowAsync<ArgumentException>();
		}
		[Fact]
		public async Task ProperCountry()
		{
			CountryAddRequest country_request = _fixture.Create<CountryAddRequest>();
			Country country = country_request.ToCountry();
			CountryAddResponse country_response = country.ToCountryResponse();

			_countriesRepositoryMock
			 .Setup(temp => temp.AddCountry(It.IsAny<Country>()))
			 .ReturnsAsync(country);

			_countriesRepositoryMock
			 .Setup(temp => temp.GetCountryByName(It.IsAny<string>()))
			 .ReturnsAsync(null as Country);


			//Act
			CountryAddResponse country_from_add_country = await _countryService.AddCountry(country_request);

			country.Id = country_from_add_country.Id;
			country_response.Id = country_from_add_country.Id;

			//Assert
			country_from_add_country.Id.Should().NotBe(Guid.Empty);
			country_from_add_country.Should().BeEquivalentTo(country_response);
		}
		#endregion
		#region GetAllCountries
		[Fact]
		public async Task GetAllCountries_EmptyList()
		{
			List<Country> country_empty_list = new List<Country>();
			_countriesRepositoryMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(country_empty_list);

			//Act
			List<CountryAddResponse> actual_country_response_list = await _countryService.GetAllCountries();

			//Assert
			actual_country_response_list.Should().BeEmpty();
		}
		[Fact]
		public async Task GetAllCountry_Proper()
		{
			List<Country> country_list = new List<Country>() {
		_fixture.Build<Country>()
		.With(temp => temp.People, null as List<Person>).Create(),
		_fixture.Build<Country>()
		.With(temp => temp.People, null as List<Person>).Create()
	  };

			List<CountryAddResponse> country_response_list = country_list.Select(temp => temp.ToCountryResponse()).ToList();

			_countriesRepositoryMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(country_list);

			//Act
			List<CountryAddResponse> actualCountryResponseList = await _countryService.GetAllCountries();

			//Assert
			actualCountryResponseList.Should().BeEquivalentTo(country_response_list);
		}
		#endregion
		#region GetCountry
		[Fact]
		public async Task GetCountry_Null()
		{
			Guid? id = null;
			CountryAddResponse? response = await _countryService.GetCountry(id);
			Assert.Null(response);
		}
		[Fact]
		public async Task GetCountry_Proper()
		{
		
			Country country = _fixture.Build<Country>()
			  .With(temp => temp.People, null as List<Person>)
			  .Create();
			CountryAddResponse country_response = country.ToCountryResponse();

			_countriesRepositoryMock
			 .Setup(temp => temp.GetCountryById(It.IsAny<Guid>()))
			 .ReturnsAsync(country);

			//Act
			CountryAddResponse? country_response_from_get = await _countryService.GetCountry(country.Id);


			//Assert
			country_response_from_get.Should().Be(country_response);

		}
		#endregion
	}
}
