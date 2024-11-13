using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.DTOs.Countries
{
	public class CountryAddRequest
	{
        public string Name { get; set; }
		public Country ToCountry()
		{
			return new Country { Name = Name };
		}
    }
}
