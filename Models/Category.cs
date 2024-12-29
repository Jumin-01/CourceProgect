using System;
using System.Collections.Generic;

namespace test2.Models;

public partial class Category
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? ParentsCategory { get; set; }

    public virtual ICollection<Category> InverseParentsCategoryNavigation { get; set; } = new List<Category>();

    public virtual Category? ParentsCategoryNavigation { get; set; }

    public virtual ICollection<Plan> Plans { get; set; } = new List<Plan>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
