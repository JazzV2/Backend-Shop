using AutoMapper;
using backend.Core.Context;
using backend.Core.Dtos.Product;
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

       /* [HttpPost]
        [Route("Create")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateProduct([FromForm] ProductCreateDto dto, IFormFile formFile)
        {
            try
            {

            }
        }*/
    }
}
