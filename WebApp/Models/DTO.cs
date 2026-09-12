using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace WebApp.Models
{
    public class DTO
    {
        [JsonPropertyName("token")]
        public required string Token { get; set; }

        [JsonPropertyName("uloga")]
        public string? Uloga { get; set; }

        [JsonPropertyName("korisnickoIme")]
        public string? KorisnickoIme { get; set; }

    }
}
