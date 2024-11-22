using DataLayer.Context;
using DataLayer.Interfaces;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Repositories
{
	public class CountryRepository : ICountryRepository
	{
		private readonly AppDbContext _appDbContext;

		public CountryRepository(AppDbContext appDbContext)
		{
			_appDbContext = appDbContext;
		}

		public async Task<Country> AddCountry(Country country)
		{
			await _appDbContext.Countries.AddAsync(country);
			await _appDbContext.SaveChangesAsync();
			return country;
		}

		
		public async Task<List<Country>> GetAllCountries()
		{
			return await _appDbContext.Countries.ToListAsync();
		}

		public async Task<Country?> GetCountryById(Guid? id)
		{
			return await _appDbContext.Countries.FirstOrDefaultAsync(c => c.Id == id);
		}

		public async Task<Country> GetCountryByName(string name)
		{
			return await _appDbContext.Countries.FirstOrDefaultAsync(c => c.Name == name);

		}
	}
}
