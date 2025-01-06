using ContactManager.Filters.ActionFilters;
using DataLayer.Context;
using DataLayer.Interfaces;
using DataLayer.Repositories;
using DataLayer.Services;
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
			});
			services.AddScoped<ICountryRepository, CountryRepository>();
			services.AddScoped<IPersonRepository, PersonRepository>();
			services.AddScoped<ICountryService, CountryService>();
			services.AddScoped<IPersonService, PersonService>();
			services.AddDbContext<AppDbContext>(opt =>
			{
				opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
			});
		}
	}
}
