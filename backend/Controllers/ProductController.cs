using AutoMapper;
using backend.Core.Context;
using backend.Core.Dtos.Product;
using backend.Core.Models;
using backend.CustomExceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProductController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("Create")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateProduct([FromForm] ProductCreateDto dto, IFormFileCollection formFileCollection)
        {
            var newProduct = _mapper.Map<Product>(dto);
            string newUrlProduct = Guid.NewGuid().ToString();
            newProduct.UrlProduct = newUrlProduct;

            await _context.AddAsync(newProduct);
            await _context.SaveChangesAsync();

            try
            {
                int countImages = 0;

                foreach (var file in formFileCollection)
                {
                    string extension = Path.GetExtension(file.FileName);

                    if (!(extension == ".jpg" || extension == ".png"))
                        throw new InvalidFileTypeException(file.FileName, ".jpg, .png");

                    countImages++;

                    using(MemoryStream stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);
                        Image newImage = new Image
                        {
                            Name = countImages.ToString(),
                            FileType = extension,
                            ProductImage = stream.ToArray(),
                            UrlProduct = newUrlProduct
                        };

                        await _context.AddAsync(newImage);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok("Product Created Successfully");
        }

        [HttpGet]
        [Route("Get")]
        public async Task<ActionResult<IEnumerable<ProductGetDto>>> GetProducts()
        {
            var products = await _context.Products.Include(product => product.Images).ToListAsync();
            var convertedProducts = _mapper.Map<IEnumerable<ProductGetDto>>(products);

            return Ok(convertedProducts);
        }

        [HttpGet]
        [Route("Get{url}")]
        public async Task<ActionResult<ProductByUrlGetDto>> GetProduct([FromRoute] string url)
        {
            var product = await _context.Products.Include(product => product.Images).FirstOrDefaultAsync(product => product.UrlProduct == url);
            var convertedProduct = _mapper.Map<ProductByUrlGetDto>(product);

            if (product is null)
                return NotFound("Couldn't find the product");

            var listImages = product.Images.ToList();

            listImages.ForEach(image =>
            {
                convertedProduct.Images.Add(Convert.ToBase64String(image.ProductImage));
            });

            return Ok(convertedProduct);
        }

        /* [HttpPatch]
         [Route("Update{url}")]
         [Authorize(Roles = "Admin")]
         public async Task<IActionResult> UpdateProduct([FromBody] ProductCreateDto dto)*/
    }
}
