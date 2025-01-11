using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using DataLayer.Context;
using DataLayer.Interfaces;
using DataLayer.Services;
using DataLayer.Repositories;
using Serilog;
using ContactManager.Filters.ActionFilters;
using ContactManager.StartupExtensions;
using ContactManager.Middlewares;
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
			builder.Services.ConfigureServices(builder.Configuration);

			var app = builder.Build();
			if (app.Environment.IsDevelopment())
			{

				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				app.UseExceptionHandlingMiddleware();
			}
			app.UseHsts();
			app.UseHttpsRedirection();
			// Use HttpLogging
			app.UseHttpLogging();
			app.UseSerilogRequestLogging();
			// Rotativa config
			if (builder.Environment.IsEnvironment("Test") == false)
				RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");

			app.UseStaticFiles();
			app.UseRouting();
			app.UseAuthentication();
			app.UseAuthorization();
			app.MapControllers();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name:"areas",
					pattern:"{area}/{controller}/{action}/{id?}"
					);
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller}/{action}/{id?}"
					);
				
			}
			);

			app.Run();
		}
	}
}
