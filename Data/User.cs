using System;
using System.Collections.Generic;

namespace WebCoffee.Data;

public partial class User
{
    public int Id { get; set; }

    public string? FullName { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? PasswordHash { get; set; }

    public int? UserTypeId { get; set; }

    public string? Address { get; set; }

    public string? Cccd { get; set; }

    public DateTime? Dob { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual UserType? UserType { get; set; }
}
