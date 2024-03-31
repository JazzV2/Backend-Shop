using System.ComponentModel.DataAnnotations;

namespace backend.Core.Dtos.AuthUser
{
    public class LoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
