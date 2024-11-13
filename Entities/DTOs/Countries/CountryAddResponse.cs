using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.DTOs.Countries
{
    public class CountryAddResponse
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
		public override bool Equals(object? obj)
		{
			if(obj == null) return false;   
            if(obj.GetType() != typeof(CountryAddResponse)) return false;
            CountryAddResponse countryAddResponse = (CountryAddResponse)obj;
            return this.Id==countryAddResponse.Id && this.Name==countryAddResponse.Name;
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
    public static class CountryResponseExtension
    {
        public static CountryAddResponse ToCountryResponse(this Country country)
        {
            return new CountryAddResponse { Id = country.Id, Name = country.Name };
        }
    }
}
