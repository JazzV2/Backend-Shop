using AutoMapper;
using backend.Core.Context;
using backend.Core.Dtos.AuthUser;
using backend.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthUserController : ControllerBase
    {
        private IMapper _mapper;
        private ApplicationDbContext _context;

        public AuthUserController(IMapper mapper, ApplicationDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterDto dto)
        {
            if (dto.Username == null && dto.Password == null && dto.FirstName == null && dto.LastName == null && dto.Email == null)
                return BadRequest("All fields must be field");

            var user = await _context.Users.FirstOrDefaultAsync(user => user.Username == dto.Username);

            if (user != null)
                return BadRequest("This Username already exists");
            
            var newUser = _mapper.Map<User>(dto);

            newUser.Role = "User";

            await _context.AddAsync(newUser);
            await _context.SaveChangesAsync();

            return Ok("User was created successfully");
        }
    }
}
