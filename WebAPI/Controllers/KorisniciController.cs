using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebAPI.DTOs;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class KorisniciController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogService _log;

        public KorisniciController(ApplicationDbContext context, ILogService log)
        {
            _context = context;
            _log = log;
        }

        // GET: api/Korisnici
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Korisnik>>> GetKorisnici()
        {
            return await _context.Korisnici.ToListAsync();
        }

        // GET: api/Korisnici/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Korisnik>> GetKorisnik(int id)
        {
            var korisnik = await _context.Korisnici.FindAsync(id);

            if (korisnik == null)
            {
                await _log.LogAsync($"Nije pronaden korisnik: (ID: {id})");


                return NotFound();
            }

            return korisnik;
        }

        // PUT: api/Korisnik/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutKorisnik(int id, Korisnik korisnik)
        {
            if (id != korisnik.Id)
            {
                return BadRequest();
            }

            _context.Entry(korisnik).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KorisnikExists(id))
                {
                    await _log.LogAsync($"Nije pronaden korisnik: (ID: {id})");
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Korisnik
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Korisnik>> PostKorisnik(Korisnik korisnik)
        {
            _context.Korisnici.Add(korisnik);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetKorisnik", new { id = korisnik.Id }, korisnik);
        }

        // DELETE: api/Korisnik/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKorisnik(int id)
        {
            var korisnik = await _context.Korisnici.FindAsync(id);
            if (korisnik == null)
            {
                await _log.LogAsync($"Nije pronaden korisnik: (ID: {id})");
                return NotFound();
            }

            _context.Korisnici.Remove(korisnik);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("recenzije")]
        public async Task<IActionResult> WithReviews()
        {
            var data = await _context.Korisnici
                .Select(k => new {
                    k.Id,
                    k.KorisnickoIme,
                    k.Email,
                    k.Uloga,
                    RecenzijaCount = k.Recenzije.Count,
                    Zadnje = k.Recenzije.OrderByDescending(r => r.Id)
                               .Take(3)
                               .Select(r => new { r.Ocjena, r.Komentar, r.AktivnostId })
                }).ToListAsync();

            return Ok(data);
        }

        [HttpGet("profile")]
        public async Task<IActionResult> Profile()
        {
            var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var u = await _context.Korisnici.FindAsync(id);
            if (u == null) return NotFound();
            return Ok(new { u.KorisnickoIme, u.Email, u.Ime, u.Prezime, u.Telefon, u.Uloga });
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] KorisnikUpdateDTO dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var u = await _context.Korisnici.FindAsync(id);
            if (u == null) return NotFound();

            u.Email = dto.Email;
            u.Ime = dto.Ime;
            u.Prezime = dto.Prezime;
            u.Telefon = dto.Telefon;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Profil ažuriran." });
        }

        private bool KorisnikExists(int id)
        {
            return _context.Korisnici.Any(e => e.Id == id);
        }


    } 
}
