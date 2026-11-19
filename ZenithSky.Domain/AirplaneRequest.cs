using System.Text.Json.Serialization;

namespace ZenithSky.Domain
{
    // Datos que llegan en el POST y PUT de /api/airplanes (JSON C del enunciado)
    public class AirplaneRequest
    {
        [JsonPropertyName("capacity")]
        public int Capacity { get; set; }

        [JsonPropertyName("typeId")]
        public int TypeId { get; set; }

        [JsonPropertyName("airlineId")]
        public int AirlineId { get; set; }
    }
}
