using EntityLayer.Entities;
using EntityLayer.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.DTOs.Persons
{
	public class PersonUpdateRequest
	{
		[Required(ErrorMessage = "Person id can't be empty")]
		public Guid Id { get; set; }
		[Required(ErrorMessage = "Name can't be empty")]
		public string? Name { get; set; }
		[Required(ErrorMessage = "Mail can't be empty")]
		[EmailAddress(ErrorMessage = "Mail address should be valid")]
		public string? Email { get; set; }
		public DateTime? BirthDate { get; set; }
		public GenderOptions? Gender { get; set; }
		public string? Address { get; set; }
		public Guid? CountryId { get; set; }
		public bool ReceiveNewsLetters { get; set; }
		public Person ToPerson()
		{
			return new Person
			{
				Id = Id,
				Name = Name,
				Email = Email,
				BirthDate = BirthDate,
				Address = Address,
				CountryId = CountryId,
				Gender = Gender.ToString(),
				ReceiveNewsLetters = ReceiveNewsLetters
			};
		}
	}
}
