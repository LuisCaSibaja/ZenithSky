using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ZenithSky.Dto
{
    public class AirportDTO
    {
        private int airportId;
        private string? iata;
        private string? icao;
        private string? name;

        public AirportDTO()
        {

        }

        [JsonPropertyName("airportId")]
        public int AirportId { get => airportId; set => airportId = value; }

        [Required(ErrorMessage = "El código IATA es obligatorio.")]
        [RegularExpression("^[A-Za-z]{3}$", ErrorMessage = "El código IATA debe tener exactamente 3 letras.")]
        [DefaultValue("SJO")]
        [JsonPropertyName("iata")]
        public string? Iata { get => iata; set => iata = value; }

        [Required(ErrorMessage = "El código ICAO es obligatorio.")]
        [RegularExpression("^[A-Za-z]{4}$", ErrorMessage = "El código ICAO debe tener exactamente 4 letras.")]
        [DefaultValue("MROC")]
        [JsonPropertyName("icao")]
        public string? Icao { get => icao; set => icao = value; }

        [Required(ErrorMessage = "El nombre del aeropuerto es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre del aeropuerto no puede superar los 100 caracteres.")]
        [DefaultValue("Aeropuerto Internacional Juan Santamaría")]
        [JsonPropertyName("name")]
        public string? Name { get => name; set => name = value; }
    }
}

