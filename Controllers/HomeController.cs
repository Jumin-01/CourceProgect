using System.Diagnostics;
using System.Security.Claims;
using Home_accounting.DataTransferObjects;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using test2.Models;

namespace test2.Controllers
{
    public class HomeController(HomeAccountingContext context) : Controller
    {
        private readonly ILogger<HomeController> _logger;

        

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Login()
        {
            var userDto = new UserDto { ReturnUrl = HttpContext.Request.Query["ReturnUrl"].ToString() };
            ViewBag.RegistrationUrl = Url.Action("CreateUser", "Home"); // Генеруємо URL для сторінки реєстрації
            return View(userDto);
        }
        public async Task<IActionResult> LoginPost(UserDto userDto)
        {
            if (!ModelState.IsValid) return View("Login", userDto);

            // Пошук користувача в базі даних
            var dbUser = context.Users.FirstOrDefault(user => user.Name == userDto.Name && user.Password == userDto.Password);

            if (dbUser == null)
            {
                ModelState.AddModelError("Password", "Invalid login or password");
                return View("Login", userDto);
            }

            // Створення та автентифікація користувача
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(new ClaimsIdentity(
                    new List<Claim>
                    {
                new(ClaimTypes.Name, dbUser.Name),
                new(ClaimTypes.Role, dbUser.Role)
                    }, CookieAuthenticationDefaults.AuthenticationScheme)));

            // Перенаправлення на попередню сторінку, якщо вона є
            if (!string.IsNullOrWhiteSpace(userDto.ReturnUrl) && Url.IsLocalUrl(userDto.ReturnUrl))
                return Redirect(userDto.ReturnUrl);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> LogOff()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            ViewBag.Roles = new List<string> { "Parents", "Children" }; // Список доступних ролей
            return View(); // Повертаємо пусту форму
        }
        [HttpPost]
        public IActionResult CreateUser(User user)
        {
            using (var dbContext = new HomeAccountingContext())
            {
                // Перевірка, чи існує ім'я користувача в базі
                if (dbContext.Users.Any(u => u.Name == user.Name))
                {
                    ModelState.AddModelError("Name", "Це ім'я користувача вже зайняте.");
                    ViewBag.Roles = new List<string> { "Parents", "Children" }; // Передаємо список ролей у разі помилки
                    return View(user); // Повертаємо ту саму форму з помилкою
                }

                // Якщо ім'я не зайняте, додаємо користувача в базу
                dbContext.Users.Add(user);
                dbContext.SaveChanges();
            }

            return RedirectToAction("Login", "Home"); // Повернення на сторінку входу після реєстрації
        }
    }
}
