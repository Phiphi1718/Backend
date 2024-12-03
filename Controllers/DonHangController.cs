using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebCoffee.Data;
using WebCoffee.Models;
using WebCoffee.Repository;

namespace WebCoffee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonHangController : ControllerBase
    {
        private readonly IDonHangRepository _donHangRepository;

        public DonHangController(IDonHangRepository donHangRepository)
        {
            _donHangRepository = donHangRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateDonHangWithDetails([FromBody] DonHangRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Tạo đối tượng DonHang
            var donHang = new DonHang
            {
                MaDonHang = request.MaDonHang,
                PhuongThucThanhToan = request.PhuongThucThanhToan,
                TongTien = request.TongTien,
            };

            // Chuyển đổi chi tiết đơn hàng từ request
            var chiTiets = request.DonHangChiTiets.Select(ct => new DonHangChiTiet
            {
                TenProduct = ct.TenProduct,
                GiaProduct = ct.GiaProduct,
                SoLuongProduct = ct.SoLuongProduct,
                SizeProduct = ct.SizeProduct,
                Topping = ct.Topping
            }).ToList();

            // Lưu vào cơ sở dữ liệu
            var createdDonHang = await _donHangRepository.AddDonHangWithDetailsAsync(donHang, chiTiets);

            return CreatedAtAction(nameof(GetByMaDonHang), new { maDonHang = createdDonHang.MaDonHang }, createdDonHang);

        }



        // Lấy tất cả đơn hàng
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var donHangs = await _donHangRepository.GetAllDonHangsAsync();
            if (donHangs != null)
                return Ok(donHangs); // Trả về danh sách đơn hàng
            else
                return StatusCode(500, "Có lỗi khi lấy danh sách đơn hàng");
        }

        [HttpGet("{maDonHang}")]
        public async Task<IActionResult> GetByMaDonHang(string maDonHang)
        {
            var donHang = await _donHangRepository.GetByMaDonHangAsync(maDonHang); // Gọi Repository để lấy dữ liệu
            if (donHang == null) return NotFound(); // Nếu không tìm thấy, trả về 404
            return Ok(donHang); // Trả về dữ liệu
        }


    }
}
