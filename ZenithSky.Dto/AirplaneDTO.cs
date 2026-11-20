using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ZenithSky.Dto
{
    public class AirplaneDTO
    {
        private int airplaneId;
        private int capacity;
        private AirplaneTypeDTO? airplaneType;
        private AirlineDTO? airline;

        public AirplaneDTO()
        {
        }

        [JsonPropertyName("airplaneId")]
        public int AirplaneId { get => airplaneId; set => airplaneId = value; }

        [Range(1, int.MaxValue, ErrorMessage = "La capacidad de pasajeros debe ser un entero mayor que cero.")]
        [DefaultValue(186)]
        [JsonPropertyName("capacity")]
        public int Capacity { get => capacity; set => capacity = value; }

        [Required(ErrorMessage = "El tipo de avión es obligatorio.")]
        [JsonPropertyName("airplaneType")]
        public AirplaneTypeDTO? AirplaneType { get => airplaneType; set => airplaneType = value; }

        [Required(ErrorMessage = "La aerolínea es obligatoria.")]
        [JsonPropertyName("airline")]
        public AirlineDTO? Airline { get => airline; set => airline = value; }
    }
}