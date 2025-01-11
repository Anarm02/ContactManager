using ContactManager.Filters.ActionFilters;
using DataLayer.Context;
using DataLayer.Interfaces;
using DataLayer.Repositories;
using DataLayer.Services;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContactManager.StartupExtensions
{
	public static class ConfigureServicesExtensions
	{
		public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
		{
			var logger = services.BuildServiceProvider().GetRequiredService<ILogger<ResponseHeaderActionFilter>>();
			services.AddControllersWithViews(opt =>
			{
				opt.Filters.Add(new ResponseHeaderActionFilter(logger, "Global-key", "Global-value", 2));
				opt.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
			});
			services.AddScoped<ICountryRepository, CountryRepository>();
			services.AddScoped<IPersonRepository, PersonRepository>();
			services.AddScoped<ICountryService, CountryService>();
			services.AddScoped<IPersonService, PersonService>();
			services.AddDbContext<AppDbContext>(opt =>
			{
				opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
			});
			services.AddIdentity<User, Role>(opt =>
			{
				opt.Password.RequiredLength = 8;
				opt.Password.RequiredUniqueChars = 3;
			}).AddEntityFrameworkStores<AppDbContext>()
				.AddDefaultTokenProviders()
				.AddUserStore<UserStore<User,Role,AppDbContext,Guid>>()
				.AddRoleStore<RoleStore<Role,AppDbContext,Guid>>();
			services.AddAuthorization(opt =>
			{
				opt.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
				opt.AddPolicy("NotAuthorized", x =>
				{
					x.RequireAssertion(context =>
					{
						return !context.User.Identity.IsAuthenticated;
					});
				});
			});
			services.ConfigureApplicationCookie(opt =>
			{
				opt.LoginPath = "/Account/Login";
			});
		}
	}
}
