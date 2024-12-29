using DataLayer.Context;
using DataLayer.Interfaces;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Repositories
{
	public class PersonRepository : IPersonRepository
	{
		private readonly AppDbContext _appDbContext;
		private readonly ILogger<PersonRepository> _logger;

		public PersonRepository(AppDbContext appDbContext, ILogger<PersonRepository> logger)
		{
			_appDbContext = appDbContext;
			_logger = logger;
		}

		public async Task<Person> AddPerson(Person person)
		{
			await _appDbContext.Persons.AddAsync(person);
			await _appDbContext.SaveChangesAsync();
			return person;
		}

		public async Task<bool> DeletePerson(Guid id)
		{
			_appDbContext.Persons.RemoveRange(_appDbContext.Persons.Where(p => p.Id == id));
			int count=await _appDbContext.SaveChangesAsync();
			return count > 0;
		}

		public async Task<List<Person>> GetAllPersons()
		{
			return await _appDbContext.Persons.Include(p=>p.Country).ToListAsync();
		}

		public async Task<Person?> GetPersonById(Guid? id)
		{
			return await _appDbContext.Persons.Include(p => p.Country).FirstOrDefaultAsync(p=>p.Id==id);
		}

		public async Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate)
		{
			_logger.LogInformation("GetFilteredPersons of PersonRepository");
			return await _appDbContext.Persons.Include(p => p.Country).Where(predicate).ToListAsync();
		}

		public async Task<Person> UpdatePerson(Person person)
		{
			Person matchingperson= await _appDbContext.Persons.Include(p => p.Country).FirstOrDefaultAsync(p => p.Id == person.Id);
			if (matchingperson != null)
			{
				matchingperson.Name = person.Name;
				matchingperson.Email = person.Email;
				matchingperson.BirthDate = person.BirthDate;
				matchingperson.Gender = person.Gender.ToString();
				matchingperson.Address = person.Address;
				matchingperson.CountryId = person.CountryId;
				matchingperson.ReceiveNewsLetters = person.ReceiveNewsLetters;
			};
			await _appDbContext.SaveChangesAsync();
			return matchingperson;
		}
	}
}
