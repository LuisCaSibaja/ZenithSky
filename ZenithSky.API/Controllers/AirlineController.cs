using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ZenithSky.Business;
using ZenithSky.Domain;
using ZenithSky.Dto;

namespace ZenithSky.API.Controllers
{
    [Produces("application/json")]
    [Route("api/airlines")]
    [ApiController]
    public class AirlinesController : ControllerBase
    {
        private readonly AirlineBusiness airlineBusiness;

        public AirlinesController(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("ZenithSky_Proyecto_JEDL")
                ?? throw new InvalidOperationException("Cadena de conexión 'ZenithSky_Proyecto_JEDL' no configurada.");
            this.airlineBusiness = new AirlineBusiness(connectionString);
        }

        //para listar las aerolíneas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Airline>>> GetAll()
        {
            var airlines = await airlineBusiness.GetAll();
            return Ok(airlines);
        }

        //para obtener por id
        [HttpGet("{id}")]
        public async Task<ActionResult<Airline>> GetById(int id)
        {
            if (id <= 0)
                return BadRequest(new { message = "El ID de la aerolínea debe ser mayor que cero." });

            var airline = await airlineBusiness.GetById(id);
            if (airline == null)
                return NotFound(new { message = $"No se encontró la aerolínea con ID {id}." });

            return Ok(airline);
        }

        //Este es para crearlas y valida que el aeropuerto base exista
        [HttpPost]
        public async Task<ActionResult<Airline>> Create([FromBody] AirlineDTO request)
        {
            if (request == null)
                return BadRequest(new { message = "Debe enviar los datos de la aerolínea." });

            request.Iata = request.Iata?.Trim().ToUpper() ?? "";
            request.Airlinename = request.Airlinename?.Trim() ?? "";

            if (request.Iata.Length != 2)
                return BadRequest(new { message = "El código IATA es obligatorio y debe tener exactamente 2 caracteres." });

            if (request.Airlinename.Length == 0 || request.Airlinename.Length > 100)
                return BadRequest(new { message = "El nombre de la aerolínea es obligatorio y no puede superar los 100 caracteres." });

            if (!await airlineBusiness.AirportExists(request.BaseAirport!.AirportId))
                return BadRequest(new { message = $"El aeropuerto base con ID {request.BaseAirport.AirportId} no existe." });

            int newId = await airlineBusiness.Insert(request);
            var created = await airlineBusiness.GetById(newId);

            return CreatedAtAction(nameof(GetById), new { id = newId }, created);
        }

        //para actualizar, con las mismo
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AirlineDTO request)
        {
            if (id <= 0)
                return BadRequest(new { message = "El ID de la aerolínea debe ser mayor que cero." });

            if (request == null)
                return BadRequest(new { message = "Debe enviar los datos de la aerolínea." });

            request.Iata = request.Iata?.Trim().ToUpper() ?? "";
            request.Airlinename = request.Airlinename?.Trim() ?? "";

            if (request.Iata.Length != 2)
                return BadRequest(new { message = "El código IATA es obligatorio y debe tener exactamente 2 caracteres." });

            if (request.Airlinename.Length == 0 || request.Airlinename.Length > 100)
                return BadRequest(new { message = "El nombre de la aerolínea es obligatorio y no puede superar los 100 caracteres." });

            if (!await airlineBusiness.AirportExists(request.BaseAirport!.AirportId))
                return BadRequest(new { message = $"El aeropuerto base con ID {request.BaseAirport.AirportId} no existe." });

            bool updated = await airlineBusiness.Update(id, request);
            if (!updated)
                return NotFound(new { message = $"No se encontró la aerolínea con ID {id}." });

            return NoContent();
        }

        //eliminamos usando la regla que se pdie
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest(new { message = "El ID de la aerolínea debe ser mayor que cero." });

            var airline = await airlineBusiness.GetById(id);
            if (airline == null)
                return NotFound(new { message = $"No se encontró la aerolínea con ID {id}." });

            if (await airlineBusiness.HasAirplanes(id))
                return Conflict(new { message = "No se puede eliminar la aerolínea porque posee aeronaves asignadas." });

            bool deleted = await airlineBusiness.Delete(id);
            if (!deleted)
                return NotFound(new { message = $"No se encontró la aerolínea con ID {id}." });

            return NoContent();
        }
    }
}