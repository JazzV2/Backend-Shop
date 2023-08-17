using System.ComponentModel.DataAnnotations;

namespace backend.Core.Models
{
    public class Product : BaseModel
    {
        [Key]
        public string UrlProduct { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        [Required]
        public double Price { get; set; }
        public int Number { get; set; }
        public string Image { get; set; }

        // Relations
        public ICollection<Order> Orders { get; set; }
    }
}
