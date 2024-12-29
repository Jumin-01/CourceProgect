using System;
using System.Collections.Generic;

namespace test2.Models;

public partial class Credit
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public decimal? RemainingAmount { get; set; }

    public decimal? Amount { get; set; }

    public DateTime? Date { get; set; }

    public string? UserName { get; set; }

    public virtual ICollection<Creditpayment> Creditpayments { get; set; } = new List<Creditpayment>();
}
