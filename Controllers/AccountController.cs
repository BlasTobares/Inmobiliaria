using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Inmobiliaria.Data;
using Inmobiliaria.Models;

namespace Inmobiliaria.Controllers
{
    public class AccountController : Controller
    {
        private readonly RepositorioUsuario repo;

        public AccountController(IConfiguration config)
        {
            repo = new RepositorioUsuario(config.GetConnectionString("DefaultConnection")!);
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password, string? returnUrl = null)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Email y contraseña son requeridos.");
                return View();
            }

            var user = repo.ObtenerPorEmail(email);
            if (user == null)
            {
                ModelState.AddModelError("", "Email o contraseña incorrecta.");
                return View();
            }

            // Verificar password (tu RepositorioUsuario hashgea con Base64)
            var passwordHash = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
            if (user.PasswordHash != passwordHash)
            {
                ModelState.AddModelError("", "Email o contraseña incorrecta.");
                return View();
            }

            // claims + cookie
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, string.IsNullOrEmpty(user.Nombre) ? user.Email : user.Nombre),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Rol ?? "Empleado")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // además guardo info en Session para tu código existente
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Rol", user.Rol ?? "Empleado");
            HttpContext.Session.SetString("Nombre", user.Nombre ?? user.Email);
            if (!string.IsNullOrEmpty(user.AvatarPath))
                HttpContext.Session.SetString("Avatar", user.AvatarPath);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
