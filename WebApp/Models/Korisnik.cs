using WebApp.Models;

public class Korisnik
{
    public int Id { get; set; }
    public required string KorisnickoIme { get; set; }
    public required string Email { get; set; }
    public required string LozinkaHash { get; set; }
    public required string Uloga { get; set; }

    public List<Recenzija>? Recenzije { get; set; }
}
