namespace WebApp.Models
{
    public class Pristupacnost
    {
        public int Id { get; set; }
        public string Naziv { get; set; }

        public List<AktivnostPristupacnost> AktivnostPristupacnosti { get; set; }
    }
}