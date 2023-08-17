using System.ComponentModel.DataAnnotations;

namespace backend.Core.Dtos.Product
{
    public class ProductCreateDto
    {
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public double Price { get; set; }
        public int Number { get; set; }
    }
}
