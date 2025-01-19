using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebCoffee.Data;
using Microsoft.Extensions.Logging;

namespace WebCoffee.Repository
{
    public interface IDonHangRepository
    {
        Task<DonHang> AddDonHangWithDetailsAsync(DonHang donHang, List<DonHangChiTiet> chiTiets);
        Task<List<DonHang>> GetAllDonHangsAsync();
        Task<DonHang?> GetByMaDonHangAsync(string maDonHang);
    }

    public class DonHangRepository : IDonHangRepository
    {
        private readonly CoffeeHouseDbContext _context;
        private readonly ILogger<DonHangRepository> _logger;

        public DonHangRepository(CoffeeHouseDbContext context, ILogger<DonHangRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Thêm đơn hàng và chi tiết đơn hàng
        public async Task<DonHang> AddDonHangWithDetailsAsync(DonHang donHang, List<DonHangChiTiet> chiTiets)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Thêm đơn hàng vào cơ sở dữ liệu
                _context.DonHangs.Add(donHang);
                await _context.SaveChangesAsync();  // Lưu đơn hàng và lấy Id tự động

                // Gán DonHangId cho các chi tiết đơn hàng
                foreach (var chiTiet in chiTiets)
                {
                    chiTiet.DonHangId = donHang.Id; // Liên kết chi tiết với đơn hàng vừa tạo
                    _context.DonHangChiTiets.Add(chiTiet); // Thêm chi tiết vào cơ sở dữ liệu
                }
                await _context.SaveChangesAsync();  // Lưu chi tiết đơn hàng

                // Commit transaction
                await transaction.CommitAsync();

                return donHang; // Trả về đơn hàng đã được tạo
            }
            catch (Exception ex)
            {
                // Rollback transaction nếu có lỗi
                await transaction.RollbackAsync();
                _logger.LogError("Error while saving order and details: " + ex.Message);
                throw;
            }
        }

        // Lấy tất cả đơn hàng và chi tiết đơn hàng
        public async Task<List<DonHang>> GetAllDonHangsAsync()
        {
            return await _context.DonHangs
                .Include(d => d.DonHangChiTiets)  // Bao gồm chi tiết đơn hàng
                .ToListAsync(); // Trả về danh sách đơn hàng
        }

        // Lấy đơn hàng theo mã đơn hàng
        public async Task<DonHang?> GetByMaDonHangAsync(string maDonHang)
        {
            return await _context.DonHangs
                .Include(d => d.DonHangChiTiets)  // Bao gồm chi tiết đơn hàng
                .FirstOrDefaultAsync(d => d.MaDonHang == maDonHang); // Tìm đơn hàng theo mã
        }
    }
}
