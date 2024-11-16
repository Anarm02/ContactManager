using CsvHelper;
using CsvHelper.Configuration;
using EntityLayer.DTOs.Persons;
using EntityLayer.Entities;
using EntityLayer.Enums;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using ServiceLayer.Context;
using ServiceLayer.Helpers;
using ServiceLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services
{
	public class PersonService : IPersonService
	{
		private readonly AppDbContext context;
		private readonly ICountryService countryService;
		public PersonService()
		{
		}
		public PersonService(AppDbContext context, ICountryService countryService)
		{
			this.context = context ?? throw new ArgumentNullException(nameof(context));
			this.countryService = countryService ?? throw new ArgumentNullException(nameof(countryService));
		}




		public async Task<PersonResponse> AddPerson(PersonAddRequest? request)
		{
			if (request == null) throw new ArgumentNullException(nameof(request));
			ValidationHelper.ModelValidation(request);
			Person person = request.ToPerson();
			person.Id = Guid.NewGuid();

			context.Persons.Add(person);
			await context.SaveChangesAsync();

			//context.sp_addPerson(person);
			return person.ToPersonResponse();

		}

		public async Task<List<PersonResponse>> GetAllPersons()
		{
			var persons = await context.Persons.Include(p => p.Country).ToListAsync();


			return persons.Select(p => p.ToPersonResponse()).ToList();
		}

		public async Task<PersonResponse> GetPersonById(Guid? id)
		{
			if (id == null) return null;
			Person? person = await context.Persons.Include(p => p.Country).FirstOrDefaultAsync(x => x.Id == id);
			if (person == null) return null;
			return person.ToPersonResponse();
		}

		public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
		{
			List<PersonResponse> allPersons = await GetAllPersons();
			List<PersonResponse> filteredPerson = allPersons;
			if (string.IsNullOrEmpty(searchString) || string.IsNullOrEmpty(searchBy))
				return filteredPerson;
			switch (searchBy)
			{
				case nameof(PersonResponse.Name):
					filteredPerson = allPersons.Where(p => p.Name != null ? p.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
					break;
				case nameof(PersonResponse.Email):
					filteredPerson = allPersons.Where(p => p.Email != null ? p.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
					break;
				case nameof(PersonResponse.BirthDate):
					filteredPerson = allPersons.Where(p => p.BirthDate != null ? p.BirthDate.Value.ToString("dd MMMM yyyy").Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
					break;
				case nameof(PersonResponse.Address):
					filteredPerson = allPersons.Where(p => p.Address != null ? p.Address.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
					break;
				case nameof(PersonResponse.CountryId):
					filteredPerson = allPersons.Where(p => p.Country != null ? p.Country.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
					break;
				case nameof(PersonResponse.Gender):
					filteredPerson = allPersons.Where(p => p.Gender != null ? p.Gender.Equals(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
					break;
				case nameof(PersonResponse.Age):
					filteredPerson = allPersons.Where(p => p.Age != null ? p.Age.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
					break;
				default:
					filteredPerson = allPersons;
					break;
			}
			return filteredPerson;
		}

		public async Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> persons, string sortBy, SortOrderOptions sortOrder)
		{
			if (string.IsNullOrEmpty(sortBy))
				return persons;
			List<PersonResponse> sortedpersons = (sortBy, sortOrder) switch
			{
				(nameof(PersonResponse.Name), SortOrderOptions.ASC) =>
				persons.OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase).ToList(),
				(nameof(PersonResponse.Name), SortOrderOptions.DESC) =>
				persons.OrderByDescending(p => p.Name, StringComparer.OrdinalIgnoreCase).ToList(),
				(nameof(PersonResponse.Email), SortOrderOptions.ASC) =>
				persons.OrderBy(p => p.Email, StringComparer.OrdinalIgnoreCase).ToList(),
				(nameof(PersonResponse.Email), SortOrderOptions.DESC) =>
				persons.OrderByDescending(p => p.Email, StringComparer.OrdinalIgnoreCase).ToList(),
				(nameof(PersonResponse.BirthDate), SortOrderOptions.ASC) =>
				persons.OrderBy(p => p.BirthDate).ToList(),
				(nameof(PersonResponse.BirthDate), SortOrderOptions.DESC) =>
				persons.OrderByDescending(p => p.BirthDate).ToList(),
				(nameof(PersonResponse.Country), SortOrderOptions.ASC) =>
				persons.OrderBy(p => p.Country, StringComparer.OrdinalIgnoreCase).ToList(),
				(nameof(PersonResponse.Country), SortOrderOptions.DESC) =>
				persons.OrderByDescending(p => p.Country, StringComparer.OrdinalIgnoreCase).ToList(),
				(nameof(PersonResponse.Age), SortOrderOptions.ASC) =>
				persons.OrderBy(p => p.Age.ToString(), StringComparer.OrdinalIgnoreCase).ToList(),
				(nameof(PersonResponse.Age), SortOrderOptions.DESC) =>
				persons.OrderByDescending(p => p.Age.ToString(), StringComparer.OrdinalIgnoreCase).ToList(),
				(nameof(PersonResponse.Address), SortOrderOptions.ASC) =>
				persons.OrderBy(p => p.Address, StringComparer.OrdinalIgnoreCase).ToList(),
				(nameof(PersonResponse.Address), SortOrderOptions.DESC) =>
				persons.OrderByDescending(p => p.Address, StringComparer.OrdinalIgnoreCase).ToList(),
				(nameof(PersonResponse.Gender), SortOrderOptions.ASC) =>
				persons.OrderBy(p => p.Gender, StringComparer.OrdinalIgnoreCase).ToList(),
				(nameof(PersonResponse.Gender), SortOrderOptions.DESC) =>
				persons.OrderByDescending(p => p.Gender, StringComparer.OrdinalIgnoreCase).ToList(),
				_ => persons
			};
			return sortedpersons;
		}

		public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? request)
		{
			if (request == null) throw new ArgumentNullException(nameof(request));
			ValidationHelper.ModelValidation(request);
			Person? person = await context.Persons.FirstOrDefaultAsync(p => p.Id == request.Id);
			if (person == null) throw new ArgumentNullException(nameof(person));
			person.Name = request.Name;
			person.Email = request.Email;
			person.BirthDate = request.BirthDate;
			person.Gender = request.Gender.ToString();
			person.Address = request.Address;
			person.CountryId = request.CountryId;
			person.ReceiveNewsLetters = request.ReceiveNewsLetters;
			try
			{
				await context.SaveChangesAsync();
			}
			catch (Exception ex)
			{

				Console.WriteLine($"Error: {ex.Message}");

			}
			return person.ToPersonResponse();
		}

		public async Task<bool> DeletePerson(Guid? personId)
		{
			if (personId == null) throw new ArgumentNullException(nameof(personId));
			Person person = await context.Persons.Include(p => p.Country).FirstOrDefaultAsync(p => p.Id == personId);
			if (person == null) return false;
			var deleted = context.Persons.Remove(person);
			await context.SaveChangesAsync();
			return true;
		}

		public async Task<MemoryStream> GetPersonsCSV()
		{
			MemoryStream memoryStream = new MemoryStream();
			StreamWriter writer = new StreamWriter(memoryStream);
			CsvConfiguration csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);
			CsvWriter csvWriter = new CsvWriter(writer, csvConfiguration);

			csvWriter.WriteField(nameof(PersonResponse.Id));
			csvWriter.WriteField(nameof(PersonResponse.Name));
			csvWriter.WriteField(nameof(PersonResponse.Email));
			csvWriter.WriteField(nameof(PersonResponse.BirthDate));
			csvWriter.WriteField(nameof(PersonResponse.Age));
			csvWriter.WriteField(nameof(PersonResponse.Gender));
			csvWriter.WriteField(nameof(PersonResponse.Country));
			csvWriter.WriteField(nameof(PersonResponse.Address));
			csvWriter.WriteField(nameof(PersonResponse.ReceiveNewsLetters));
			csvWriter.NextRecord();
			csvWriter.Flush();
			var persons = context.Persons.Include(p => p.Country).Select(p => p.ToPersonResponse()).ToList();
			foreach (var person in persons)
			{
				csvWriter.WriteField(person.Id);
				csvWriter.WriteField(person.Name);
				csvWriter.WriteField(person.Email);
				if (person.BirthDate != null)
					csvWriter.WriteField(person.BirthDate.Value.ToString("yyyy-MM-dd"));
				csvWriter.WriteField(person.Age);
				csvWriter.WriteField(person.Gender);
				csvWriter.WriteField(person.Country);
				csvWriter.WriteField(person.Address);
				csvWriter.WriteField(person.ReceiveNewsLetters);
				csvWriter.NextRecord();
				csvWriter.Flush();

			}
			//await csvWriter.WriteRecordsAsync(persons);
			memoryStream.Position = 0;
			return memoryStream;
		}

		public async Task<MemoryStream> GetPersonsExcel()
		{
			MemoryStream stream = new MemoryStream();
			using (ExcelPackage package = new ExcelPackage(stream))
			{
				ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("PersonsSheet");
				worksheet.Cells["A1"].Value = "Name";
				worksheet.Cells["B1"].Value = "Email";
				worksheet.Cells["C1"].Value = "Birth Data";
				worksheet.Cells["D1"].Value = "Age";
				worksheet.Cells["E1"].Value = "Gender";
				worksheet.Cells["F1"].Value = "Country";
				worksheet.Cells["G1"].Value = "Address";
				worksheet.Cells["H1"].Value = "Receive News Letters";
				int row = 2;
				var responses = context.Persons.Include(p => p.Country).Select(p => p.ToPersonResponse()).ToList();
				foreach(var response in responses)
				{
					worksheet.Cells[row, 1].Value = response.Name;
					worksheet.Cells[row, 2].Value = response.Email;
					worksheet.Cells[row, 3].Value = response.BirthDate.Value.ToString("yyyy-MM-dd");
					worksheet.Cells[row, 4].Value = response.Age;
					worksheet.Cells[row, 5].Value = response.Gender;
					worksheet.Cells[row, 6].Value = response.Country;
					worksheet.Cells[row, 7].Value = response.Address;
					worksheet.Cells[row, 8].Value = response.ReceiveNewsLetters;
					row++;
				}

				worksheet.Cells[$"A1:H{row}"].AutoFitColumns();
				await package.SaveAsync();
			}
			stream.Position= 0;
			return stream;
		}
	}
}
