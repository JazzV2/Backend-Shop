using System.ComponentModel.DataAnnotations;

namespace backend.Core.Models
{
    public class Order : BaseModel
    {
        [Key]
        public string UrlOrder { get; set; } = Guid.NewGuid().ToString();
        public double TotalPrice { get; set; }
        public int OrderedNumber { get; set; }

        // Relations
        public string Username { get; set; }
        public User User { get; set; }
        public string UrlProduct { get; set; }
        public Product Product { get; set; }
    }
}
