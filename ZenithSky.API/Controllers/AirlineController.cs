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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Airline>>> GetAll()
        {
            return Ok(await airlineBusiness.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Airline>> GetById(int id)
        {
            try
            {
                var airline = await airlineBusiness.GetById(id);
                if (airline == null)
                    return NotFound(new { message = $"No se encontró la aerolínea con ID {id}." });

                return Ok(airline);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<Airline>> Create([FromBody] AirlineDTO request)
        {
            try
            {
                var created = await airlineBusiness.Insert(request);
                return CreatedAtAction(nameof(GetById), new { id = created.AirlineId }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AirlineDTO request)
        {
            try
            {
                await airlineBusiness.Update(id, request);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        //Aquí lanzamos el error 409 si la aerolínea tiene aviones, este está en la capa bussinesss
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await airlineBusiness.Delete(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}