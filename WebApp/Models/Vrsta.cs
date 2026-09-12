namespace WebApp.Models
{
    public class Vrsta
    {
        public int Id { get; set; }
        public string Naziv { get; set; }

        public List<Aktivnost> Aktivnosti { get; set; }
    }
}