using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nest;
using System.Security.Claims;
using WebAPI.DTOs;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AktivnostiController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public AktivnostiController(ApplicationDbContext db) { _db = db; }

        // GET list
        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> Get() =>
            Ok(await _db.Aktivnosti
                .Include(a => a.Vrsta)
                .Include(a => a.AktivnostPristupacnosti).ThenInclude(ap => ap.Pristupacnost)
                .AsNoTracking().ToListAsync());

        // GET search + paging
        [HttpGet("search"), AllowAnonymous]
        public async Task<IActionResult> Search(string? filter = "", int page = 1, int count = 10, int? vrstaId = null, [FromServices] IMapper mapper = null!)
        {
            var q = _db.Aktivnosti
                .Include(a => a.Vrsta)
                .Include(a => a.AktivnostPristupacnosti).ThenInclude(ap => ap.Pristupacnost)
                .Include(a => a.Recenzije)
                .AsNoTracking()
                .Where(a => string.IsNullOrEmpty(filter) || a.Naziv.Contains(filter));

            if (vrstaId is not null) q = q.Where(a => a.VrstaId == vrstaId);

            var total = await q.CountAsync();
            var items = await q.OrderBy(a => a.Naziv).Skip((page - 1) * count).Take(count).ToListAsync();

            var dto = mapper.Map<List<AktivnostDTO>>(items);
            return Ok(new { items = dto, total });
        }

        // POST create (s vezama)
        [HttpPost, Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([FromBody] AktivnostAddDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            if (!await _db.Vrste.AnyAsync(v => v.Id == dto.VrstaId))
            {
                ModelState.AddModelError(nameof(dto.VrstaId), "Odabrana vrsta ne postoji.");
                return ValidationProblem(ModelState);
            }
            if (await _db.Aktivnosti.AnyAsync(x => x.Naziv == dto.Naziv))
                return BadRequest("Naziv aktivnosti već postoji.");

            var pristIds = dto.PristupacnostiIds.Distinct().ToList();
            var postoje = await _db.Pristupacnosti.Where(p => pristIds.Contains(p.Id)).Select(p => p.Id).ToListAsync();
            var missing = pristIds.Except(postoje).ToList();
            if (missing.Count > 0)
            {
                ModelState.AddModelError(nameof(dto.PristupacnostiIds), $"Nepostojeći ID-evi: {string.Join(", ", missing)}");
                return ValidationProblem(ModelState);
            }

            var a = new Aktivnost
            {
                Naziv = dto.Naziv,
                Opis = dto.Opis,
                Lokacija = dto.Lokacija,
                Kontakt = dto.Kontakt,
                VrstaId = dto.VrstaId
            };
            _db.Aktivnosti.Add(a); await _db.SaveChangesAsync();

            _db.AktivnostPristupacnosti.AddRange(pristIds.Select(pid => new AktivnostPristupacnost
            {
                AktivnostId = a.Id,
                PristupacnostId = pid
            }));

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId) || userId <= 0)
                return Forbid("Invalid user id in token.");

            if (dto.Recenzije is { Count: > 0 })
            {
                var recs = new List<Recenzija>();
                foreach (var r in dto.Recenzije)
                {

                    recs.Add(new Recenzija
                    {
                        AktivnostId = a.Id,
                        PristupacnostId = pristIds.First(),
                        Ocjena = r.Ocjena,
                        Komentar = r.Komentar,
                        KorisnikId = userId
                    });
                }
                _db.Set<Recenzija>().AddRange(recs);
            }

            await _db.SaveChangesAsync();

            var created = await _db.Set<Aktivnost>()
        .Include(x => x.Vrsta)
        .Include(x => x.AktivnostPristupacnosti).ThenInclude(ap => ap.Pristupacnost)
        .AsNoTracking()
        .Where(x => x.Id == a.Id)
        .Select(x => new
        {
            x.Id,
            x.Naziv,
            x.Opis,
            x.Lokacija,
            x.Kontakt,
            x.VrstaId,
            Vrsta = x.Vrsta == null ? null : new { x.Vrsta.Id, x.Vrsta.Naziv },
            AktivnostPristupacnosti = x.AktivnostPristupacnosti.Select(ap => new
            {
                ap.PristupacnostId,
                Pristupacnost = ap.Pristupacnost == null ? null : new { ap.Pristupacnost.Id, ap.Pristupacnost.Naziv }
            })
        })
        .FirstAsync();

            return CreatedAtAction(nameof(GetById), new { id = a.Id }, created);
        }

            [HttpGet("{id:int}"), AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var a = await _db.Aktivnosti
                .Include(x => x.Vrsta)
                .Include(x => x.AktivnostPristupacnosti).ThenInclude(ap => ap.Pristupacnost)
                .Include(x => x.Recenzije).ThenInclude(r => r.Pristupacnost)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            return a == null ? NotFound() : Ok(a);
        }
        [HttpPut("{id:int}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(int id, [FromBody] AktivnostAddDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var a = await _db.Aktivnosti
                .Include(x => x.AktivnostPristupacnosti)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (a == null) return NotFound();

            if (!await _db.Vrste.AnyAsync(v => v.Id == dto.VrstaId))
            {
                ModelState.AddModelError(nameof(dto.VrstaId), "Odabrana vrsta ne postoji.");
                return ValidationProblem(ModelState);
            }

            var pristIds = (dto.PristupacnostiIds ?? new List<int>()).Distinct().ToList();
            var postoje = await _db.Pristupacnosti.Where(p => pristIds.Contains(p.Id)).Select(p => p.Id).ToListAsync();
            var missing = pristIds.Except(postoje).ToList();
            if (missing.Count > 0)
            {
                ModelState.AddModelError(nameof(dto.PristupacnostiIds), $"Nepostojeći ID-evi: {string.Join(", ", missing)}");
                return ValidationProblem(ModelState);
            }

            // osnovna polja
            a.Naziv = dto.Naziv;
            a.Opis = dto.Opis;
            a.Lokacija = dto.Lokacija;
            a.Kontakt = dto.Kontakt;
            a.VrstaId = dto.VrstaId;

            // sync M-N: postojeće vs nove
            var existing = a.AktivnostPristupacnosti.Select(x => x.PristupacnostId).ToList();
            var toAdd = pristIds.Except(existing).ToList();
            var toRemove = existing.Except(pristIds).ToList();

            if (toRemove.Count > 0)
            {
                var removeRows = a.AktivnostPristupacnosti.Where(x => toRemove.Contains(x.PristupacnostId)).ToList();
                _db.AktivnostPristupacnosti.RemoveRange(removeRows);
            }
            if (toAdd.Count > 0)
            {
                var addRows = toAdd.Select(pid => new AktivnostPristupacnost
                {
                    AktivnostId = a.Id,
                    PristupacnostId = pid
                });
                _db.AktivnostPristupacnosti.AddRange(addRows);
            }

            await _db.SaveChangesAsync();

            return NoContent();
        }

        // DELETE
        [HttpDelete("{id:int}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var a = await _db.Aktivnosti
                .Include(x => x.AktivnostPristupacnosti)
                .Include(x => x.Recenzije)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (a == null) return NotFound();

            
            if (a.AktivnostPristupacnosti?.Count > 0)
                _db.AktivnostPristupacnosti.RemoveRange(a.AktivnostPristupacnosti);

            if (a.Recenzije?.Count > 0)
                _db.Recenzije.RemoveRange(a.Recenzije);

            _db.Aktivnosti.Remove(a);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
