using EntityLayer.DTOs.Countries;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Interfaces
{
	public interface ICountryService
	{
		Task<CountryAddResponse> AddCountry(CountryAddRequest request);
		Task<List<CountryAddResponse>> GetAllCountries();
		Task<CountryAddResponse?> GetCountry(Guid? countryId);
		Task<int> UploadFromExcel(IFormFile file);
	}
}
