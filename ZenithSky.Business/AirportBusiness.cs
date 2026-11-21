using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenithSky.Data;
using ZenithSky.Domain;
using ZenithSky.Dto;

namespace ZenithSky.Business
{
    public class AirportBusiness
    {
        private readonly AirportData airportData;

        public AirportBusiness(string connectionString)
        {
            this.airportData = new AirportData(connectionString);
        }

        // Lista todos o filtra por nombre, IATA o ICAO
        public async Task<IEnumerable<Airport>> GetAirports(string? search)
        {
            return await airportData.GetAirports(search ?? "");
        }

        public async Task<Airport?> GetByAirportId(int id)
        {
            return await airportData.GetByAirportId(id);
        }

        public async Task<int> Insert(AirportDTO airport)
        {
            return await airportData.Insert(airport);
        }

        public async Task<bool> Update(int id, AirportDTO airport)
        {
            return await airportData.Update(id, airport);
        }

        public async Task<bool> Delete(int id)
        {
            return await airportData.Delete(id);
        }

        public async Task<bool> IataExists(string iata, int excludeId = 0)
        {
            return await airportData.IataExists(iata, excludeId);
        }

        public async Task<bool> IcaoExists(string icao, int excludeId = 0)
        {
            return await airportData.IcaoExists(icao, excludeId);
        }

        public async Task<bool> IsBaseAirport(int id)
        {
            return await airportData.IsBaseAirport(id);
        }

        // Reglas de negocio para creación y actualización de aeropuertos
        public async Task<BusinessResult> Validate(AirportDTO? request, int excludeId = 0)
        {
            if (request == null)
                return BusinessResult.BadRequest("Debe enviar los datos del aeropuerto.");

            request.Iata = request.Iata?.Trim().ToUpper() ?? "";
            request.Icao = request.Icao?.Trim().ToUpper() ?? "";
            request.Name = request.Name?.Trim() ?? "";

            // RN-APT-01: Código IATA de 3 letras alfabéticas
            if (request.Iata.Length != 3 || !request.Iata.All(char.IsAsciiLetter))
                return BusinessResult.BadRequest("El código IATA es obligatorio y debe tener exactamente 3 letras.");

            // RN-APT-02: Código ICAO de 4 letras alfabéticas
            if (request.Icao.Length != 4 || !request.Icao.All(char.IsAsciiLetter))
                return BusinessResult.BadRequest("El código ICAO es obligatorio y debe tener exactamente 4 letras.");

            // RN-APT-03: Nombre obligatorio hasta 100 caracteres
            if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Length > 100)
                return BusinessResult.BadRequest("El nombre del aeropuerto es obligatorio y no puede superar los 100 caracteres.");

            // RN-APT-04: Unicidad IATA 
            if (await airportData.IataExists(request.Iata, excludeId))
                return BusinessResult.Conflict($"Ya existe un aeropuerto con el código IATA {request.Iata}.");

            // RN-APT-05: Unicidad ICAO 
            if (await airportData.IcaoExists(request.Icao, excludeId))
                return BusinessResult.Conflict($"Ya existe un aeropuerto con el código ICAO {request.Icao}.");

            return BusinessResult.Ok();
        }

        // Regla de negocio para eliminación de aeropuertos
        public async Task<BusinessResult> ValidateDelete(int id)
        {
            if (id <= 0)
                return BusinessResult.BadRequest("El ID del aeropuerto debe ser mayor que cero.");

            var airport = await airportData.GetByAirportId(id);
            if (airport == null)
                return BusinessResult.NotFound($"No se encontró el aeropuerto con ID {id}.");

            // RN-APT-07: Si el aeropuerto es base_airport de alguna aerolínea, retorna 409 Conflict
            if (await airportData.IsBaseAirport(id))
                return BusinessResult.Conflict("No se puede eliminar el aeropuerto porque es el aeropuerto base de una o más aerolíneas.");

            return BusinessResult.Ok();
        }
    }
}
