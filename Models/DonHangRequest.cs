using WebCoffee.Data;

namespace WebCoffee.Models
{
    public class DonHangRequest
    {
        public string MaDonHang { get; set; } = null!;

        public string PhuongThucThanhToan { get; set; } = null!;

        public decimal TongTien { get; set; }

        public virtual ICollection<DonHangChiTiet> DonHangChiTiets { get; set; } = new List<DonHangChiTiet>();
    }

    public class DonHangChiTietRequest
    {
        public string TenProduct { get; set; } = null!;

        public decimal GiaProduct { get; set; }

        public int SoLuongProduct { get; set; }

        public string? SizeProduct { get; set; }

        public string? Topping { get; set; }

        public int? DonHangId { get; set; }

    }
    public class donHangChiTiet: DonHangChiTietRequest
    {
        public virtual DonHang? DonHang { get; set; }
    }


}
