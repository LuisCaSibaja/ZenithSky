using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZenithSky.Data;
using ZenithSky.Domain;
using ZenithSky.Dto;

namespace ZenithSky.Business
{
    public class AirplaneBusiness
    {
        private readonly AirplaneData airplaneData;

        public AirplaneBusiness(string connectionString)
        {
            this.airplaneData = new AirplaneData(connectionString);
        }

        public async Task<IEnumerable<Airplane>> GetAll(int? typeId = null, int? airlineId = null)
        {
            return await airplaneData.GetAll(typeId, airlineId);
        }

        public async Task<Airplane?> GetById(int id)
        {
            return await airplaneData.GetById(id);
        }

        public async Task<int> Insert(AirplaneDTO airplane)
        {
            return await airplaneData.Insert(airplane);
        }

        public async Task<bool> Update(int id, AirplaneDTO airplane)
        {
            return await airplaneData.Update(id, airplane);
        }

        public async Task<bool> Delete(int id)
        {
            return await airplaneData.Delete(id);
        }

        public async Task<bool> AirplaneTypeExists(int typeId)
        {
            return await airplaneData.AirplaneTypeExists(typeId);
        }

        public async Task<bool> AirlineExists(int airlineId)
        {
            return await airplaneData.AirlineExists(airlineId);
        }

        public async Task<BusinessResult> Validate(AirplaneDTO? request)
        {
            if (request == null)
                return BusinessResult.BadRequest("Debe enviar los datos del avión.");

            if (request.Capacity <= 0)
                return BusinessResult.BadRequest("La capacidad de pasajeros debe ser un entero mayor que cero.");

            int typeId = request.TypeId;
            if (typeId <= 0 || !await airplaneData.AirplaneTypeExists(typeId))
                return BusinessResult.BadRequest($"El tipo de avión con ID {typeId} no existe.");

            int airlineId = request.AirlineId;
            if (airlineId <= 0 || !await airplaneData.AirlineExists(airlineId))
                return BusinessResult.BadRequest($"La aerolínea con ID {airlineId} no existe.");

            return BusinessResult.Ok();
        }

        public async Task<BusinessResult> ValidateDelete(int id)
        {
            if (id <= 0)
                return BusinessResult.BadRequest("El ID del avión debe ser mayor que cero.");

            var airplane = await airplaneData.GetById(id);
            if (airplane == null)
                return BusinessResult.NotFound($"No se encontró el avión con ID {id}.");

            return BusinessResult.Ok();
        }
    }
}
