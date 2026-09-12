using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly JwtService _jwt;
        private readonly ILogService _log;

        public AuthController(ApplicationDbContext db, JwtService jwt, ILogService log)
        { _db = db; _jwt = jwt; _log = log; }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            if (await _db.Korisnici.AnyAsync(k => k.KorisnickoIme == req.KorisnickoIme))
                return BadRequest("Korisničko ime već postoji.");

            var korisnik = new Korisnik
            {
                KorisnickoIme = req.KorisnickoIme,
                LozinkaHash = HashLozinka(req.Lozinka), // hash here
                Uloga = "Korisnik",
                Email = req.Email
            };

            _db.Korisnici.Add(korisnik);
            await _db.SaveChangesAsync();
            await _log.LogAsync($"Registriran korisnik {korisnik.KorisnickoIme}");
            return Ok("Uspješna registracija.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var u = await _db.Korisnici.FirstOrDefaultAsync(k => k.KorisnickoIme == req.KorisnickoIme);
            if (u == null || u.LozinkaHash != HashLozinka(req.Lozinka))
                return Unauthorized("Neispravno korisničko ime ili lozinka.");

            var token = _jwt.GenerateToken(u);
            await _log.LogAsync($"Login: {u.KorisnickoIme}");
            return Ok(new { token });
        }

        [Authorize]
        [HttpPost("changepassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest req)
        {
            var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var u = await _db.Korisnici.FindAsync(id);
            if (u == null || u.LozinkaHash != HashLozinka(req.StaraLozinka))
                return BadRequest("Stara lozinka je netočna.");

            u.LozinkaHash = HashLozinka(req.NovaLozinka);
            await _db.SaveChangesAsync();
            await _log.LogAsync($"Promijenjena lozinka: {u.KorisnickoIme}");
            return Ok("Lozinka promijenjena.");
        }

        private static string HashLozinka(string lozinka)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(lozinka)));
        }
    }

    public class LoginRequest { public required string KorisnickoIme { get; set; } public required string Lozinka { get; set; } }
    public class ChangePasswordRequest { public required string StaraLozinka { get; set; } public required string NovaLozinka { get; set; } }
    public class RegisterRequest { public string KorisnickoIme { get; set; } = ""; public string Lozinka { get; set; } = ""; public string? Email { get; set; }
    }
}
