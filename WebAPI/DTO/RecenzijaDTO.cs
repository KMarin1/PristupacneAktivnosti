using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTOs
{
    public class RecenzijaAddDto
    {
        [Range(1, 5), Display(Name = "Ocjena (1-5)")]
        public int Ocjena { get; set; }

        [StringLength(2000), Display(Name = "Komentar")]
        public string? Komentar { get; set; }
    }
}
