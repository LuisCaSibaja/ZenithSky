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

        public async List<Airport> GetAllAirports(string search)
        {
            return await airportData.GetAllAirports(search);
        }

        public async Task<Airport?> GetByAirportId(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID del aeropuerto debe ser mayor que cero.");

            return await airportData.GetByAirportId(id);
        }

        
    }
}
