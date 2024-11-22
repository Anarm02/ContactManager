using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Interfaces
{
	public interface IPersonRepository
	{
		Task<Person> AddPerson(Person person);
		Task<bool> DeletePerson(Guid id);
		Task<Person> UpdatePerson(Person person);
		Task<List<Person>> GetAllPersons();
		Task<Person?> GetPersonById(Guid? id);
		Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate);
	}
}
