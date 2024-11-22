using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Interfaces
{
	public interface ICountryRepository
	{
		Task<Country> AddCountry(Country country);
		Task<List<Country>> GetAllCountries();
		Task<Country?> GetCountryById(Guid? id);
		Task<Country> GetCountryByName(string name);

	}
}
