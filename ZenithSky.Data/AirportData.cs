using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
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

        //crud con procedimientos almacenados
        // Inserta un aeropuerto y devuelve el id generado
        public async Task<int> Insert(AirportRequest airport)
        {
            int newId;

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_InsertarAirport", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@iata", airport.Iata);
                    command.Parameters.AddWithValue("@icao", airport.Icao);
                    command.Parameters.AddWithValue("@name", airport.Name);

                    newId = Convert.ToInt32(await command.ExecuteScalarAsync());
                }
            }
            return newId;
        }

        // Actualiza un aeropuerto; devuelve false si no existe
        public async Task<bool> Update(int id, AirportRequest airport)
        {
            int rowsAffected;

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_ActualizarAirport", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@airport_id", id);
                    command.Parameters.AddWithValue("@iata", airport.Iata);
                    command.Parameters.AddWithValue("@icao", airport.Icao);
                    command.Parameters.AddWithValue("@name", airport.Name);

                    rowsAffected = Convert.ToInt32(await command.ExecuteScalarAsync());
                }
            }
            return rowsAffected > 0;
        }

        // Elimina un aeropuerto; devuelve false si no existe
        public async Task<bool> Delete(int id)
        {
            int rowsAffected;

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_EliminarAirport", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@airport_id", id);

                    rowsAffected = Convert.ToInt32(await command.ExecuteScalarAsync());
                }
            }
            return rowsAffected > 0;
        }

        // Para el POST y PUT: ¿ya existe otro aeropuerto con ese IATA? (excludeId = 0 en el POST)
        public async Task<bool> IataExists(string iata, int excludeId = 0)
        {
            return await ExecuteCount("sp_ExisteIataAirport", "@iata", iata, excludeId) > 0;
        }

        // Para el POST y PUT: ¿ya existe otro aeropuerto con ese ICAO? (excludeId = 0 en el POST)
        public async Task<bool> IcaoExists(string icao, int excludeId = 0)
        {
            return await ExecuteCount("sp_ExisteIcaoAirport", "@icao", icao, excludeId) > 0;
        }

        // Para el 409 Conflict: el aeropuerto es base_airport de alguna aerolínea
        public async Task<bool> IsBaseAirport(int airportId)
        {
            int count;

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_EsAeropuertoBase", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@airport_id", airportId);

                    count = Convert.ToInt32(await command.ExecuteScalarAsync());
                }
            }
            return count > 0;
        }

        private async Task<int> ExecuteCount(string procedure, string codeParameter, string code, int excludeId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand(procedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue(codeParameter, code);
                    command.Parameters.AddWithValue("@airport_id", excludeId);

                    return Convert.ToInt32(await command.ExecuteScalarAsync());
                }
            }
        }
    }
}
