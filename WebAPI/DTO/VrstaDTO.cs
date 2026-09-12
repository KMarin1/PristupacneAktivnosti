using Nest;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTOs
{
    public class VrstaDto
    {
        public int Id { get; set; }

        [Required, StringLength(120), Display(Name = "Naziv")]
        public string Naziv { get; set; } = default!;
    }
}
