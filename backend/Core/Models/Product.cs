using System.ComponentModel.DataAnnotations;

namespace backend.Core.Models
{
    public class Product : BaseModel
    {
        [Key]
        public string UrlProduct { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        [Required]
        public double Price { get; set; }
        public int Number { get; set; }

        // Relations
        public ICollection<Order> Orders { get; set; }
        public ICollection<Image> Images { get; set; }
    }
}
