using System;
using System.Security.Cryptography;
using WebAPI.Models;

public class Aktivnost
{
    public int Id { get; set; }
    public string Naziv { get; set; }
    public string Opis { get; set; }
    public string Lokacija { get; set; }
    public string Kontakt { get; set; }

    public int VrstaId { get; set; }
    public Vrsta Vrsta { get; set; }

    public List<AktivnostPristupacnost> AktivnostPristupacnosti { get; set; }
    public List<Recenzija> Recenzije { get; set; }
}