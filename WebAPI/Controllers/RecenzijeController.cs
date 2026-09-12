using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RecenzijeController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public RecenzijeController(ApplicationDbContext db) { _db = db; }

        public class RecAddDto
        {
            public int AktivnostId { get; set; }
            public int? PristupacnostId { get; set; }
            public int Ocjena { get; set; }
            public string? Komentar { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RecAddDto dto)
        {
            if (dto.Ocjena < 1 || dto.Ocjena > 5) return BadRequest("Ocjena 1-5.");
            var a = await _db.Aktivnosti.AsNoTracking().FirstOrDefaultAsync(x => x.Id == dto.AktivnostId);
            if (a == null) return NotFound("Aktivnost ne postoji.");

            if (dto.PristupacnostId.HasValue && !await _db.Pristupacnosti.AnyAsync(p => p.Id == dto.PristupacnostId.Value))
                return BadRequest("PristupacnostId ne postoji.");

            var uidStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(uidStr, out var uid)) return Forbid();

            var r = new Recenzija
            {
                AktivnostId = dto.AktivnostId,
                PristupacnostId = dto.PristupacnostId ?? (await _db.AktivnostPristupacnosti.Where(x => x.AktivnostId == dto.AktivnostId).Select(x => x.PristupacnostId).FirstOrDefaultAsync()),
                Ocjena = dto.Ocjena,
                Komentar = dto.Komentar,
                KorisnikId = uid
            };

            _db.Recenzije.Add(r);
            await _db.SaveChangesAsync();
            return Ok(r);
        }


            [HttpDelete("{id:int}")]
            public async Task<IActionResult> Delete(int id)
            {
                var rec = await _db.Recenzije.FindAsync(id);
                if (rec == null) return NotFound();

                var uid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var role = (User.FindFirstValue(ClaimTypes.Role) ?? "").ToLowerInvariant();
                var isOwner = rec.KorisnikId == uid;
                var isAdmin = role == "admin";

                if (!isOwner && !isAdmin) return Forbid();

                _db.Recenzije.Remove(rec);
                await _db.SaveChangesAsync();
                return NoContent();
            }
        

    }
}
