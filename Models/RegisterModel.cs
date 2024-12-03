using System.ComponentModel.DataAnnotations;

namespace WebCoffee.Models
{
    public class RegisterModel
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Password { get; set; } = null!; // Mật khẩu đầu vào\
    }
}
