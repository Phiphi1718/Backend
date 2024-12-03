using System;
using System.Collections.Generic;

namespace WebCoffee.Data;

public partial class UserType
{
    public int Id { get; set; }

    public string? MaLoai { get; set; }

    public string? TypeName { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
