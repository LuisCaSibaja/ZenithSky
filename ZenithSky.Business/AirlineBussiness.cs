using System;
using System.Collections.Generic;
using System.Text;
using ZenithSky.Data;
using ZenithSky.Domain;
using ZenithSky.Dto;

namespace ZenithSky.Business
{
    public class AirlineBusiness
    {
        private readonly AirlineData airlineData;

        public AirlineBusiness(string connectionString)
        {
            this.airlineData = new AirlineData(connectionString);
        }

        public async Task<IEnumerable<Airline>> GetAll()
        {
            return await airlineData.GetAll();
        }

        public async Task<Airline?> GetById(int id)
        {
            return await airlineData.GetById(id);
        }

        public async Task<int> Insert(AirlineDTO airline)
        {
            return await airlineData.Insert(airline);
        }

        public async Task<bool> Update(int id, AirlineDTO airline)
        {
            return await airlineData.Update(id, airline);
        }

        public async Task<bool> Delete(int id)
        {
            return await airlineData.Delete(id);
        }

        public async Task<bool> AirportExists(int airportId)
        {
            return await airlineData.AirportExists(airportId);
        }

        public async Task<bool> HasAirplanes(int id)
        {
            return await airlineData.HasAirplanes(id);
        }
    }
}