using System;
using System.Collections.Generic;

namespace test2.Models;

public partial class Plan
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? Type { get; set; }

    public decimal? Amount { get; set; }

    public int? CategoryId { get; set; }

    public DateOnly? Period { get; set; }

    public string? CategoryName { get; set; }

    public string? UserName { get; set; }

    public virtual Category? Category { get; set; }
}
