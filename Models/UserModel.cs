using WebCoffee.Data;

namespace WebCoffee.Models
{
    public class UserModel

    {
        public int Id { get; set; }
        public string? FullName { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? PasswordHash { get; set; }
        public string? Address { get; set; }
        public string? Cccd { get; set; }
        public DateTime? Dob { get; set; }

        public int? UserTypeId { get; set; } = 2; // Mặc định là khách hàng
        public virtual UserType UserType { get; set; }
    }
    public class UserVM : UserModel
    {
        public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();


        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();


    }
    public class Users
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string MatKhau { get; set; } = null!;
    }


    public class UpdateUserModel
    {
        public string Email { get; set; } // Email dùng để xác định người dùng
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? Cccd { get; set; }
        public DateTime? Dob { get; set; }
    }


}