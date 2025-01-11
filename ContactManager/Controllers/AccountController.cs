using EntityLayer.DTOs.Auth;
using EntityLayer.Entities;
using EntityLayer.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ContactManager.Controllers
{
	[Route("[controller]/[action]")]
	public class AccountController : Controller
	{
		private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;
		private readonly RoleManager<Role> _roleManager;

		public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<Role> roleManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_roleManager = roleManager;
		}

		[HttpGet]
		[Authorize("NotAuthorized")]
		public IActionResult Register()
		{
			return View();
		}
		[HttpPost]
		[Authorize("NotAuthorized")]
		public async Task<IActionResult> Register(RegisterDto registerDto)
		{
			if (!ModelState.IsValid)
			{
				ViewBag.Errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();
				return View(registerDto);
			}
			User user = new User() { Email = registerDto.Email, Name = registerDto.Name, UserName = registerDto.Email, PhoneNumber = registerDto.Phone };
			var result = await _userManager.CreateAsync(user, registerDto.Password);
			if (result.Succeeded)
			{
				if (registerDto.UserType == UserTypeOptions.Admin)
				{
					if(await _roleManager.FindByNameAsync(UserTypeOptions.Admin.ToString()) is null)
					{
						Role role=new Role() {Name=UserTypeOptions.Admin.ToString() };
						await _roleManager.CreateAsync(role);
					}
					await _userManager.AddToRoleAsync(user,UserTypeOptions.Admin.ToString());
				}
				else
				{
					await _userManager.AddToRoleAsync(user, UserTypeOptions.User.ToString());
				}
				await _signInManager.SignInAsync(user, isPersistent: true);
				return RedirectToAction("Index", "Persons");
			}
			else
			{
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError("Register", error.Description);
				}
				return View();
			}
		}
		[HttpGet]
		[Authorize("NotAuthorized")]
		public IActionResult Login()
		{
			return View();
		}
		[HttpPost]
		[Authorize("NotAuthorized")]
		public async Task<IActionResult> Login(LoginDto loginDto, string? returnUrl)
		{
			if (!ModelState.IsValid)
			{
				ViewBag.Errors = ModelState.Values.SelectMany(x => x.Errors).SelectMany(x => x.ErrorMessage).ToList();
			}
			var result = await _signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password, true, false);
			if (result.Succeeded)
			{
				var user=await _userManager.FindByEmailAsync(loginDto.Email);
				if (user != null)
				{
					if(await _userManager.IsInRoleAsync(user, UserTypeOptions.Admin.ToString()))
					{
						return RedirectToAction("Index", "Home", new { area = "Admin" });
					}
				}
				if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
				{
					return LocalRedirect(returnUrl);
				}
				return RedirectToAction("Index", "Persons");
			}
			ModelState.AddModelError("Login", "Something went wrong");
			return View(loginDto);
		}
		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Index", "Persons");
		}
		public async Task<IActionResult> EmailAlreadyExist(string email)
		{
			var user = await _userManager.FindByEmailAsync(email);
			if (user != null)
			{
				return Json(false);
			}
			return Json(true);
		}
	}
}
