using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Core.Models
{
    public class Image : BaseModel
    {
        [Key]
        public string ID { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string FileType { get; set; }
        [Column(TypeName = "image")]
        public byte[] ProductImage { get; set; }
        public bool IsMain { get; set; }

        // Relations
        public string UrlProduct { get; set; }
        public Product Product { get; set; }
    }
}
