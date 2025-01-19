using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using WebCoffee.Data; // Đảm bảo có namespace đúng cho DbContext
using WebCoffee.Models;
using WebCoffee.Repository;
using WebCoffee.ViewModel;

namespace WebCoffee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAllOrigins")] // Áp dụng CORS cho controller này
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailRepository _emailRepository;
        private readonly CoffeeHouseDbContext _context;

        public AuthController(IUserRepository userRepository, CoffeeHouseDbContext context, IEmailRepository emailRepository)
        {
            _userRepository = userRepository;
            _context = context;
            _emailRepository = emailRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            var user = await _userRepository.RegisterUserAsync(model);
            if (user != null)
            {
                return Ok(new { message = "Đăng ký thành công", user });
            }
            return BadRequest(new { message = "Đăng ký không thành công" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest(new { message = "Email và mật khẩu là bắt buộc." });
            }

            // Xác thực người dùng
            var user = await _userRepository.LoginUserAsync(model.Email, model.Password);
            if (user != null)
            {
                // Tạo JWT token cho người dùng
                var token = _userRepository.GenerateJwtToken(user);

                // Lấy userType từ UserTypes collection (nếu có)
                // Giả sử user.UserType không phải là một collection mà là một đối tượng UserType
                var userType = user.UserType?.TypeName ?? "DefaultRole";


                return Ok(new
                {
                    message = "Đăng nhập thành công",

                    token = token,
                    email = user.Email,
                    userType = userType,
                    phone = user.Phone,          // Đảm bảo user đã có giá trị phone
                    fullName = user.FullName,    // Đảm bảo user đã có giá trị fullName
                    address = user.Address,      // Đảm bảo user đã có giá trị address
                    cccd = user.Cccd,            // Đảm bảo user đã có giá trị cccd
                    dob = user.Dob               // Đảm bảo user đã có giá trị dob
                });

            }

            return Unauthorized(new { message = "Đăng nhập không thành công" });
        }


        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Email))
            {
                return BadRequest(new { message = "Email là bắt buộc để cập nhật." });
            }

            // Gọi phương thức trong repository để cập nhật thông tin người dùng
            var updatedUser = await _userRepository.UpdateUserAsync(model.Email, model);

            if (updatedUser == null)
            {
                return NotFound(new { message = "Không tìm thấy người dùng để cập nhật." });
            }

            return Ok(new
            {
                message = "Thông tin người dùng đã được cập nhật thành công.",
                user = new
                {
                    email = updatedUser.Email,
                    fullName = updatedUser.FullName,
                    phone = updatedUser.Phone,
                    address = updatedUser.Address,
                    cccd = updatedUser.Cccd,
                    dob = updatedUser.Dob
                }
            });
        }



        [HttpPost("ForgotPassword")]
        public IActionResult SendNewPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email không hợp lệ.");
            }

            bool result = _emailRepository.SendPasswordToEmail(email);
            if (result)
            {
                return Ok("Mật khẩu mới đã được gửi đến email của bạn.");
            }
            else
            {
                return StatusCode(500, "Có lỗi xảy ra khi gửi email.");
            }
        }

        [HttpPut("ChangePass")]
        public IActionResult ChangePass(string gmails, ChangPassVM changPassVM)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == gmails);
            if (user == null)
            {
                return NotFound("User không tồn tại");
            }

            // Kiểm tra mật khẩu cũ
            if (user.PasswordHash != changPassVM.Password)
            {
                return BadRequest("Mật khẩu cũ không chính xác");
            }

            // Kiểm tra sự khớp của mật khẩu mới
            if (changPassVM.NewPassword != changPassVM.ReNewPassword)
            {
                return BadRequest("Mật khẩu mới không khớp");
            }

            // Cập nhật mật khẩu mới
            user.PasswordHash = changPassVM.NewPassword; // Cập nhật mật khẩu mới không mã hóa
            _context.SaveChanges();

            var email = new EmailModel
            {
                ToEmail = user.Email,
                Subject = "Mật khẩu đã được thay đổi",
                Body = "Mật khẩu của bạn đã được thay đổi thành công.",
            };
            _emailRepository.SendEmail(email);
            return Ok("Đã thay đổi mật khẩu");
        }
        private string GenerateRandomPassword()
        {
            // Hàm tạo mật khẩu ngẫu nhiên (bạn có thể thay đổi logic tùy thích)
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, 8) // Độ dài mật khẩu
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}