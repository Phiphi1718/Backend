using System;
using System.Collections.Generic;

namespace WebCoffee.Data;

public partial class Store
{
    public int Id { get; set; }

    public string Address { get; set; } = null!;

    public string Ward { get; set; } = null!;

    public string District { get; set; } = null!;

    public string Province { get; set; } = null!;
}
