using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebCoffee.Data;
using WebCoffee.Models;

namespace WebCoffee.Repository
{
    public interface IUserRepository
    {
        Task<UserModel> RegisterUserAsync(RegisterModel model);
        Task<UserModel?> LoginUserAsync(string email, string password);
        Task<UserModel?> UpdateUserAsync(string email, UpdateUserModel model);
        string GenerateJwtToken(UserModel user);
    }

    public class UserRepository : IUserRepository
    {
        private readonly CoffeeHouseDbContext _context;
        private readonly IConfiguration _configuration;

        public UserRepository(CoffeeHouseDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // Đăng ký tài khoản
        public async Task<UserModel> RegisterUserAsync(RegisterModel model)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == model.Email);

            if (existingUser != null)
            {
                throw new Exception("Email đã tồn tại. Vui lòng chọn một email khác.");
            }

            // Tạo đối tượng User từ RegisterModel
            var userEntity = new WebCoffee.Data.User
            {
                FullName = model.FullName,
                Email = model.Email,
                Phone = model.Phone,
                PasswordHash = model.Password, // Lưu trực tiếp (không khuyến khích)
                UserTypeId = 2 // Mặc định là khách hàng
            };

            await _context.Users.AddAsync(userEntity);
            await _context.SaveChangesAsync();

            // Chuyển đổi lại từ WebCoffee.Data.User sang UserModel để trả về
            return new UserModel
            {
                Id = userEntity.Id,
                FullName = userEntity.FullName,
                Email = userEntity.Email,
                Phone = userEntity.Phone,
                PasswordHash = userEntity.PasswordHash,
                UserTypeId = userEntity.UserTypeId
            };
        }


        // Đăng nhập
        public async Task<UserModel?> LoginUserAsync(string email, string password)
        {
            var userEntity = await _context.Users
     .Include(u => u.UserType) // Nạp thông tin UserType
     .FirstOrDefaultAsync(u => u.Email == email);

            Console.WriteLine($"Address: {userEntity?.Address}, Cccd: {userEntity?.Cccd}, Dob: {userEntity?.Dob}");

            if (userEntity == null || userEntity.PasswordHash != password)
            {
                return null; // Email hoặc mật khẩu không đúng
            }


            // Chuyển đổi từ WebCoffee.Data.User sang UserModel
            return new UserModel
            {
                Id = userEntity.Id,
                FullName = userEntity.FullName,
                Email = userEntity.Email,
                Phone = userEntity.Phone,
                PasswordHash = userEntity.PasswordHash,
                Address = userEntity.Address, // Kiểm tra lại việc ánh xạ từ userEntity sang UserModel
                Cccd = userEntity.Cccd,     // Kiểm tra lại việc ánh xạ từ userEntity sang UserModel
                Dob = userEntity.Dob,       // Kiểm tra lại việc ánh xạ từ userEntity sang UserModel

                UserType = userEntity.UserType // Ánh xạ UserType nếu cần
            };

        }


        // Tạo JWT Token
        public string GenerateJwtToken(UserModel user)
        {
            var userType = user.UserType?.TypeName ?? "Customer"; // Lấy tên loại tài khoản

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, userType)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async Task<UserModel?> UpdateUserAsync(string email, UpdateUserModel model)
        {
            // Tìm người dùng theo email
            var userEntity = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (userEntity == null)
            {
                return null; // Không tìm thấy người dùng
            }

            // Cập nhật các thông tin người dùng
            userEntity.FullName = model.FullName ?? userEntity.FullName;
            userEntity.Address = model.Address ?? userEntity.Address;
            userEntity.Cccd = model.Cccd ?? userEntity.Cccd;
            userEntity.Dob = model.Dob ?? userEntity.Dob;


            // Lưu thay đổi vào cơ sở dữ liệu
            _context.Users.Update(userEntity);
            await _context.SaveChangesAsync();

            // Trả về thông tin đã cập nhật
            return new UserModel
            {
                Id = userEntity.Id,
                FullName = userEntity.FullName,
                Email = userEntity.Email,
                Phone = userEntity.Phone,
                Address = userEntity.Address,
                Cccd = userEntity.Cccd,
                Dob = userEntity.Dob
            };
        }

    }
}
