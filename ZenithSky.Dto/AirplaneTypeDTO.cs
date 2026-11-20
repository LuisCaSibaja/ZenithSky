using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ZenithSky.Dto
{
    public class AirplaneTypeDTO
    {
        private int typeId;
        private string? identifier;
        private string? description;

        public AirplaneTypeDTO()
        {
        }

        [JsonPropertyName("typeId")]
        public int TypeId { get => typeId; set => typeId = value; }

        [Required(ErrorMessage = "El identificador es obligatorio.")]
        [DefaultValue("A320")]
        [JsonPropertyName("identifier")]
        public string? Identifier { get => identifier; set => identifier = value; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [DefaultValue("Airbus A320 comercial de pasillo único")]
        [JsonPropertyName("description")]
        public string? Description { get => description; set => description = value; }
    }
}