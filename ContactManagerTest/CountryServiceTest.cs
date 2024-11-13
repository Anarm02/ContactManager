using EntityLayer.DTOs.Countries;
using ServiceLayer.Interfaces;
using ServiceLayer.Services;
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

		public CountryServiceTest()
		{
			_countryService = new CountryService();
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
			CountryAddRequest request=new CountryAddRequest() { Name=null};
			await Assert.ThrowsAsync<ArgumentException>(async () =>
			{
				await _countryService.AddCountry(request);
			});
		}
		[Fact]
		public async Task CountryNameDuplicateRequest()
		{
			CountryAddRequest request = new CountryAddRequest() { Name = "USA" };
			CountryAddRequest request1 = new CountryAddRequest() { Name = "USA" };
			await Assert.ThrowsAsync<ArgumentException>(async () =>
			{
				await _countryService.AddCountry(request);
				await _countryService.AddCountry(request1);
			});
		}
		[Fact]
		public async Task ProperCountry()
		{
			CountryAddRequest request=new CountryAddRequest() { Name="japan"};
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
			List<CountryAddRequest> requests=new List<CountryAddRequest>() {
			new CountryAddRequest(){Name="USA"},
			new CountryAddRequest(){Name="UK"}
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
			CountryAddRequest request=new CountryAddRequest() {Name="Japan" };
			CountryAddResponse response =await _countryService.AddCountry(request);
			CountryAddResponse response1 =await _countryService.GetCountry(response.Id);
			Assert.Equal(response,response1);

		}
		#endregion
	}
}
