using System.Text.Json.Serialization;

namespace ZenithSky.Domain
{
    // Datos que llegan en el POST y PUT de /api/airlines (JSON B del enunciado)
    public class AirlineRequest
    {
        [JsonPropertyName("iata")]
        public string? Iata { get; set; }

        [JsonPropertyName("airlinename")]
        public string? Airlinename { get; set; }

        [JsonPropertyName("baseAirport")]
        public int BaseAirport { get; set; }
    }
}