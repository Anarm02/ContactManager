using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using DataLayer.Context;
using DataLayer.Interfaces;
using DataLayer.Services;
using DataLayer.Repositories;

namespace ContactManager
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);
			builder.Services.AddControllersWithViews();
			builder.Services.AddScoped<ICountryRepository,CountryRepository>();
			builder.Services.AddScoped<IPersonRepository,PersonRepository>();
			builder.Services.AddScoped<ICountryService,CountryService>();
			builder.Services.AddScoped<IPersonService,PersonService>();
			builder.Services.AddDbContext<AppDbContext>(opt =>
			{
				opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
			});
			
			var app = builder.Build();
			if(app.Environment.IsDevelopment())
				app.UseDeveloperExceptionPage();
			if(builder.Environment.IsEnvironment("Test")==false)
				RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");
			app.UseStaticFiles();
			app.UseRouting();
			app.MapControllers();

			app.Run();
		}
	}
}
