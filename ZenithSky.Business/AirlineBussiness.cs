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
            if (id <= 0)
                throw new ArgumentException("El ID de la aerolínea debe ser mayor que cero.");

            return await airlineData.GetById(id);
        }

        // Retorna la aerolínea creada con su aeropuerto base, para eso los datos deben de ser válidos y la aerolínea debe de existr.
        public async Task<Airline> Insert(AirlineDTO airline)
        {
            await Validate(airline);

            int newId = await airlineData.Insert(airline);

            return (await airlineData.GetById(newId))!;
        }

        // Igual que el anterior, solo que si no existe la aerolínea lanza error 404
        public async Task Update(int id, AirlineDTO airline)
        {
            if (id <= 0)
                throw new ArgumentException("El ID de la aerolínea debe ser mayor que cero.");

            await Validate(airline);

            if (!await airlineData.Update(id, airline))
                throw new KeyNotFoundException($"No se encontró la aerolínea con ID {id}.");
        }

        // Aquí se implementa lo del error 409, no se puede eliminar una aerolínea con aviones asociados
        public async Task Delete(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID de la aerolínea debe ser mayor que cero.");

            if (await airlineData.GetById(id) == null)
                throw new KeyNotFoundException($"No se encontró la aerolínea con ID {id}.");

            if (await airlineData.HasAirplanes(id))
                throw new InvalidOperationException("No se puede eliminar la aerolínea porque posee aeronaves asignadas.");

            await airlineData.Delete(id);
        }

        //validaciones
        private async Task Validate(AirlineDTO? airline)
        {
            if (airline == null)
                throw new ArgumentException("Debe enviar los datos de la aerolínea.");

            airline.Iata = airline.Iata?.Trim().ToUpper() ?? "";
            airline.Airlinename = airline.Airlinename?.Trim() ?? "";

            if (airline.Iata.Length != 2)
                throw new ArgumentException("El código IATA es obligatorio y debe tener exactamente 2 letras.");

            if (airline.Airlinename.Length == 0 || airline.Airlinename.Length > 100)
                throw new ArgumentException("El nombre de la aerolínea es obligatorio y no puede superar los 100 caracteres.");

            if (!await airlineData.AirportExists(airline.BaseAirport))
                throw new ArgumentException($"El aeropuerto base con ID {airline.BaseAirport} no existe.");
        }
    }
}