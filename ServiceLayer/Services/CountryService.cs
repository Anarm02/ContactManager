using EntityLayer.DTOs.Countries;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.Context;
using ServiceLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services
{
	public class CountryService : ICountryService
	{
	
		private readonly AppDbContext context;

		public CountryService()
		{
		}

		public CountryService(AppDbContext context)
		{
			this.context = context;
		}

		
		public async Task<CountryAddResponse> AddCountry(CountryAddRequest request)
		{
			if (request == null)
				throw new ArgumentNullException(nameof(request));
			else if (request.Name == null)
				throw new ArgumentException(nameof(request.Name));
			if (await context.Countries.AnyAsync(c=>c.Name==request.Name))
				throw new ArgumentException(nameof(request.Name));
			Country country = request.ToCountry();
			country.Id = Guid.NewGuid();
			await context.AddAsync(country);
			await context.SaveChangesAsync();
			return country.ToCountryResponse();

		}

		public async Task<List<CountryAddResponse>> GetAllCountries()
		{
			return await context.Countries.Select(country => country.ToCountryResponse()).ToListAsync();
		}

		public async Task<CountryAddResponse?> GetCountry(Guid? countryId)
		{
			if (countryId == null)
				return null;
			Country country =await context.Countries?.FirstOrDefaultAsync(c => c.Id == countryId);
			if (country == null)
				return null;
			return country.ToCountryResponse();
		}
	}
}
