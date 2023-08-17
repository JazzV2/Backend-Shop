using AutoMapper;
using backend.Core.Context;
using backend.Core.Dtos.AuthUser;
using backend.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthUserController : ControllerBase
    {
        private IMapper _mapper;
        private ApplicationDbContext _context;
        private IConfiguration _configuration;

        public AuthUserController(IMapper mapper, ApplicationDbContext context, IConfiguration configuration)
        {
            _mapper = mapper;
            _context = context;
            _configuration = configuration;
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

        [HttpPost]
        [Route("RegisterAdmin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterDto dto)
        {
            if (dto.Username == null && dto.Password == null && dto.FirstName == null && dto.LastName == null && dto.Email == null)
                return BadRequest("All fields must be field");

            var user = await _context.Users.FirstOrDefaultAsync(user => user.Username == dto.Username);

            if (user != null)
                return BadRequest("This Username already exists");

            var newUser = _mapper.Map<User>(dto);

            newUser.Role = "Admin";

            await _context.AddAsync(newUser);
            await _context.SaveChangesAsync();

            return Ok("User was created successfully");
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(user => user.Username == dto.Username);

            if (user == null)
                return NotFound("User not found");

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.HashPassword))
                return Unauthorized("Bad password");

            var token = CreateToken(user);

            return Ok(token);
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("JWTID", Guid.NewGuid().ToString())
            };

            var authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));

            var tokenObject = new JwtSecurityToken(
                    issuer: _configuration["Jwt:ValidIssuer"],
                    audience: _configuration["Jwt:ValidAudience"],
                    expires: DateTime.Now.AddHours(1),
                    claims: claims,
                    signingCredentials: new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256)
                );

            string token = new JwtSecurityTokenHandler().WriteToken(tokenObject);

            return token;
        }

    }
}
