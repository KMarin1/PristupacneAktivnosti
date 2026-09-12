using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTOs
{
    public class AktivnostAddDto
    {
        [Required, StringLength(120), Display(Name = "Naziv")]
        public string Naziv { get; set; } = default!;

        [StringLength(2000), Display(Name = "Opis")]
        public string? Opis { get; set; }

        [Required, StringLength(200), Display(Name = "Lokacija")]
        public string Lokacija { get; set; } = default!;

        [EmailAddress, StringLength(200), Display(Name = "Kontakt e-mail")]
        public string? Kontakt { get; set; }

        [Required, Display(Name = "Vrsta")]
        public int VrstaId { get; set; }

        [MinLength(1), Display(Name = "Pristupačnosti")]
        public List<int> PristupacnostiIds { get; set; } = new();

        public List<RecenzijaAddDto>? Recenzije { get; set; }
    }

}
