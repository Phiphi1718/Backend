using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebCoffee.Data;
using WebCoffee.Models;

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

        // Logic để thêm đơn hàng và chi tiết đơn hàng
        public async Task<DonHang> AddDonHangWithDetailsAsync(DonHang donHang, List<DonHangChiTiet> chiTiets)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Lưu thông tin đơn hàng
                _context.DonHangs.Add(donHang);
                await _context.SaveChangesAsync();

                // Lưu thông tin các chi tiết đơn hàng
                foreach (var chiTiet in chiTiets)
                {
                    chiTiet.DonHangId = donHang.Id; // Liên kết với ID của DonHang
                    _context.DonHangChiTiets.Add(chiTiet);
                }
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return donHang;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }




        // Lấy tất cả đơn hàng
        public async Task<List<DonHang>> GetAllDonHangsAsync()
        {
            return await _context.DonHangs
                .Include(d => d.DonHangChiTiets)  // Bao gồm chi tiết đơn hàng
                .ToListAsync();
        }

        // Lấy đơn hàng theo mã đơn hàng
        public async Task<DonHang?> GetByMaDonHangAsync(string maDonHang)
        {
            return await _context.DonHangs
                .Include(d => d.DonHangChiTiets) // Bao gồm chi tiết đơn hàng
                .FirstOrDefaultAsync(d => d.MaDonHang == maDonHang); // Tìm đơn hàng theo mã đơn hàng
        }


    }
}
