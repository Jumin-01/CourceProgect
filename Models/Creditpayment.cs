using System;
using System.Collections.Generic;

namespace test2.Models;

public partial class Creditpayment
{
    public int Id { get; set; }

    public int? CreditId { get; set; }

    public decimal? Amount { get; set; }

    public DateTime? Date { get; set; }

    public string? UserName { get; set; }

    public decimal? CreditAmount { get; set; }

    public virtual Credit? Credit { get; set; }
}
