using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using DataLayer.Context;
using DataLayer.Interfaces;
using DataLayer.Services;
using DataLayer.Repositories;
using Serilog;
namespace ContactManager
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);
			builder.Host.UseSerilog((HostBuilderContext context,IServiceProvider provider,LoggerConfiguration configuration) =>
			{
				configuration.ReadFrom.Configuration(context.Configuration).ReadFrom.Services(provider);
			});

			// Ensure HttpLogging configuration
			builder.Services.AddHttpLogging(opt =>
			{
				opt.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestProperties | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponsePropertiesAndHeaders;
			});

			builder.Services.AddControllersWithViews();
			builder.Services.AddScoped<ICountryRepository, CountryRepository>();
			builder.Services.AddScoped<IPersonRepository, PersonRepository>();
			builder.Services.AddScoped<ICountryService, CountryService>();
			builder.Services.AddScoped<IPersonService, PersonService>();
			builder.Services.AddDbContext<AppDbContext>(opt =>
			{
				opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
			});

			var app = builder.Build();
			if (app.Environment.IsDevelopment())
				app.UseDeveloperExceptionPage();

			// Use HttpLogging
			app.UseHttpLogging();
			app.UseSerilogRequestLogging();
			// Rotativa config
			if (builder.Environment.IsEnvironment("Test") == false)
				RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");

			app.UseStaticFiles();
			app.UseRouting();
			app.MapControllers();

			app.Run();
		}
	}
}
