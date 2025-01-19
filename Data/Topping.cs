﻿using System;
using System.Collections.Generic;

namespace WebCoffee.Data;

public partial class Topping
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public decimal? Price { get; set; }

    public string? Description { get; set; }

    public string? Img { get; set; }
}
