using System;
using System.Collections.Generic;

namespace WebCoffee.Data;

public partial class Payment
{
    public int Id { get; set; }

    public string? PaymentName { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
