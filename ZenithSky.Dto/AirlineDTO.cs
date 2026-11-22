using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ZenithSky.Dto
{
    public class AirlineDTO
    {
        private int airlineId;
        private string? iata;
        private string? airlinename;
        private int baseAirport;

        public AirlineDTO()
        {

        }

        [JsonPropertyName("airlineId")]
        public int AirlineId { get => airlineId; set => airlineId = value; }

        [Required(ErrorMessage = "El código IATA es obligatorio.")]
        [RegularExpression("^[A-Za-z0-9]{2}$", ErrorMessage = "El código IATA debe tener exactamente 2 caracteres.")]
        [DefaultValue("ZS")]
        [JsonPropertyName("iata")]
        public string? Iata { get => iata; set => iata = value; }

        [Required(ErrorMessage = "El nombre de la aerolínea es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre de la aerolínea no puede superar los 100 caracteres.")]
        [DefaultValue("ZenithSky Airlines")]
        [JsonPropertyName("airlinename")]
        public string? Airlinename { get => airlinename; set => airlinename = value; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un aeropuerto base válido.")]
        [DefaultValue(1)]
        [JsonPropertyName("baseAirport")]
        public int BaseAirport { get => baseAirport; set => baseAirport = value; }
    }
}
