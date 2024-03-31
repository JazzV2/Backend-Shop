

namespace backend.Core.Dtos.Product
{
    public class ProductByUrlGetDto
    {
        public string UrlProduct { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public double Price { get; set; }
        public int Number { get; set; }
        public List<string> IDImages { get; set; }
        public List<string> Images { get; set; }
    }
}
