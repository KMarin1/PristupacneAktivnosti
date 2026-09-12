using WebApp.Models;

public class AktivnostPristupacnost
{
    public int AktivnostId { get; set; }
    public Aktivnost Aktivnost { get; set; }

    public int PristupacnostId { get; set; }
    public Pristupacnost Pristupacnost { get; set; }
}