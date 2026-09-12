namespace WebApp.Models
{
    public class Recenzija
    {
        public int Id { get; set; }
        public string Tekst { get; set; }
        public int Ocjena { get; set; }

        public int AktivnostId { get; set; }
        public Aktivnost Aktivnost { get; set; }

        public int KorisnikId { get; set; }
        public Korisnik Korisnik { get; set; }
    }
}