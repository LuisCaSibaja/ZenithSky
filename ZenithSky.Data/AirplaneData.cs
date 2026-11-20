using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using ZenithSky.Domain;
using ZenithSky.Dto;

namespace ZenithSky.Data
{
    public class AirplaneData
    {
        private readonly string connectionString;

        public AirplaneData(string connectionString)
        {
            this.connectionString = connectionString;
        }

        //crud con procedimientos almacenados
        // Lista los aviones con tipo y aerolínea; typeId y airlineId son filtros opcionales
        public async Task<IEnumerable<Airplane>> GetAll(int? typeId = null, int? airlineId = null)
        {
            var airplanes = new List<Airplane>();

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_ListarAirplane", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@type_id", (object?)typeId ?? DBNull.Value);
                    command.Parameters.AddWithValue("@airline_id", (object?)airlineId ?? DBNull.Value);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            airplanes.Add(MapAirplane(reader));
                        }
                    }
                }
            }
            return airplanes;
        }

        public async Task<Airplane?> GetById(int id)
        {
            Airplane? airplane = null;

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_ObtenerIdAirplane", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@airplane_id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            airplane = MapAirplane(reader);
                        }
                    }
                }
            }
            return airplane;
        }

        public async Task<int> Insert(AirplaneDTO airplane)
        {
            int newId;

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_InsertarAirplane", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@capacity", airplane.Capacity);
                    command.Parameters.AddWithValue("@type_id", airplane.AirplaneType!.TypeId);
                    command.Parameters.AddWithValue("@airline_id", airplane.Airline!.AirlineId);

                    newId = Convert.ToInt32(await command.ExecuteScalarAsync());
                }
            }
            return newId;
        }

        public async Task<bool> Update(int id, AirplaneDTO airplane)
        {
            int rowsAffected;

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_ActualizarAirplane", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@airplane_id", id);
                    command.Parameters.AddWithValue("@capacity", airplane.Capacity);
                    command.Parameters.AddWithValue("@type_id", airplane.AirplaneType!.TypeId);
                    command.Parameters.AddWithValue("@airline_id", airplane.Airline!.AirlineId);

                    rowsAffected = Convert.ToInt32(await command.ExecuteScalarAsync());
                }
            }
            return rowsAffected > 0;
        }

        public async Task<bool> Delete(int id)
        {
            int rowsAffected;

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_EliminarAirplane", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@airplane_id", id);

                    rowsAffected = Convert.ToInt32(await command.ExecuteScalarAsync());
                }
            }
            return rowsAffected > 0;
        }

        // Para el POST y PUT: valida que exista el tipo de avión
        public async Task<bool> AirplaneTypeExists(int typeId)
        {
            return await ExecuteCount("sp_ExisteTipoAvion", "@type_id", typeId) > 0;
        }

        // Para el POST y PUT: valida que exista la aerolínea
        public async Task<bool> AirlineExists(int airlineId)
        {
            return await ExecuteCount("sp_ExisteAirline", "@airline_id", airlineId) > 0;
        }

        private async Task<int> ExecuteCount(string procedure, string parameter, int value)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand(procedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue(parameter, value);

                    return Convert.ToInt32(await command.ExecuteScalarAsync());
                }
            }
        }

        // Columnas en el orden que devuelven sp_ListarAirplane y sp_ObtenerIdAirplane
        private static Airplane MapAirplane(SqlDataReader reader)
        {
            return new Airplane
            {
                AirplaneId = reader.GetInt32(0),
                Capacity = reader.GetInt32(1),
                AirplaneType = new AirplaneType
                {
                    TypeId = reader.GetInt32(2),
                    Identifier = reader.GetString(3),
                    Description = reader.IsDBNull(4) ? "" : reader.GetString(4)
                },
                Airline = new Airline
                {
                    AirlineId = reader.GetInt32(5),
                    Iata = reader.GetString(6),
                    AirlineName = reader.GetString(7),
                    BaseAirport = new Airport
                    {
                        AirportId = reader.GetInt32(8),
                        Iata = reader.GetString(9),
                        Icao = reader.GetString(10),
                        Name = reader.GetString(11)
                    }
                }
            };
        }
    }
}
