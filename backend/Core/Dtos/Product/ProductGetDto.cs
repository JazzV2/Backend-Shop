

namespace backend.Core.Dtos.Product
{
    public class ProductGetDto
    {
        public string UrlProduct { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public double Price { get; set; }
        public int Number { get; set; }
        public string FirstImage { get; set; }
    }
}