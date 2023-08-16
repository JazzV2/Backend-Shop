using System.ComponentModel.DataAnnotations;

namespace backend.Core.Models
{
    public class User : BaseModel
    {
        [Key]
        public string Username { get; set; }
        [Required]
        public string HashPassword { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }

        // Relations
        public ICollection<Order> Orders { get; set; }
    }
}
