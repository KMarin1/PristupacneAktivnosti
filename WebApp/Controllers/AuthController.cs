using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace WebApp.Controllers
{
    public class AuthController : Controller
    {
        private const string ApiBase = "https://localhost:7024/";

        [HttpGet]
        public IActionResult Login()
        {
            ViewBag.Msg = TempData["msg"];
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string korisnickoIme, string lozinka)
        {
            using var client = new HttpClient { BaseAddress = new Uri(ApiBase) };
            var res = await client.PostAsJsonAsync("api/Auth/login", new { KorisnickoIme = korisnickoIme, Lozinka = lozinka });
            var raw = await res.Content.ReadAsStringAsync();

            if (!res.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", string.IsNullOrWhiteSpace(raw) ? "Neuspješna prijava." : raw);
                return View();
            }

            // Extract token from API response
            string? token;
            try
            {
                using var doc = JsonDocument.Parse(raw);
                token = doc.RootElement.TryGetProperty("token", out var tEl) ? tEl.GetString() : null;
            }
            catch
            {
                token = null;
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                ModelState.AddModelError("", "Token nedostaje.");
                return View();
            }

            // Decode JWT to get claims
            string role = "Korisnik";
            string? userId = null;
            string? userName = null;

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);

                // Some issuers use long URIs for claim types, support both
                userId = jwt.Claims.FirstOrDefault(c =>
                             c.Type == ClaimTypes.NameIdentifier ||
                             c.Type == "nameid" ||
                             c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")
                         ?.Value;

                userName = jwt.Claims.FirstOrDefault(c =>
                               c.Type == ClaimTypes.Name ||
                               c.Type == "unique_name" ||
                               c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")
                           ?.Value;

                var rawRole = jwt.Claims.FirstOrDefault(c =>
                                  c.Type == ClaimTypes.Role ||
                                  c.Type == "role" ||
                                  c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
                              ?.Value;

                if (!string.IsNullOrWhiteSpace(rawRole))
                {
                    role = rawRole.Equals("admin", StringComparison.OrdinalIgnoreCase) ? "Admin" : rawRole;
                }
            }
            catch
            {
                // If decoding fails, default role stays "Korisnik"
            }

            // Store everything in localStorage and go to Aktivnosti
            return Content($@"
<script>
  localStorage.setItem('jwt', '{token}');
  localStorage.setItem('role', '{role}');
  {(userId is not null ? $"localStorage.setItem('userId','{userId}');" : "")}
  {(userName is not null ? $"localStorage.setItem('username','{userName}');" : "")}
  location='/Aktivnosti/Index';
</script>", "text/html");
        }

        public IActionResult Logout() => Content(@"
<script>
  localStorage.removeItem('jwt');
  localStorage.removeItem('role');
  localStorage.removeItem('userId');
  localStorage.removeItem('username');
  location='/Auth/Login';
</script>", "text/html");

        [HttpPost]
        public async Task<IActionResult> Register(string korisnickoIme, string lozinka, string email)
        {
            using var client = new HttpClient { BaseAddress = new Uri(ApiBase) };
            var res = await client.PostAsJsonAsync("api/Auth/register", new
            {
                KorisnickoIme = korisnickoIme,
                Lozinka = lozinka,   // plain password; API hashes it
                Email = email
            });

            var raw = await res.Content.ReadAsStringAsync();

            if (!res.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", string.IsNullOrWhiteSpace(raw) ? "Registracija nije uspjela." : raw);
                return View("Login");
            }

            TempData["msg"] = "Uspješna registracija! Sada se prijavite.";
            return RedirectToAction("Login");
        }
    }
}
