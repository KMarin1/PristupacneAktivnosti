using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTOs
{
    public class KorisnikUpdateDTO
    {
        [Required, EmailAddress, StringLength(200)]
        public string Email { get; set; } = default!;

        [StringLength(80)]
        public string? Ime { get; set; }

        [StringLength(80)]
        public string? Prezime { get; set; }

        [Phone, StringLength(40)]
        public string? Telefon { get; set; }
    }
}
