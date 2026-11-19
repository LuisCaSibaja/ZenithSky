using System.Text.Json.Serialization;

namespace ZenithSky.Domain
{
    // Datos que llegan en el POST y PUT de /api/airports (JSON A del enunciado)
    public class AirportRequest
    {
        [JsonPropertyName("iata")]
        public string? Iata { get; set; }

        [JsonPropertyName("icao")]
        public string? Icao { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }
}
