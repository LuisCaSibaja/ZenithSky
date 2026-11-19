using System;
using System.Collections.Generic;
using System.Text;
using ZenithSky.Data;
using ZenithSky.Domain;

namespace ZenithSky.Business
{
    public class AirplaneTypeBusiness
    {
        private readonly AirplaneTypeData airplaneTypeData;

        public AirplaneTypeBusiness(string connectionString)
        {
            this.airplaneTypeData = new AirplaneTypeData(connectionString);
        }

        public async Task<IEnumerable<AirplaneType>> GetAirplaneTypes()
        {
            return await airplaneTypeData.GetAirplaneTypes();
        }

    }
}
