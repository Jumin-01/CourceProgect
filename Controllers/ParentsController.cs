using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using test2.Controllers;
using test2.Models;

namespace Home_accounting.Controllers
{
    public class ParentsController(HomeAccountingContext context) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        
        
        #region TransactionControlers
        public IActionResult ListTransaction()
        {
            var userName = User.Identity.Name;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("Login", "Account");
            }

            var modelList = context.Transactions
                .Where(transaction => transaction.UserName == userName)
                .OrderBy(transaction => transaction.Date)
                .ToList();

            return View(modelList);
        }

        public IActionResult DeleteTransaction(int id)
        {
            var person = context.Transactions.Find(id);
            if (person != null)
            {
                context.Transactions.Remove(person);
                context.SaveChanges();
            }
            return RedirectToAction("ListTransaction");
        }

        [HttpGet]
        public IActionResult AddTransaction()
        {
           
            ViewBag.Categories = GetSortedCategories(context.Categories.ToList(), null);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddTransaction(Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                var userName = User.Identity.Name;
                var user = context.Users.FirstOrDefault(u => u.Name == userName);

                if (user == null)
                {
                    ModelState.AddModelError("", "Користувач не знайдений.");
                    return View(transaction);
                }

                transaction.UserId = user.Id;
                transaction.Date = transaction.Date ?? DateTime.Now;

                context.Database.ExecuteSqlRaw(
                    "CALL AddTransaction(@p0, @p1, @p2, @p3, @p4)",
                    parameters: new object[] { transaction.UserId, transaction.CategoryId, transaction.Type, transaction.Amount, transaction.Date?.ToString("yyyy-MM-dd") });

                return RedirectToAction("ListTransaction");
            }
            return View(transaction);
        }

        [HttpGet]
        public IActionResult EditTransaction(int id)
        {
            var transaction = context.Transactions.FirstOrDefault(t => t.Id == id);
            if (transaction == null)
            {
                return NotFound();
            }

            ViewBag.Categories = GetSortedCategories(context.Categories.ToList(), null); 
            return View(transaction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditTransaction(Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                var category = context.Categories.FirstOrDefault(c => c.Id == transaction.CategoryId);
                if (category != null)
                {
                    transaction.CategoryName = category.Name;
                }

                var existingTransaction = context.Transactions.FirstOrDefault(t => t.Id == transaction.Id);
                if (existingTransaction == null)
                {
                    return NotFound();
                }

                existingTransaction.CategoryId = transaction.CategoryId;
                existingTransaction.Type = transaction.Type;
                existingTransaction.Amount = transaction.Amount;
                existingTransaction.Date = transaction.Date;
                existingTransaction.CategoryName = transaction.CategoryName;

                context.Transactions.Update(existingTransaction);
                context.SaveChanges();

                return RedirectToAction("ListTransaction");
            }

            ViewBag.Categories = GetSortedCategories(context.Categories.ToList(), null);
            return View(transaction);
        }
        #endregion
        #region PlanControlers
        public IActionResult DeletePlan(int id)
        {
            var person = context.Plans.Find(id);
            if (person != null)
            {
                context.Plans.Remove(person);
                context.SaveChanges();
            }
            return RedirectToAction("ListTransaction");
        }
        public IActionResult ListPlan()
        {
            var userName = User.Identity.Name;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("Login", "Account");
            }

            var modelList = context.Plans
                .Where(transaction => transaction.UserName == userName)
                .OrderBy(transaction => transaction.Period)
                .ToList();

            return View(modelList);
        }
        [HttpGet]
        public IActionResult AddPlan()
        {
            var categories = context.Categories.ToList();
            ViewBag.Categories = GetSortedCategories(context.Categories.ToList(), null);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddPlan(Plan plan)
        {
            if (ModelState.IsValid)
            {
                var userName = User.Identity.Name;
                var user = context.Users.FirstOrDefault(u => u.Name == userName);

                if (user == null)
                {
                    ModelState.AddModelError("", "Користувач не знайдений.");
                    return View(plan);
                }

                plan.UserId = user.Id;
                plan.Period = plan.Period ?? DateOnly.FromDateTime(DateTime.Now);

                context.Database.ExecuteSqlRaw(
                    "CALL AddPlan(@p0, @p1, @p2, @p3, @p4)",
                    parameters: new object[] { plan.UserId, plan.CategoryId, plan.Type, plan.Amount, plan.Period?.ToString("yyyy-MM-dd") });

                return RedirectToAction("ListPlan");
            }
            return View(plan);
        }
        [HttpGet]
        public IActionResult EditPlan(int id)
        {
            var plan = context.Plans.FirstOrDefault(t => t.Id == id);
            if (plan == null)
            {
                return NotFound();
            }

            ViewBag.Categories = GetSortedCategories(context.Categories.ToList(), null);
            return View(plan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditPlan(Plan plan)
        {
            if (ModelState.IsValid)
            {
                var category = context.Categories.FirstOrDefault(c => c.Id == plan.CategoryId);
                if (category != null)
                {
                    plan.CategoryName = category.Name;
                }

                var existingTransaction = context.Plans.FirstOrDefault(t => t.Id == plan.Id);
                if (existingTransaction == null)
                {
                    return NotFound();
                }

                existingTransaction.CategoryId = plan.CategoryId;
                existingTransaction.Type = plan.Type;
                existingTransaction.Amount = plan.Amount;
                existingTransaction.Period = plan.Period;
                existingTransaction.CategoryName = plan.CategoryName;

                context.Plans.Update(existingTransaction);
                context.SaveChanges();

                return RedirectToAction("ListPlan");
            }

            ViewBag.Categories = GetSortedCategories(context.Categories.ToList(), null);
            return View(plan);
        }
        #endregion
        #region CreditControlers
        public IActionResult ListCredit()
        {
            var userName = User.Identity.Name;
            var user = context.Users.FirstOrDefault(u => u.Name == userName);

            var userId = user.Id;

            if (string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("Login", "Account");
            }

            var creditList = context.Credits
                .Where(credit => credit.UserId == userId)
                .OrderBy(credit => credit.Date)
                .ToList();

            return View(creditList);
        }
        public IActionResult DeleteCredit(int id)
        {
            var credit = context.Credits.Include(c => c.Creditpayments).FirstOrDefault(c => c.Id == id);
            if (credit != null)
            {
                // Видалення всіх платежів, пов'язаних з кредитом
                context.Creditpayments.RemoveRange(credit.Creditpayments);

                // Видалення самого кредиту
                context.Credits.Remove(credit);

                context.SaveChanges();
            }
            return RedirectToAction("ListCredit");
        }
        [HttpGet]
        public IActionResult AddCredit()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddCredit(Credit credit)
        {
            if (ModelState.IsValid)
            {
                var userName = User.Identity.Name;
                var user = context.Users.FirstOrDefault(u => u.Name == userName);
                

                if (user == null)
                {
                    ModelState.AddModelError("", "Користувач не знайдений.");
                    return View(credit);
                }

                credit.UserId = user.Id;
                credit.Date = credit.Date ?? DateTime.Now;

                context.Database.ExecuteSqlRaw(
                    "CALL AddCredit(@p0, @p1, @p2)",
                    parameters: new object[] { credit.UserId, credit.Amount, credit.Date?.ToString("yyyy-MM-dd") });

                return RedirectToAction("ListCredit");
            }

            return View(credit);
        }
        [HttpGet]
        public IActionResult EditCredit(int id)
        {
            var credit = context.Credits.FirstOrDefault(c => c.Id == id);
            if (credit == null)
            {
                return NotFound();
            }

            return View(credit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCredit(Credit credit)
        {
            if (ModelState.IsValid)
            {
                // Отримуємо поточного авторизованого користувача
                var userName = User.Identity.Name;
                var user = context.Users.FirstOrDefault(u => u.Name == userName);

                if (user == null)
                {
                    ModelState.AddModelError("", "Авторизований користувач не знайдений.");
                    return View(credit);
                }

                try
                {
                    // Виклик збереженої процедури UpdateCredit
                    context.Database.ExecuteSqlRaw(
                        "CALL UpdateCredit(@p0, @p1, @p2, @p3, @p4)",
                        parameters: new object[]
                        {
                    credit.Id,
                    user.Id, // UserId береться з авторизованого користувача
                    credit.Amount,
                    credit.RemainingAmount, // Залишкова сума вводиться користувачем
                    credit.Date?.ToString("yyyy-MM-dd") // Дата у форматі MySQL
                        });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Помилка при оновленні кредиту: {ex.Message}");
                    return View(credit);
                }

                return RedirectToAction("ListCredit");
            }

            return View(credit);
        }
        #endregion
        #region PaymentControlers
        [HttpGet]
        [Route("Parents/ListCreditPayment/{creditId}")]
        public IActionResult ListCreditPayment(int creditId)
        {
            var credit = context.Credits.Include(c => c.Creditpayments).FirstOrDefault(c => c.Id == creditId);

            if (credit == null)
            {
                return NotFound();
            }

            var payments = credit.Creditpayments?.OrderBy(payment => payment.Date).ToList() ?? new List<Creditpayment>();

            ViewBag.Credit = credit;
            ViewBag.CreditId = creditId;

            return View(payments);
        }

        [HttpGet]
        public IActionResult AddPayment(int creditId)
        {
            // Передаємо CreditId у вигляд
            ViewBag.CreditId = creditId;

            // Ініціалізуємо нову модель для форми
            return View(new Creditpayment { CreditId = creditId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddPayment(Creditpayment payment)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Виклик збереженої процедури AddCreditPayment
                    context.Database.ExecuteSqlRaw(
                        "CALL AddCreditPayment(@p0, @p1, @p2)",
                        parameters: new object[]
                        {
                            payment.CreditId,
                            payment.Amount,
                            payment.Date ?? DateTime.Now // Дата платежу або поточна дата
                        });

                    return RedirectToAction("ListCreditPayment", new { creditId = payment.CreditId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Помилка при додаванні платежу: {ex.Message}");
                }
            }

            ViewBag.CreditId = payment.CreditId; // Зберігаємо CreditId для повторного використання у вигляді
            return View(payment);
        }

        [HttpGet]
        public IActionResult EditPayment(int id)
        {
            var payment = context.Creditpayments.FirstOrDefault(p => p.Id == id);
            if (payment == null)
            {
                return NotFound();
            }

            ViewBag.CreditId = payment.CreditId; // Передаємо CreditId у вигляд для редагування
            return View(payment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditPayment(Creditpayment payment)
        {
            if (ModelState.IsValid)
            {
                var existingPayment = context.Creditpayments.FirstOrDefault(p => p.Id == payment.Id);
                if (existingPayment == null)
                {
                    return NotFound();
                }

                existingPayment.Amount = payment.Amount;
                existingPayment.Date = payment.Date;

                context.Creditpayments.Update(existingPayment);
                context.SaveChanges();

                return RedirectToAction("ListCreditPayment", new { creditId = payment.CreditId });
            }

            ViewBag.CreditId = payment.CreditId;
            return View(payment);
        }

        public IActionResult DeletePayment(int id)
        {
            var payment = context.Creditpayments.FirstOrDefault(p => p.Id == id);
            if (payment != null)
            {
                var creditId = payment.CreditId; // Зберігаємо CreditId перед видаленням
                context.Creditpayments.Remove(payment);
                context.SaveChanges();
                return RedirectToAction("ListCreditPayment", new { creditId });
            }

            return RedirectToAction("ListPayments");
        }
        #endregion
        #region CategoryControlers

        public IActionResult ListCategory()
        {
            // Отримуємо всі категорії
            var categories = context.Categories
                .Include(c => c.ParentsCategoryNavigation)
                .ToList();

            // Сортуємо категорії рекурсивно
            var sortedCategories = GetSortedCategories(categories, null);

            return View(sortedCategories);
        }

        private List<Category> GetSortedCategories(List<Category> categories, int? parentId)
        {
            var result = new List<Category>();

            // Знаходимо всі категорії, які мають заданого батька (або null для верхнього рівня)
            var parentCategories = categories
                .Where(c => c.ParentsCategory == parentId)
                .OrderBy(c => c.Id)
                .ToList();

            foreach (var category in parentCategories)
            {
                // Додаємо батьківську категорію до результату
                result.Add(category);

                // Рекурсивно додаємо всі підкатегорії
                result.AddRange(GetSortedCategories(categories, category.Id));
            }

            return result;
        }

        [HttpGet]
        public IActionResult AddCategory()
        {
            ViewBag.ParentCategories = GetSortedCategories(context.Categories.ToList(), null);
            return View(new Category());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                context.Categories.Add(category);
                context.SaveChanges();
                return RedirectToAction("ListCategory");
            }

            ViewBag.ParentCategories = GetSortedCategories(context.Categories.ToList(), null);
            return View(category);
        }

        [HttpGet]
        public IActionResult EditCategory(int id)
        {
            var category = context.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            ViewBag.ParentCategories = GetSortedCategories(context.Categories.ToList(), null);
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                var existingCategory = context.Categories.FirstOrDefault(c => c.Id == category.Id);
                if (existingCategory == null)
                {
                    return NotFound();
                }

                existingCategory.Name = category.Name;
                existingCategory.ParentsCategory = category.ParentsCategory;

                context.Categories.Update(existingCategory);
                context.SaveChanges();

                return RedirectToAction("ListCategory");
            }

            ViewBag.ParentCategories = GetSortedCategories(context.Categories.ToList(), null);
            return View(category);
        }

        public IActionResult DeleteCategory(int id)
        {
            var category = context.Categories.FirstOrDefault(c => c.Id == id);
            if (category != null)
            {
                context.Categories.Remove(category);
                context.SaveChanges();
            }
            return RedirectToAction("ListCategory");
        }

        #endregion


        [HttpGet]
        public IActionResult EditUser(string id)
        {
            using (var dbContext = new HomeAccountingContext())
            {
                var user = dbContext.Users.FirstOrDefault(u => u.Name == id);
                if (user == null)
                {
                    return NotFound(); // Повертаємо помилку 404, якщо користувача не знайдено
                }
                ViewBag.Id = user.Id;
                ViewBag.Roles = new List<string> { "Parents", "Children" }; // Список доступних ролей
                return View(user); // Передаємо модель користувача у представлення
            }
        }

        [HttpPost]
        public IActionResult EditUser(User user)
        {
            using (var dbContext = new HomeAccountingContext())
            {
                var existingUser = dbContext.Users.FirstOrDefault(u => u.Id == user.Id);
                if (existingUser == null)
                {
                    return NotFound(); // Користувач не знайдений
                }

                // Оновлення даних користувача
                existingUser.Name = user.Name;
                existingUser.Role = user.Role;
                existingUser.Balance = user.Balance;

                // Перевірка, чи ввели новий пароль
                if (!string.IsNullOrWhiteSpace(user.Password))
                {
                    existingUser.Password = user.Password; // Оновлюємо пароль, якщо він змінений
                }

                dbContext.SaveChanges();

                return RedirectToAction("Index", "Home"); // Перенаправлення на головну сторінку
            }
        }
        [HttpPost]
        public IActionResult DeleteUser(int id)
        {
            using (var dbContext = new HomeAccountingContext())
            {
                // Знаходимо користувача за ID
                var user = dbContext.Users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    return NotFound(); // Користувач не знайдений
                }

                // Видаляємо транзакції користувача
                var transactions = dbContext.Transactions.Where(t => t.UserId == id).ToList();
                dbContext.Transactions.RemoveRange(transactions);

                // Видаляємо плани користувача
                var plans = dbContext.Plans.Where(p => p.UserId == id).ToList();
                dbContext.Plans.RemoveRange(plans);

                // Видаляємо кредити користувача
                var credits = dbContext.Credits.Where(c => c.UserId == id).ToList();

                // Видаляємо платежі за кредити користувача
                var creditIds = credits.Select(c => c.Id).ToList();
                var creditPayments = dbContext.Creditpayments
                    .Where(cp => cp.CreditId.HasValue && creditIds.Contains(cp.CreditId.Value))
                    .ToList();
                dbContext.Creditpayments.RemoveRange(creditPayments);

                // Видаляємо кредити
                dbContext.Credits.RemoveRange(credits);
                dbContext.SaveChanges();
                // Видаляємо користувача
                dbContext.Users.Remove(user);

                // Зберігаємо зміни
                dbContext.SaveChanges();
            }
            LogOff(); // Очищаємо сесію

            // Перенаправляємо на головну сторінку після видалення
            return RedirectToAction("Login", "Home");
        }
        public async Task<IActionResult> LogOff()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
