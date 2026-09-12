namespace WebAPI.Models
{
    public class Recenzija
    {
        public int Id { get; set; }
        public string? Komentar { get; set; }
        public int Ocjena { get; set; }

        public int AktivnostId { get; set; }
        public Aktivnost? Aktivnost { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public int KorisnikId { get; set; }
        public Korisnik? Korisnik { get; set; }

        public int PristupacnostId { get; set; }
        public Pristupacnost? Pristupacnost { get; set; }
    }
}