using EntityLayer.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ServiceLayer.Context
{
	public class AppDbContext:DbContext
	{
        public AppDbContext(DbContextOptions opt):base(opt)
        {
            
        }
        public virtual DbSet<Person> Persons { get; set; }
		public virtual DbSet<Country> Countries { get; set; }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			
			//modelBuilder.Entity<Person>(e =>
			//{
			//	e.HasOne<Country>(p=>p.Country).WithMany(c=>c.People).HasForeignKey(p=>p.CountryId);
			//});
			string countriesjson = File.ReadAllText("countries.json");
			var countries=JsonSerializer.Deserialize<List<Country>>(countriesjson);
			foreach(var country in countries)
			{
				modelBuilder.Entity<Country>().HasData(country);
			}
			string personjson = File.ReadAllText("persons.json");
			var people=JsonSerializer.Deserialize<List<Person>>(personjson);
			foreach(var person in people)
			{
				modelBuilder.Entity<Person>().HasData(person);
			}
			modelBuilder.Entity<Country>().ToTable("Countries");
			modelBuilder.Entity<Person>().ToTable("Persons");
			modelBuilder.Entity<Person>().Property(p => p.TIN)
				.HasColumnName("TaxIdentificationNumber")
				.HasColumnType("varchar(8)")
				.HasDefaultValue("ABC12345");
			//modelBuilder.Entity<Person>().HasIndex(p => p.TIN).IsUnique();
			modelBuilder.Entity<Person>().HasCheckConstraint("CHK-TIN", "len([TaxIdentificationNumber])=8");
		}
		public List<Person> sp_GetAllPersons()
		{
			return Persons.FromSqlRaw("Execute [dbo].[GetAllPersons]").ToList();
		}
		public int sp_addPerson(Person person)
		{
			SqlParameter[] parameters = new SqlParameter[] {
				new SqlParameter("@Id",person.Id),
				new SqlParameter("@Name",person.Name),
				new SqlParameter("@Email",person.Email),
				new SqlParameter("@BirthDate",person.BirthDate),
				new SqlParameter("@Gender",person.Gender),
				new SqlParameter("@Address",person.Address),
				new SqlParameter("@CountryId",person.CountryId),
				new SqlParameter("@ReceiveNewsLetters",person.ReceiveNewsLetters),

			};
			return Database.ExecuteSqlRaw("Execute [dbo].[InsertPerson] @Id,@Name,@Email,@BirthDate,@Gender,@Address,@CountryId,@ReceiveNewsLetters",parameters);
		}
		
	}
}
