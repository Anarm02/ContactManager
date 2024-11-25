using ContactManager;
using DataLayer.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactManagerTest
{
	public class CustomWebApplicationFactory:WebApplicationFactory<Program>
	{
		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			base.ConfigureWebHost(builder);
			builder.UseEnvironment("Test");
			builder.ConfigureServices(services =>
			{
				var descriptor=services.SingleOrDefault(t => t.ServiceType == typeof(DbContextOptions<AppDbContext>));
				if (descriptor != null)
					services.Remove(descriptor);
				services.AddDbContext<AppDbContext>(opt =>
				{
					opt.UseInMemoryDatabase("TestDB");
				});
			});			
		}
	}
}
