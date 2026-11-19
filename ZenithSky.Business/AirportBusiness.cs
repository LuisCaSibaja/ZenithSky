using System;
using System.Collections.Generic;
using System.Text;
using ZenithSky.Data;
using ZenithSky.Domain;

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

        public async Task<int> Insert(AirportRequest airport)
        {
            return await airportData.Insert(airport);
        }

        public async Task<bool> Update(int id, AirportRequest airport)
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
    }
}
