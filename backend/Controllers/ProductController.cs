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

            try
            {
                int counter = 0;
                bool isMainImage = false;

                foreach (var file in formFileCollection)
                {
                    string extension = Path.GetExtension(file.FileName);

                    if (!(extension == ".jpg" || extension == ".png"))
                        throw new InvalidFileTypeException(file.FileName, ".jpg, .png");

                    using(MemoryStream stream = new MemoryStream())
                    {
                        if (counter == 0)
                            isMainImage = true;
                        else
                            isMainImage = false;

                        counter++;

                        await file.CopyToAsync(stream);
                        Image newImage = new Image
                        {
                            Name = file.Name,
                            FileType = extension,
                            ProductImage = stream.ToArray(),
                            UrlProduct = newUrlProduct,
                            IsMain = isMainImage
                        };

                        await _context.AddAsync(newImage);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            await _context.SaveChangesAsync();
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

            listImages.OrderBy(image => image.IsMain);

            listImages.ForEach(image =>
            {
                convertedProduct.Images.Add(Convert.ToBase64String(image.ProductImage));
                convertedProduct.IDImages.Add(image.ID);
            });



            return Ok(convertedProduct);
        }

        [HttpPatch]
        [Route("UpdateProductText{url}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProductText([FromBody] ProductCreateDto dto, [FromRoute] string url)
        {
            var product = await _context.Products.FirstOrDefaultAsync(item => item.UrlProduct == url);

            if (product is null)
                return NotFound("Couldn't find the product");

            product.UpdatedAt = DateTime.Now;

            var patchedProduct = _mapper.Map(dto, product);

            await _context.SaveChangesAsync();


            return Ok("Product patched successfully");
        }

        [HttpPut]
        [Route("AddNewImages{url}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddNewImagesToProduct([FromRoute] string urlProduct, [FromForm] IFormFileCollection formFileCollection)
        {
            try
            {
                foreach (var file in formFileCollection)
                {
                    string extension = Path.GetExtension(file.FileName);

                    if (!(extension == ".jpg" || extension == ".png"))
                        throw new InvalidFileTypeException(file.FileName, ".jpg, .png");

                    using (MemoryStream stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);
                        Image newImage = new Image
                        {
                            Name = file.Name,
                            FileType = extension,
                            ProductImage = stream.ToArray(),
                            UrlProduct = urlProduct,
                            IsMain = false
                        };

                        await _context.AddAsync(newImage);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            await _context.SaveChangesAsync();

            return Ok("Images was added successfully");
        }

        [HttpDelete]
        [Route("DeleteProductImages{ID}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteImages([FromRoute] string ID)
        {
            var image = await _context.Images.FirstOrDefaultAsync(image => image.ID == ID);

            if (image is null)
                return NotFound("Couldn't find the image");

            _context.Images.Remove(image);
            await _context.SaveChangesAsync();

            return Ok("Image was deleted successfully");
        }

        [HttpDelete]
        [Route("DeleteProduct{url}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([FromRoute] string url)
        {
            var product = await _context.Products.Include(product => product.Images).FirstOrDefaultAsync(product => product.UrlProduct == url);

            if (product is null)
                return NotFound("Couldn't find the product");

            var images = product.Images.ToList();

            images.ForEach(async image =>
            {
                _context.Images.Remove(image);
            });

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok("Product deleted successfully");
        }
    }
}
