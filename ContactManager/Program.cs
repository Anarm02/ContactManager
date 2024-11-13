using Microsoft.EntityFrameworkCore;
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
			app.UseStaticFiles();
			app.UseRouting();
			app.MapControllers();

			app.Run();
		}
	}
}
