using EntityLayer.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.DTOs.Auth
{
	public class RegisterDto
	{
		[Required(ErrorMessage = "Name can't be blank")]
		public string Name { get; set; }
		[Required(ErrorMessage = "Email can't be blank")]
		[EmailAddress]
		[Remote(action: "EmailAlreadyExist", controller: "Account", ErrorMessage = "Email already exists")]
		public string Email { get; set; }
		[Required(ErrorMessage = "Phone can't be blank")]
		[RegularExpression("[0-9]*$")]
		public string Phone { get; set; }
		[Required(ErrorMessage = "Password can't be blank")]
		[DataType(DataType.Password)]
		public string Password { get; set; }
		[Required(ErrorMessage = "Confirm Password can't be blank")]
		[DataType(DataType.Password)]
		[Compare("Password")]
		public string ConfirmPassword { get; set; }
		public UserTypeOptions UserType { get; set; }=UserTypeOptions.User;
	}
}
