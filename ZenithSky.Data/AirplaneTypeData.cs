using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.SqlClient;
using ZenithSky.Domain;

namespace ZenithSky.Data
{
    public class AirplaneTypeData
    {
        private readonly string connectionString;

        public AirplaneTypeData(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<IEnumerable<AirplaneType>> GetAirplaneTypes()
        {
            var airplaneTypes = new List<AirplaneType>();

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = @"
                    SELECT * FROM airplane_type";

                using (var command = new SqlCommand(query, connection))
                {

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var airplaneType = new AirplaneType
                            {
                                TypeId = reader.GetInt32(0),
                                Identifier = reader.GetString(1),
                                Description = reader.GetString(2),
                            };

                            airplaneTypes.Add(airplaneType);
                        }
                    }
                }
            } // GetAirplaneTypes

            return airplaneTypes;
        }
    }
}
