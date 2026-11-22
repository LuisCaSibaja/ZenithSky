using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.SqlClient;
using ZenithSky.Domain;
using ZenithSky.Dto;
using System.Data;

namespace ZenithSky.Data{
    public class AirlineData{
        private readonly string connectionString;

        public AirlineData(string connectionString)
        {
           this.connectionString = connectionString;
        }

         //crud con procedimientos almacenados
         public async Task<IEnumerable<Airline>> GetAll()
         {
            var airlines = new List<Airline>();
            using (var connection = new SqlConnection(connectionString))
            {
               await connection.OpenAsync();
               using (var command = new SqlCommand("sp_ListarAirline", connection))
               {
                  command.CommandType = CommandType.StoredProcedure;
                  using (var reader = await command.ExecuteReaderAsync())
                  {
                     while (await reader.ReadAsync())
                     {
                         var airline = new Airline
                         {
                            AirlineId = reader.GetInt32(0),
                            Iata = reader.GetString(1),
                            AirlineName = reader.GetString(2),
                            BaseAirport = new Airport
                            {
                               AirportId = reader.GetInt32(3),
                               Iata = reader.GetString(4),
                               Icao = reader.GetString(5),
                               Name = reader.GetString(6)
                            }
                         };
                         airlines.Add(airline);
                     }
                  }
               }
            }
            return airlines;
         }

          public async Task<Airline?> GetById(int id)
          {
             Airline? airline = null;
             using (var connection = new SqlConnection(connectionString))
             {
                await connection.OpenAsync();
                using (var command = new SqlCommand("sp_ObtenerIdAirline", connection))
                {
                   command.CommandType = CommandType.StoredProcedure;
                   command.Parameters.AddWithValue("@airline_id", id);
                   using (var reader = await command.ExecuteReaderAsync())
                   {
                      if (await reader.ReadAsync())
                      {
                         airline = new Airline
                         {
                            AirlineId = reader.GetInt32(0),
                            Iata = reader.GetString(1),
                            AirlineName = reader.GetString(2),
                            BaseAirport = new Airport
                            {
                               AirportId = reader.GetInt32(3),
                               Iata = reader.GetString(4),
                               Icao = reader.GetString(5),
                               Name = reader.GetString(6)
                            }
                         };
                      }
                   }
                }
             }
             return airline;
          }

          public async Task<int> Insert(AirlineDTO airline)
          {
             int newId;
             using (var connection = new SqlConnection(connectionString))
             {
                 await connection.OpenAsync();

                 using (var command = new SqlCommand("sp_InsertarAirline", connection))
                 {
                     command.CommandType = CommandType.StoredProcedure;
                     command.Parameters.AddWithValue("@iata", airline.Iata);
                     command.Parameters.AddWithValue("@airlinename", airline.Airlinename);
                     command.Parameters.AddWithValue("@base_airport", airline.BaseAirport);

                    newId = Convert.ToInt32(await command.ExecuteScalarAsync());
                 }
             }
             return newId;
          }

          public async Task<bool> Update(int id, AirlineDTO airline)
          {
              int rowsAffected;

              using (var connection = new SqlConnection(connectionString))
              {
                 await connection.OpenAsync();

                 using (var command = new SqlCommand("sp_ActualizarAirline", connection))
                 {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@airline_id", id);
                    command.Parameters.AddWithValue("@iata", airline.Iata);
                    command.Parameters.AddWithValue("@airlinename", airline.Airlinename);
                    command.Parameters.AddWithValue("@base_airport", airline.BaseAirport);

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

                using (var command = new SqlCommand("sp_EliminarAirline", connection))
                {
                   command.CommandType = CommandType.StoredProcedure;
                   command.Parameters.AddWithValue("@airline_id", id);

                   rowsAffected = Convert.ToInt32(await command.ExecuteScalarAsync());
                }
             }

             return rowsAffected > 0;
          }

          //eesto es para el post y put
          public async Task<bool> AirportExists(int airportId)
          {
             int count;

             using (var connection = new SqlConnection(connectionString))
             {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_ExisteAeropuertoBase", connection))
                {
                   command.CommandType = CommandType.StoredProcedure;
                   command.Parameters.AddWithValue("@airport_id", airportId);

                   count = Convert.ToInt32(await command.ExecuteScalarAsync());
                }
             }
             return count > 0;
          }

          //esto lo hice para la parte del 409 conflict que pide el profe
          public async Task<bool> HasAirplanes(int airlineId)
          {
             int count;

             using (var connection = new SqlConnection(connectionString))
             {
                await connection.OpenAsync();
                using (var command = new SqlCommand("sp_TieneAviones", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@airline_id", airlineId);

                    count = Convert.ToInt32(await command.ExecuteScalarAsync());
                    }
                }
             return count > 0;
          }
    }
}