using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using test2.Models;

namespace Home_accounting.Controllers
{
    public class ChartController(HomeAccountingContext context) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ListChart()
        {
            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("Login", "Account");
            }

            // Визначаємо перший і останній день місяця
            DateTime firstDayOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            // Отримуємо транзакції за останній місяць
            var transactions = context.Transactions
                .Where(t => t.UserName == userName && t.Date >= firstDayOfMonth && t.Date <= lastDayOfMonth)
                .GroupBy(t => t.Date.Value.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Income = g.Where(t => t.Type == "income").Sum(t => t.Amount) ?? 0, // Доходи
                    Expense = g.Where(t => t.Type == "expense").Sum(t => t.Amount) ?? 0 // Витрати
                })
                .ToDictionary(g => g.Date);

            // Формуємо повний список днів місяця
            var dailyData = Enumerable.Range(0, (lastDayOfMonth - firstDayOfMonth).Days + 1)
                .Select(offset => firstDayOfMonth.AddDays(offset))
                .Select(date => new
                {
                    Date = date,
                    Income = transactions.ContainsKey(date) ? transactions[date].Income : 0,
                    Expense = transactions.ContainsKey(date) ? transactions[date].Expense : 0
                })
                .ToList();

            // Групуємо доходи по категоріях
            var incomeData = context.Transactions
                .Where(t => t.UserName == userName && t.Date >= firstDayOfMonth && t.Date <= lastDayOfMonth && t.Type == "income")
                .GroupBy(t => t.Category.Name)
                .Select(g => new
                {
                    Category = g.Key,
                    Total = g.Sum(t => t.Amount) ?? 0
                })
                .ToList();

            // Групуємо витрати по категоріях
            var expenseData = context.Transactions
                .Where(t => t.UserName == userName && t.Date >= firstDayOfMonth && t.Date <= lastDayOfMonth && t.Type == "expense")
                .GroupBy(t => t.Category.Name)
                .Select(g => new
                {
                    Category = g.Key,
                    Total = g.Sum(t => t.Amount) ?? 0
                })
                .ToList();

            // Передаємо дані для лінійного графіка
            ViewBag.Dates = dailyData.Select(d => d.Date.ToString("yyyy-MM-dd")).ToArray();
            ViewBag.Income = dailyData.Select(d => d.Income).ToArray();
            ViewBag.Expenses = dailyData.Select(d => d.Expense).ToArray();

            // Передаємо дані для кругових графіків
            ViewBag.IncomeLabels = incomeData.Select(i => i.Category).ToArray();
            ViewBag.IncomeData = incomeData.Select(i => i.Total).ToArray();
            ViewBag.ExpenseLabels = expenseData.Select(e => e.Category).ToArray();
            ViewBag.ExpenseData = expenseData.Select(e => e.Total).ToArray();

            return View();
        }
        [HttpGet]
        public IActionResult GetChartData(int month, int year)
        {
            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized();
            }

            // Визначаємо перший і останній день місяця
            DateTime firstDayOfMonth = new DateTime(year, month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            // Отримуємо транзакції за вказаний місяць
            var transactions = context.Transactions
                .Where(t => t.UserName == userName && t.Date >= firstDayOfMonth && t.Date <= lastDayOfMonth)
                .GroupBy(t => t.Date.Value.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Income = g.Where(t => t.Type == "income").Sum(t => t.Amount) ?? 0,
                    Expense = g.Where(t => t.Type == "expense").Sum(t => t.Amount) ?? 0
                })
                .ToDictionary(g => g.Date);

            // Формуємо повний список днів місяця
            var dailyData = Enumerable.Range(0, (lastDayOfMonth - firstDayOfMonth).Days + 1)
                .Select(offset => firstDayOfMonth.AddDays(offset))
                .Select(date => new
                {
                    Date = date.ToString("yyyy-MM-dd"),
                    Income = transactions.ContainsKey(date) ? transactions[date].Income : 0,
                    Expense = transactions.ContainsKey(date) ? transactions[date].Expense : 0
                })
                .ToList();

            return Json(new
            {
                dates = dailyData.Select(d => d.Date).ToArray(),
                income = dailyData.Select(d => d.Income).ToArray(),
                expenses = dailyData.Select(d => d.Expense).ToArray()
            });
        }
        [HttpGet]
        public IActionResult GetCategoryData(int month, int year)
        {
            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized();
            }

            DateTime firstDayOfMonth = new DateTime(year, month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            // Доходи по категоріях
            var incomeData = context.Transactions
                .Where(t => t.UserName == userName && t.Date >= firstDayOfMonth && t.Date <= lastDayOfMonth && t.Type == "income")
                .GroupBy(t => t.Category.Name)
                .Select(g => new
                {
                    Category = g.Key,
                    Total = g.Sum(t => t.Amount) ?? 0
                })
                .ToList();

            // Витрати по категоріях
            var expenseData = context.Transactions
                .Where(t => t.UserName == userName && t.Date >= firstDayOfMonth && t.Date <= lastDayOfMonth && t.Type == "expense")
                .GroupBy(t => t.Category.Name)
                .Select(g => new
                {
                    Category = g.Key,
                    Total = g.Sum(t => t.Amount) ?? 0
                })
                .ToList();

            return Json(new
            {
                incomeLabels = incomeData.Select(i => i.Category).ToArray(),
                incomeData = incomeData.Select(i => i.Total).ToArray(),
                expenseLabels = expenseData.Select(e => e.Category).ToArray(),
                expenseData = expenseData.Select(e => e.Total).ToArray()
            });
        }
    }
}
