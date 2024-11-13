using EntityLayer.Entities;
using EntityLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.DTOs.Persons
{
	public class PersonResponse
	{
		public Guid Id { get; set; }
		public string? Name { get; set; }
		public string? Email { get; set; }
		public DateTime? BirthDate { get; set; }
		public string? Gender { get; set; }
		public string? Address { get; set; }
		public Guid? CountryId { get; set; }
		public bool ReceiveNewsLetters { get; set; }
        public string? Country { get; set; }
        public double? Age { get; set; }
       
        public override bool Equals(object? obj)
		{
			if(obj==null) return false;
			if(obj.GetType() != typeof(PersonResponse)) return false;
			PersonResponse response = (PersonResponse)obj;
			return this.Id == response.Id && this.Name == response.Name
				&& this.Email == response.Email
				&& this.BirthDate == response.BirthDate
				&& this.Gender == response.Gender
				&& this.Address == response.Address
				&& this.CountryId == response.CountryId
				&& this.ReceiveNewsLetters == response.ReceiveNewsLetters;
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		public override string ToString()
		{
			return $"Person Id:{Id}, Person Name:{Name}";
		}
		public PersonUpdateRequest ToPersonUpdateRequest()
		{
			return new PersonUpdateRequest()
			{
				Id = Id,
				Name = Name,
				Email = Email,
				BirthDate = BirthDate,
				Address = Address,
				CountryId = CountryId,
				ReceiveNewsLetters = ReceiveNewsLetters,
				Gender = (GenderOptions)Enum.Parse(typeof(GenderOptions), Gender, true)
			};
		}
	}
	public static class PersonExtension
	{
		public static PersonResponse ToPersonResponse(this Person person)
		{
			return new PersonResponse()
			{
				Id = person.Id,
				Name = person.Name,
				Address = person.Address,
				CountryId = person.CountryId,
				BirthDate = person.BirthDate,
				Gender = person.Gender,
				Email = person.Email,
				ReceiveNewsLetters = person.ReceiveNewsLetters,
				Age = (person.BirthDate != null) ? Math.Round((DateTime.Now - person.BirthDate.Value).TotalDays / 365.25) : 0,
				Country = person.Country?.Name

			};
		}
	}
}
