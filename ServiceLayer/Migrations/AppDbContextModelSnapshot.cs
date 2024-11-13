﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ServiceLayer.Context;

#nullable disable

namespace ServiceLayer.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("EntityLayer.Entities.Country", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Countries", (string)null);

                    b.HasData(
                        new
                        {
                            Id = new Guid("14629847-905a-4a0e-9abe-80b61655c5cb"),
                            Name = "Philippines"
                        },
                        new
                        {
                            Id = new Guid("56bf46a4-02b8-4693-a0f5-0a95e2218bdc"),
                            Name = "Thailand"
                        },
                        new
                        {
                            Id = new Guid("12e15727-d369-49a9-8b13-bc22e9362179"),
                            Name = "China"
                        },
                        new
                        {
                            Id = new Guid("8f30bedc-47dd-4286-8950-73d8a68e5d41"),
                            Name = "Palestinian Territory"
                        },
                        new
                        {
                            Id = new Guid("501c6d33-1bbe-45f1-8fbd-2275913c6218"),
                            Name = "China"
                        });
                });

            modelBuilder.Entity("EntityLayer.Entities.Person", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)");

                    b.Property<DateTime?>("BirthDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CountryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Gender")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("Name")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool>("ReceiveNewsLetters")
                        .HasColumnType("bit");

                    b.Property<string>("TIN")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(8)")
                        .HasDefaultValue("ABC12345")
                        .HasColumnName("TaxIdentificationNumber");

                    b.HasKey("Id");

                    b.ToTable("Persons", (string)null);

                    b.HasCheckConstraint("CHK-TIN", "len([TaxIdentificationNumber])=8");

                    b.HasData(
                        new
                        {
                            Id = new Guid("c03bbe45-9aeb-4d24-99e0-4743016ffce9"),
                            Address = "4 Parkside Point",
                            BirthDate = new DateTime(1989, 8, 28, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            CountryId = new Guid("56bf46a4-02b8-4693-a0f5-0a95e2218bdc"),
                            Email = "mwebsdale0@people.com.cn",
                            Gender = "Female",
                            Name = "Marguerite",
                            ReceiveNewsLetters = false
                        },
                        new
                        {
                            Id = new Guid("c3abddbd-cf50-41d2-b6c4-cc7d5a750928"),
                            Address = "6 Morningstar Circle",
                            BirthDate = new DateTime(1990, 10, 5, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            CountryId = new Guid("14629847-905a-4a0e-9abe-80b61655c5cb"),
                            Email = "ushears1@globo.com",
                            Gender = "Female",
                            Name = "Ursa",
                            ReceiveNewsLetters = false
                        },
                        new
                        {
                            Id = new Guid("c6d50a47-f7e6-4482-8be0-4ddfc057fa6e"),
                            Address = "73 Heath Avenue",
                            BirthDate = new DateTime(1995, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            CountryId = new Guid("14629847-905a-4a0e-9abe-80b61655c5cb"),
                            Email = "fbowsher2@howstuffworks.com",
                            Gender = "Male",
                            Name = "Franchot",
                            ReceiveNewsLetters = true
                        },
                        new
                        {
                            Id = new Guid("d15c6d9f-70b4-48c5-afd3-e71261f1a9be"),
                            Address = "83187 Merry Drive",
                            BirthDate = new DateTime(1987, 1, 9, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            CountryId = new Guid("12e15727-d369-49a9-8b13-bc22e9362179"),
                            Email = "asarvar3@dropbox.com",
                            Gender = "Male",
                            Name = "Angie",
                            ReceiveNewsLetters = true
                        },
                        new
                        {
                            Id = new Guid("89e5f445-d89f-4e12-94e0-5ad5b235d704"),
                            Address = "50467 Holy Cross Crossing",
                            BirthDate = new DateTime(1995, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            CountryId = new Guid("56bf46a4-02b8-4693-a0f5-0a95e2218bdc"),
                            Email = "ttregona4@stumbleupon.com",
                            Gender = "Gender",
                            Name = "Tani",
                            ReceiveNewsLetters = false
                        },
                        new
                        {
                            Id = new Guid("2a6d3738-9def-43ac-9279-0310edc7ceca"),
                            Address = "97570 Raven Circle",
                            BirthDate = new DateTime(1988, 1, 4, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            CountryId = new Guid("8f30bedc-47dd-4286-8950-73d8a68e5d41"),
                            Email = "mlingfoot5@netvibes.com",
                            Gender = "Male",
                            Name = "Mitchael",
                            ReceiveNewsLetters = false
                        },
                        new
                        {
                            Id = new Guid("29339209-63f5-492f-8459-754943c74abf"),
                            Address = "57449 Brown Way",
                            BirthDate = new DateTime(1983, 2, 16, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            CountryId = new Guid("12e15727-d369-49a9-8b13-bc22e9362179"),
                            Email = "mjarrell6@wisc.edu",
                            Gender = "Male",
                            Name = "Maddy",
                            ReceiveNewsLetters = true
                        },
                        new
                        {
                            Id = new Guid("ac660a73-b0b7-4340-abc1-a914257a6189"),
                            Address = "4 Stuart Drive",
                            BirthDate = new DateTime(1998, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            CountryId = new Guid("12e15727-d369-49a9-8b13-bc22e9362179"),
                            Email = "pretchford7@virginia.edu",
                            Gender = "Female",
                            Name = "Pegeen",
                            ReceiveNewsLetters = true
                        },
                        new
                        {
                            Id = new Guid("012107df-862f-4f16-ba94-e5c16886f005"),
                            Address = "413 Sachtjen Way",
                            BirthDate = new DateTime(1990, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            CountryId = new Guid("12e15727-d369-49a9-8b13-bc22e9362179"),
                            Email = "hmosco8@tripod.com",
                            Gender = "Male",
                            Name = "Hansiain",
                            ReceiveNewsLetters = true
                        },
                        new
                        {
                            Id = new Guid("cb035f22-e7cf-4907-bd07-91cfee5240f3"),
                            Address = "484 Clarendon Court",
                            BirthDate = new DateTime(1997, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            CountryId = new Guid("8f30bedc-47dd-4286-8950-73d8a68e5d41"),
                            Email = "lwoodwing9@wix.com",
                            Gender = "Male",
                            Name = "Lombard",
                            ReceiveNewsLetters = false
                        },
                        new
                        {
                            Id = new Guid("28d11936-9466-4a4b-b9c5-2f0a8e0cbde9"),
                            Address = "2 Warrior Avenue",
                            BirthDate = new DateTime(1990, 5, 24, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            CountryId = new Guid("501c6d33-1bbe-45f1-8fbd-2275913c6218"),
                            Email = "mconachya@va.gov",
                            Gender = "Female",
                            Name = "Minta",
                            ReceiveNewsLetters = true
                        },
                        new
                        {
                            Id = new Guid("a3b9833b-8a4d-43e9-8690-61e08df81a9a"),
                            Address = "9334 Fremont Street",
                            BirthDate = new DateTime(1987, 1, 19, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            CountryId = new Guid("501c6d33-1bbe-45f1-8fbd-2275913c6218"),
                            Email = "vklussb@nationalgeographic.com",
                            Gender = "Female",
                            Name = "Verene",
                            ReceiveNewsLetters = true
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
