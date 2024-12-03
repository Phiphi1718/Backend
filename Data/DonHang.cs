using System;
using System.Collections.Generic;

namespace WebCoffee.Data;

public partial class DonHang
{
    public int Id { get; set; }

    public string MaDonHang { get; set; } = null!;

    public string PhuongThucThanhToan { get; set; } = null!;

    public decimal TongTien { get; set; }

    public virtual ICollection<DonHangChiTiet> DonHangChiTiets { get; set; } = new List<DonHangChiTiet>();
}
