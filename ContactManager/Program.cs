using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using ServiceLayer.Context;
using ServiceLayer.Interfaces;
using ServiceLayer.Services;

namespace ContactManager
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);
			builder.Services.AddControllersWithViews();
			builder.Services.AddScoped<ICountryService,CountryService>();
			builder.Services.AddScoped<IPersonService,PersonService>();
			builder.Services.AddDbContext<AppDbContext>(opt =>
			{
				opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
			});
			
			var app = builder.Build();
			if(app.Environment.IsDevelopment())
				app.UseDeveloperExceptionPage();
			RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");
			app.UseStaticFiles();
			app.UseRouting();
			app.MapControllers();

			app.Run();
		}
	}
}
