using System.ComponentModel;
using System.Text.Json.Serialization;

namespace ZenithSky.Domain
{
    // Datos que llegan en el POST y PUT de /api/airlines (JSON B del enunciado)
    public class AirlineRequest
    {
        [DefaultValue("AA")]
        [JsonPropertyName("iata")]
        public string? Iata { get; set; }

        [DefaultValue("American Airlines")]
        [JsonPropertyName("airlinename")]
        public string? Airlinename { get; set; }

        [DefaultValue(1)]
        [JsonPropertyName("baseAirport")]
        public int BaseAirport { get; set; }
    }
}