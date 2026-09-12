using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.DTOs;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PristupacnostController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public PristupacnostController(ApplicationDbContext db) { _db = db; }

        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> Get() => Ok(await _db.Pristupacnosti.AsNoTracking().ToListAsync());

        [HttpPost, Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([FromBody] PristupacnostDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            // provjera duplikata
            if (await _db.Pristupacnosti.AnyAsync(x => x.Naziv == dto.Naziv))
            {
                ModelState.AddModelError("Naziv", "Pristupačnost s ovim nazivom već postoji.");
                return ValidationProblem(ModelState);
            }

            var p = new Pristupacnost { Naziv = dto.Naziv };
            _db.Pristupacnosti.Add(p);
            await _db.SaveChangesAsync();

            return Ok(p);
        }

        [HttpPut("{id:int}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(int id, [FromBody] Pristupacnost p)
        {
            var dbP = await _db.Pristupacnosti.FindAsync(id);
            if (dbP == null) return NotFound();
            dbP.Naziv = p.Naziv;
            await _db.SaveChangesAsync();
            return Ok(dbP);
        }

        [HttpDelete("{id:int}"), Authorize(Roles = "Admin,admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var p = await _db.Pristupacnosti.FindAsync(id);
            if (p == null) return NotFound();

            var linkedAp = await _db.AktivnostPristupacnosti.AnyAsync(x => x.PristupacnostId == id);
            var linkedRec = await _db.Recenzije.AnyAsync(r => r.PristupacnostId == id);
            if (linkedAp || linkedRec)
                return Conflict("Pristupačnost je povezana s aktivnostima ili recenzijama. Uklonite poveznice prije brisanja.");

            _db.Pristupacnosti.Remove(p);
            await _db.SaveChangesAsync();
            return NoContent();
        }

    }
}
