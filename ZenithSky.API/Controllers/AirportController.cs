using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ZenithSky.Business;
using ZenithSky.Domain;

namespace ZenithSky.API.Controllers
{
    [Produces("application/json")]
    [Route("api/airports")]
    [ApiController]
    public class AirportsController : ControllerBase
    {
        private readonly AirportBusiness airportBusiness;

        public AirportsController(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("ZenithSky_Proyecto_JEDL")
                ?? throw new InvalidOperationException("Cadena de conexión 'ZenithSky_Proyecto_JEDL' no configurada.");
            this.airportBusiness = new AirportBusiness(connectionString);
        }

        // GET: api/airports  o  api/airports?search=SJO  (busca por nombre, IATA o ICAO)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Airport>>> GetAll([FromQuery] string? search = null)
        {
            var airports = await airportBusiness.GetAirports(search);
            return Ok(airports);
        }

        // GET: api/airports/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Airport>> GetById(int id)
        {
            if (id <= 0)
                return BadRequest(new { message = "El ID del aeropuerto debe ser mayor que cero." });

            var airport = await airportBusiness.GetByAirportId(id);
            if (airport == null)
                return NotFound(new { message = $"No se encontró el aeropuerto con ID {id}." });

            return Ok(airport);
        }

        // POST: api/airports  -> valida iata (3 letras) e icao (4 letras) obligatorios y no duplicados
        [HttpPost]
        public async Task<ActionResult<Airport>> Create([FromBody] AirportRequest request)
        {
            var error = await Validate(request, 0);
            if (error != null)
                return error;

            int newId = await airportBusiness.Insert(request);
            var created = await airportBusiness.GetByAirportId(newId);

            return CreatedAtAction(nameof(GetById), new { id = newId }, created);
        }

        // PUT: api/airports/1  -> mismas validaciones, sin contar al propio aeropuerto como duplicado
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AirportRequest request)
        {
            if (id <= 0)
                return BadRequest(new { message = "El ID del aeropuerto debe ser mayor que cero." });

            if (await airportBusiness.GetByAirportId(id) == null)
                return NotFound(new { message = $"No se encontró el aeropuerto con ID {id}." });

            var error = await Validate(request, id);
            if (error != null)
                return error;

            bool updated = await airportBusiness.Update(id, request);
            if (!updated)
                return NotFound(new { message = $"No se encontró el aeropuerto con ID {id}." });

            return NoContent();
        }

        // DELETE: api/airports/1  -> 409 Conflict si es base_airport de alguna aerolínea
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest(new { message = "El ID del aeropuerto debe ser mayor que cero." });

            var airport = await airportBusiness.GetByAirportId(id);
            if (airport == null)
                return NotFound(new { message = $"No se encontró el aeropuerto con ID {id}." });

            if (await airportBusiness.IsBaseAirport(id))
                return Conflict(new { message = "No se puede eliminar el aeropuerto porque es el aeropuerto base de una o más aerolíneas." });

            bool deleted = await airportBusiness.Delete(id);
            if (!deleted)
                return NotFound(new { message = $"No se encontró el aeropuerto con ID {id}." });

            return NoContent();
        }

        // Validaciones comunes del POST y PUT. Devuelve null si todo está bien.
        private async Task<ActionResult?> Validate(AirportRequest? request, int excludeId)
        {
            if (request == null)
                return BadRequest(new { message = "Debe enviar los datos del aeropuerto." });

            request.Iata = request.Iata?.Trim().ToUpper() ?? "";
            request.Icao = request.Icao?.Trim().ToUpper() ?? "";
            request.Name = request.Name?.Trim() ?? "";

            if (request.Iata.Length != 3 || !request.Iata.All(char.IsAsciiLetter))
                return BadRequest(new { message = "El código IATA es obligatorio y debe tener exactamente 3 letras." });

            if (request.Icao.Length != 4 || !request.Icao.All(char.IsAsciiLetter))
                return BadRequest(new { message = "El código ICAO es obligatorio y debe tener exactamente 4 letras." });

            if (request.Name.Length == 0 || request.Name.Length > 100)
                return BadRequest(new { message = "El nombre del aeropuerto es obligatorio y no puede superar los 100 caracteres." });

            if (await airportBusiness.IataExists(request.Iata, excludeId))
                return Conflict(new { message = $"Ya existe un aeropuerto con el código IATA {request.Iata}." });

            if (await airportBusiness.IcaoExists(request.Icao, excludeId))
                return Conflict(new { message = $"Ya existe un aeropuerto con el código ICAO {request.Icao}." });

            return null;
        }
    }
}
