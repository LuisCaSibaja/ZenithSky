using Microsoft.AspNetCore.Mvc;
using ZenithSky.Business;
using ZenithSky.Domain;

namespace ZenithSky.API.Controllers
{
    [Produces("application/json")]
    [Route("api/airplane-types")]
    [ApiController]
    public class AirplaneTypeController : ControllerBase
    {
        private readonly AirplaneTypeBusiness airplaneTypeBusiness;

        public AirplaneTypeController(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("ZenithSky_Proyecto_JEDL")
                ?? throw new InvalidOperationException("Cadena de conexión 'ZenithSky_Proyecto_JEDL' no configurada.");
            this.airplaneTypeBusiness = new AirplaneTypeBusiness(connectionString);
        }

        // GET: api/airplane-types
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AirplaneType>>> GetAll()
        {
            var airplaneTypes = await airplaneTypeBusiness.GetAirplaneTypes();
            return Ok(airplaneTypes);
        }
    }
}
