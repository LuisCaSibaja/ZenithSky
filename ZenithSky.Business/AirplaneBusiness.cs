using System;
using System.Collections.Generic;
using System.Text;
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
    }
}
