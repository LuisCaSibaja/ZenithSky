using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ZenithSky.Business;
using ZenithSky.Domain;

namespace ZenithSky.API.Controllers
{
    [Produces("application/json")]
    [Route("api/airplanes")]
    [ApiController]
    public class AirplanesController : ControllerBase
    {
        private readonly AirplaneBusiness airplaneBusiness;

        public AirplanesController(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("ZenithSky_Proyecto_JEDL")
                ?? throw new InvalidOperationException("Cadena de conexión 'ZenithSky_Proyecto_JEDL' no configurada.");
            this.airplaneBusiness = new AirplaneBusiness(connectionString);
        }

        // GET: api/airplanes  o  api/airplanes?typeId=2  o  api/airplanes?airlineId=1
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Airplane>>> GetAll([FromQuery] int? typeId = null, [FromQuery] int? airlineId = null)
        {
            if (typeId.HasValue && typeId <= 0)
                return BadRequest(new { message = "El filtro typeId debe ser mayor que cero." });

            if (airlineId.HasValue && airlineId <= 0)
                return BadRequest(new { message = "El filtro airlineId debe ser mayor que cero." });

            var airplanes = await airplaneBusiness.GetAll(typeId, airlineId);
            return Ok(airplanes);
        }

        // GET: api/airplanes/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Airplane>> GetById(int id)
        {
            if (id <= 0)
                return BadRequest(new { message = "El ID del avión debe ser mayor que cero." });

            var airplane = await airplaneBusiness.GetById(id);
            if (airplane == null)
                return NotFound(new { message = $"No se encontró el avión con ID {id}." });

            return Ok(airplane);
        }

        // POST: api/airplanes  -> valida capacity > 0 y que existan el tipo y la aerolínea
        [HttpPost]
        public async Task<ActionResult<Airplane>> Create([FromBody] AirplaneRequest request)
        {
            var error = await Validate(request);
            if (error != null)
                return error;

            int newId = await airplaneBusiness.Insert(request);
            var created = await airplaneBusiness.GetById(newId);

            return CreatedAtAction(nameof(GetById), new { id = newId }, created);
        }

        // PUT: api/airplanes/1
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AirplaneRequest request)
        {
            if (id <= 0)
                return BadRequest(new { message = "El ID del avión debe ser mayor que cero." });

            var error = await Validate(request);
            if (error != null)
                return error;

            bool updated = await airplaneBusiness.Update(id, request);
            if (!updated)
                return NotFound(new { message = $"No se encontró el avión con ID {id}." });

            return NoContent();
        }

        // DELETE: api/airplanes/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest(new { message = "El ID del avión debe ser mayor que cero." });

            bool deleted = await airplaneBusiness.Delete(id);
            if (!deleted)
                return NotFound(new { message = $"No se encontró el avión con ID {id}." });

            return NoContent();
        }

        // Validaciones comunes del POST y PUT. Devuelve null si todo está bien.
        private async Task<ActionResult?> Validate(AirplaneRequest? request)
        {
            if (request == null)
                return BadRequest(new { message = "Debe enviar los datos del avión." });

            if (request.Capacity <= 0)
                return BadRequest(new { message = "La capacidad de pasajeros debe ser un entero mayor que cero." });

            if (!await airplaneBusiness.AirplaneTypeExists(request.TypeId))
                return BadRequest(new { message = $"El tipo de avión con ID {request.TypeId} no existe." });

            if (!await airplaneBusiness.AirlineExists(request.AirlineId))
                return BadRequest(new { message = $"La aerolínea con ID {request.AirlineId} no existe." });

            return null;
        }
    }
}
