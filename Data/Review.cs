using System;
using System.Collections.Generic;

namespace WebCoffee.Data;

public partial class Review
{
    public int Id { get; set; }

    public int? OrderId { get; set; }

    public int? UserId { get; set; }

    public int? ProductId { get; set; }

    public string? Comment { get; set; }

    public int? Rating { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Product? Product { get; set; }

    public virtual User? User { get; set; }
}
