using EntityLayer.DTOs.Persons;
using EntityLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Interfaces
{
	public interface IPersonService
	{
		Task<PersonResponse> AddPerson(PersonAddRequest? request);
		Task<List<PersonResponse>> GetAllPersons();
		Task<PersonResponse> GetPersonById(Guid? id);
		Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString);
		Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> persons,string sortBy, SortOrderOptions sortOrder);
		Task<PersonResponse> UpdatePerson(PersonUpdateRequest? request);
		Task<bool> DeletePerson(Guid? personId);
		Task<MemoryStream> GetPersonsCSV();
		Task<MemoryStream> GetPersonsExcel();
	}
}
