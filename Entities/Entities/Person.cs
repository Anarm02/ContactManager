using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
	public class Person
	{
        [Key]
        public Guid Id { get; set; }
        [StringLength(50)]
        public string? Name { get; set; }
        [StringLength(100)]
        public string? Email { get; set; }
        public DateTime? BirthDate { get; set; }
        [StringLength (10)]
        public string? Gender { get; set; }
        [StringLength(300)]
        public string? Address { get; set; }
        public Guid? CountryId { get; set; }
        public bool ReceiveNewsLetters { get; set; }
        public string? TIN { get; set; }
        [ForeignKey("CountryId")]
        public Country Country { get; set; }
    }
}
