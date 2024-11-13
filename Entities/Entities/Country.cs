using System.ComponentModel.DataAnnotations;

namespace EntityLayer.Entities
{
    public class Country
    {
        [Key]
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public virtual ICollection<Person>? People { get; set; }
    }
}
