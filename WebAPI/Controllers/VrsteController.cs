using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VrsteController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public VrsteController(ApplicationDbContext db) { _db = db; }

        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> Get() => Ok(await _db.Vrste.AsNoTracking().ToListAsync());

        [HttpPost, Authorize(Roles="admin")]
        public async Task<IActionResult> Create([FromBody] Vrsta v)
        {
            if (await _db.Vrste.AnyAsync(x => x.Naziv == v.Naziv))
                return BadRequest("Naziv već postoji.");
            _db.Vrste.Add(v);
            await _db.SaveChangesAsync();
            return Ok(v);
        }

        [HttpPut("{id:int}"), Authorize(Roles="admin")]
        public async Task<IActionResult> Update(int id, [FromBody] Vrsta v)
        {
            var dbV = await _db.Vrste.FindAsync(id);
            if (dbV == null) return NotFound();
            dbV.Naziv = v.Naziv;
            await _db.SaveChangesAsync();
            return Ok(dbV);
        }

        [HttpDelete("{id:int}"), Authorize(Roles = "Admin,admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var v = await _db.Vrste.FindAsync(id);
            if (v == null) return NotFound();

            var inUse = await _db.Aktivnosti.AnyAsync(a => a.VrstaId == id);
            if (inUse) return Conflict("Vrsta se koristi na aktivnostima. Najprije promijenite/obrišite povezane aktivnosti.");

            _db.Vrste.Remove(v);
            await _db.SaveChangesAsync();
            return NoContent();
        }

    }
}
