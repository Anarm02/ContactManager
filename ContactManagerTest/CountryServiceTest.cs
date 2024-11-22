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

namespace ContactManagerTest
{
	public class CountryServiceTest
	{
		private readonly ICountryService _countryService;
		private readonly IFixture _fixture;
		public CountryServiceTest()
		{
			_fixture = new Fixture();
			var initialCountries = new List<Country>() { };
			DbContextMock<AppDbContext> mockDB = new DbContextMock<AppDbContext>(new DbContextOptionsBuilder<AppDbContext>().Options);	
			var context=mockDB.Object;
			mockDB.CreateDbSetMock(c=>c.Countries,initialCountries);
			_countryService = new CountryService(null);
		}
		#region AddCountry
		[Fact]
		public async Task AddNullCountryRequest()
		{
			CountryAddRequest request = null;
			await Assert.ThrowsAsync<ArgumentNullException>(async () =>
			{
			await	_countryService.AddCountry(request);
			});
		}
		[Fact]
		public async Task CountryNameNullRequest()
		{
			CountryAddRequest request=_fixture.Build<CountryAddRequest>().With(c=>c.Name,null as string).Create();
			await Assert.ThrowsAsync<ArgumentException>(async () =>
			{
				await _countryService.AddCountry(request);
			});
		}
		[Fact]
		public async Task CountryNameDuplicateRequest()
		{
			CountryAddRequest request = _fixture.Build<CountryAddRequest>()
				.With(c=>c.Name,"USA")
				.Create();
			CountryAddRequest request1 = _fixture.Build<CountryAddRequest>()
				.With(c => c.Name, "USA")
				.Create();
			await Assert.ThrowsAsync<ArgumentException>(async () =>
			{
				await _countryService.AddCountry(request);
				await _countryService.AddCountry(request1);
			});
		}
		[Fact]
		public async Task ProperCountry()
		{
			CountryAddRequest request=_fixture.Create<CountryAddRequest>();
			CountryAddResponse response=await _countryService.AddCountry(request);
			List<CountryAddResponse> responses= await _countryService.GetAllCountries();
			Assert.True(response.Id!=Guid.Empty);
			Assert.Contains(response, responses);
		}
		#endregion
		#region GetAllCountries
		[Fact]
		public async Task GetAllCountries_EmptyList()
		{
			List<CountryAddResponse> countries=await _countryService.GetAllCountries();
			Assert.Empty(countries);
		}
		[Fact]
		public async Task GetAllCountry_Proper() {
			var request1 = _fixture.Create<CountryAddRequest>();
			var request2 = _fixture.Create<CountryAddRequest>();
			List<CountryAddRequest> requests=new List<CountryAddRequest>() {
			request1,request2
			};
			List<CountryAddResponse> responses=new List<CountryAddResponse>();
			foreach (CountryAddRequest request in requests) {
				responses.Add(await _countryService.AddCountry(request));
			}
			List<CountryAddResponse> countries =await _countryService.GetAllCountries();
			foreach (CountryAddResponse country in countries) { 
			
			Assert.Contains(country,responses);
			}
		}
		#endregion
		#region GetCountry
		[Fact]
		public async Task GetCountry_Null()
		{
			Guid? id=null;
			CountryAddResponse? response=await _countryService.GetCountry(id);
			Assert.Null(response);
		}
		[Fact]
		public async Task GetCountry_Proper()
		{
			CountryAddRequest request=_fixture.Create<CountryAddRequest>();
			CountryAddResponse response =await _countryService.AddCountry(request);
			CountryAddResponse response1 =await _countryService.GetCountry(response.Id);
			Assert.Equal(response,response1);

		}
		#endregion
	}
}
