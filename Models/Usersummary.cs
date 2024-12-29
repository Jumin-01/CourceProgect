using System;
using System.Collections.Generic;

namespace test2.Models;

public partial class Usersummary
{
    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public decimal? UserBalance { get; set; }

    public long IncomeCount { get; set; }

    public decimal? TotalIncome { get; set; }

    public long ExpenseCount { get; set; }

    public decimal? TotalExpense { get; set; }

    public long CreditCount { get; set; }

    public decimal? TotalCreditAmount { get; set; }

    public decimal? TotalRemainingDebt { get; set; }
}
