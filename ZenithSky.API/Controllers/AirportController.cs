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
    [Route("api/airports")]
    [ApiController]
    public class AirportsController : ControllerBase
    {
        private readonly AirportBusiness airportBusiness;

        public AirportsController(IConfiguration configuration)
        {
            // Asegúrate de que el nombre "ZenithSkyDB" coincida con tu appsettings.json
            string connectionString = configuration.GetConnectionString("ZenithSkyDB")
                ?? throw new InvalidOperationException("Cadena de conexión 'ZenithSkyDB' no configurada.");
            this.airportBusiness = new AirportBusiness(connectionString);
        }

        // GET: api/airports?search=SJO
        [HttpGet]
        public IEnumerable<Airport> GetAll([FromQuery] string? search = null)
        {
            Airport airports = airportBusiness.GetAllAirports(search);
            return airports;
        }////metodo de busqueda de aeropuertos

        // GET: api/airports/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Airport>> GetById(int id)
        {
            try
            {
                var airport = await airportBusiness.GetByAirportId(id);
                if (airport == null)
                    return NotFound(new { message = $"No se encontró el aeropuerto con ID {id}." });

                return Ok(airport);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }//metodo de id de aeropuertos
    }
}