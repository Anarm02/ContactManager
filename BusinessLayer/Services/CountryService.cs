using EntityLayer.DTOs.Countries;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using DataLayer.Context;
using DataLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Services
{
	public class CountryService : ICountryService
	{

		private readonly ICountryRepository _repository;

		public CountryService()
		{
		}

		public CountryService( ICountryRepository repository )
		{
			
			_repository = repository;
		}


		public async Task<CountryAddResponse> AddCountry(CountryAddRequest request)
		{
			if (request == null)
				throw new ArgumentNullException(nameof(request));
			else if (request.Name == null)
				throw new ArgumentException(nameof(request.Name));
			if (await _repository.GetCountryByName(request.Name)!=null)
				throw new ArgumentException("Country already exist");
			Country country = request.ToCountry();
			country.Id = Guid.NewGuid();
			await _repository.AddCountry(country);
			return country.ToCountryResponse();

		}

		public async Task<List<CountryAddResponse>> GetAllCountries()
		{
			var countries = await _repository.GetAllCountries();
			return countries.Select(country => country.ToCountryResponse()).ToList();
		}

		public async Task<CountryAddResponse?> GetCountry(Guid? countryId)
		{
			if (countryId == null)
				return null;
			Country? country = await _repository.GetCountryById(countryId);
			if (country == null)
				return null;
			return country.ToCountryResponse();
		}

		public async Task<int> UploadFromExcel(IFormFile file)
		{
			MemoryStream memoryStream = new MemoryStream();
			await file.CopyToAsync(memoryStream);
			int addedCountryCount = 0;
			using (ExcelPackage package = new ExcelPackage(memoryStream))
			{
				ExcelWorksheet sheet = package.Workbook.Worksheets["Countries"];
				int rowcount = sheet.Dimension.Rows;
				
				for (int i = 2; i <= rowcount; i++)
				{
					string? countryname = sheet.Cells[i, 1].Value.ToString();
					if (!string.IsNullOrWhiteSpace(countryname) &&  await _repository.GetCountryByName(countryname)!=null)
					{
						Country country = new Country() { Name = countryname };
						await _repository.AddCountry(country);
						addedCountryCount++;
					}
				}
			}
			return addedCountryCount;
		}
	}
}
