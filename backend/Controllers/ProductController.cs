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
            // Getting link for our image
            string hostUrl = Request.Scheme + "://" + Request.Host + Request.PathBase;
            // Mapping our new Product
            var newProduct = _mapper.Map<Product>(dto);

            // Path where directories with photos are stored
            var mainPahtProducts = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "Products");

            // New directory's name for single product images
            var newGuid = Guid.NewGuid().ToString();
            var directoryOfPhotos = newGuid + dto.Name;

            // Path for new directory contains images
            var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "Products", directoryOfPhotos);

            // Updating new model product (hostUrl and ImagesPath)
            newProduct.ImagesPath = hostUrl + "/Upload/Products/" + directoryOfPhotos;
            newProduct.UrlProduct = newGuid;

            try
            {
                if (!Directory.Exists(mainPahtProducts))
                    Directory.CreateDirectory(mainPahtProducts);

                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                int countImages = 0;
                
                foreach (var file in formFileCollection)
                {
                    countImages++;
                    var extension = Path.GetExtension(file.FileName);

                    if (!(extension == ".jpg" || extension == ".png"))
                        throw new InvalidFileTypeException(file.FileName, ".jpg, .png");

                    var imagePath = Path.Combine(directoryPath, countImages.ToString() + extension);

                    using (FileStream stream = System.IO.File.Create(imagePath))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
            }
            catch (Exception ex)
            {
                if (Directory.Exists(directoryPath))
                    Directory.Delete(directoryPath, true);

                return BadRequest(ex.Message);
            }

            await _context.AddAsync(newProduct);
            await _context.SaveChangesAsync();

            return Ok("Product Created Successfully");
        }

        [HttpGet]
        [Route("Get")]
        public async Task<ActionResult<IEnumerable<ProductGetDto>>> GetProducts()
        {
            var products = await _context.Products.ToListAsync();
            var convertedProducts = _mapper.Map<IEnumerable<ProductGetDto>>(products);

            return Ok(convertedProducts);
        }

        [HttpGet]
        [Route("Get{url}")]
        public async Task<ActionResult<ProductByUrlGetDto>> GetProduct([FromRoute] string url)
        {
            var product = await _context.Products.FirstOrDefaultAsync(product => product.UrlProduct == url);
            var convertedProduct = _mapper.Map<ProductByUrlGetDto>(product);

            if (product is null)
                return NotFound("Couldn't find the product");

            return Ok(convertedProduct);
        }

       /* [HttpPatch]
        [Route("Update{url}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct([FromBody] ProductCreateDto dto)*/
    }
}
