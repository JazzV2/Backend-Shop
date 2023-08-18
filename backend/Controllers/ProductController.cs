using AutoMapper;
using backend.Core.Context;
using backend.Core.Dtos.Product;
using backend.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateProduct([FromForm] ProductCreateDto dto, IFormFileCollection formFileCollection)
        {
            int passCount = 0;
            int errorCount = 0;
            string message = null;

            var newProduct = _mapper.Map<Product>(dto);

            try
            {
                var mainPahtProducts = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "Products");

                if (!System.IO.Directory.Exists(mainPahtProducts))
                    System.IO.Directory.CreateDirectory(mainPahtProducts);

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "Products", Guid.NewGuid().ToString() + dto.Name);

                newProduct.ImagesPath = filePath;

                if (!System.IO.Directory.Exists(filePath))
                    System.IO.Directory.CreateDirectory(filePath);

                int countImages = 0;
                

                foreach (var file in formFileCollection)
                {
                    countImages++;
                    var imagePath = Path.Combine(filePath, countImages.ToString() + ".pdf");

                    using (FileStream stream = System.IO.File.Create(imagePath))
                    {
                        await file.CopyToAsync(stream);
                        passCount++;
                    }
                }
                
            }
            catch (Exception ex)
            {
                errorCount++;
                message = "Something gone wrong: " + ex.Message + "\n" + "Photos uploaded: " + passCount.ToString() + " Photos failed: " + errorCount.ToString();
            }

            if (message != null)
                return BadRequest(message);

            await _context.AddAsync(newProduct);
            await _context.SaveChangesAsync();

            return Ok("Product Created Successfully");
        }
    }
}
