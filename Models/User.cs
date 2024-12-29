using System;
using System.Collections.Generic;

namespace test2.Models;

public partial class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Password { get; set; }

    public string? Role { get; set; }

    public decimal? Balance { get; set; }
}
