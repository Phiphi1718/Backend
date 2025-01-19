using System;
using System.Collections.Generic;

namespace WebCoffee.Data;

public partial class DonHangChiTiet
{
    public int Id { get; set; }

    public string TenProduct { get; set; } = null!;

    public decimal GiaProduct { get; set; }

    public int SoLuongProduct { get; set; }

    public string? SizeProduct { get; set; }

    public string? Topping { get; set; }

    public int DonHangId { get; set; }

    public virtual DonHang DonHang { get; set; } = null!;
}
