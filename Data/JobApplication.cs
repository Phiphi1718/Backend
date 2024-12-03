using System;
using System.Collections.Generic;

namespace WebCoffee.Data;

public partial class JobApplication
{
    public int Id { get; set; }

    public string? FullName { get; set; }

    public DateTime? BirthDate { get; set; }

    public string? CitizenId { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Gender { get; set; }

    public string? EducationLevel { get; set; }

    public string? Address { get; set; }

    public string? Position { get; set; }

    public string? Image { get; set; }
}
