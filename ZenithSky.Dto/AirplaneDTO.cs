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
        private int typeId;
        private int airlineId;

        public AirplaneDTO()
        {
        }

        [JsonPropertyName("airplaneId")]
        public int AirplaneId { get => airplaneId; set => airplaneId = value; }

        [Range(1, int.MaxValue, ErrorMessage = "La capacidad de pasajeros debe ser un entero mayor que cero.")]
        [DefaultValue(186)]
        [JsonPropertyName("capacity")]
        public int Capacity { get => capacity; set => capacity = value; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un tipo de avión válido.")]
        [DefaultValue(2)]
        [JsonPropertyName("typeId")]
        public int TypeId { get => typeId; set => typeId = value; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una aerolínea válida.")]
        [DefaultValue(1)]
        [JsonPropertyName("airlineId")]
        public int AirlineId { get => airlineId; set => airlineId = value; }
    }
}