using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.SqlClient;
using ZenithSky.Domain;

namespace ZenithSky.Data
{
    public class AirportData
    {
        private readonly string connectionString;

        public AirportData(string connectionString)
        {
            this.connectionString = connectionString;
        }

        //hacer procedimiento para avion?
        //solo hice el primer cuadro
        // Lista todos los aeropuertos o filtra por nombre, IATA o ICAO
        public async Task<IEnumerable<Airport>> GetAirports(string search)
        {
            var airports = new List<Airport>();

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = @"
                    SELECT airport_id, iata, icao, name
                    FROM airport
                    WHERE @search IS NULL OR @search = ''
                          OR name LIKE @pattern
                          OR iata LIKE @pattern
                          OR icao LIKE @pattern
                    ORDER BY airport_id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@search", (object?)search ?? DBNull.Value);
                    command.Parameters.AddWithValue("@pattern", $"%{search?.Trim()}%");

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var airport = new Airport
                            {
                                AirportId = reader.GetInt32(0),
                                Iata = reader.GetString(1),
                                Icao = reader.GetString(2),
                                Name = reader.GetString(3)
                            };

                            airports.Add(airport);
                        }
                    }
                }
            } // GetAirports

            return airports;
        }

        // Consulta un aeropuerto por su ID
        public async Task<Airport?> GetByAirportId(int id)
        {
            Airport? airport = null;

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = @"
                    SELECT airport_id, iata, icao, name
                    FROM airport
                    WHERE airport_id = @id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            airport = new Airport
                            {
                                AirportId = reader.GetInt32(0),
                                Iata = reader.GetString(1),
                                Icao = reader.GetString(2),
                                Name = reader.GetString(3)
                            };
                        }
                    }
                }
            } // GetByAirportId

            return airport;
        } 

        public void Insertar(Airport airport)//revisar
        {

        }

        public void Eliminar(int airport_id)//revisar
        {

        }

        public void Actualizar(int airport_id)//revisar
        {

        }
    }
}
