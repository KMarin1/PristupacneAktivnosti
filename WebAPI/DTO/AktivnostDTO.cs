using Microsoft.AspNetCore.Mvc;

namespace WebAPI.DTOs
{
    public class AktivnostDTO
    {
        public int Id { get; set; }
        public string Naziv { get; set; } = default!;
        public string? Opis { get; set; }
        public string Lokacija { get; set; } = default!;
        public string? Kontakt { get; set; }
        public int VrstaId { get; set; }
        public string VrstaNaziv { get; set; } = "";
        public List<string> Pristupacnosti { get; set; } = new();
        public double? ProsjecnaOcjena { get; set; }
    }
}
