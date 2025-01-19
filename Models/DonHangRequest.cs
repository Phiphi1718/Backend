namespace WebCoffee.Models
{
    public class DonHangRequest
    {
        public string MaDonHang { get; set; } = null!;
        public string PhuongThucThanhToan { get; set; } = null!;
        public decimal TongTien { get; set; }

        // Các chi tiết của sản phẩm trong đơn hàng
        public virtual ICollection<DonHangChiTietRequest> DonHangChiTiets { get; set; } = new List<DonHangChiTietRequest>();
    }

    // DTO cho chi tiết sản phẩm
    public class DonHangChiTietRequest
    {
        public string TenProduct { get; set; } = null!;
        public decimal GiaProduct { get; set; }
        public int SoLuongProduct { get; set; }
        public string? SizeProduct { get; set; }
        public string? Topping { get; set; }
    }
}
