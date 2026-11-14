using ZenithSky.Data;
using ZenithSky.Domain;

namespace ZenithSky.Test.Data
{
    [TestFixture]
    public class AirportDataTest
    {
        private string connectionString;

        [SetUp]
        public void Setup()
        {
            this.connectionString = "Data Source=163.178.173.130;Initial Catalog=ZenithSky_Proyecto_JEDL;User ID=Lenguajes;Password=lenguajesparaiso2025;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False;Command Timeout=300";

        }

        [Test]
        public async Task GetAirports_Iata_Exitoso()
        {
            // Arrange
            AirportData airportData = new AirportData(this.connectionString);

            // Act
            IEnumerable<Airport> airports = await airportData.GetAirports("SJO");
            // Assert
            Assert.That(airports.Count(), Is.EqualTo(1));
            Assert.That(airports.First().Iata, Is.EqualTo("SJO"));

            System.Console.WriteLine("Aeropuertos encontrados por iata:");
            foreach (var airport in airports)
            {
                System.Console.WriteLine($"- {airport.Name} ({airport.Iata})");
            }
        }

        [Test]
        public async Task GetAirports_Icao_Exitoso()
        {
            // Arrange
            AirportData airportData = new AirportData(this.connectionString);

            // Act
            IEnumerable<Airport> airports = await airportData.GetAirports("KMIA");
            // Assert
            Assert.That(airports.Count(), Is.EqualTo(1));
            Assert.That(airports.First().Icao, Is.EqualTo("KMIA"));

            System.Console.WriteLine("Aeropuertos encontrados por icao:");
            foreach (var airport in airports)
            {
                System.Console.WriteLine($"- {airport.Name} ({airport.Icao})");
            }
        }

        [Test]
        public async Task GetAirports_NameAirport_Exitoso()
        {
            // Arrange
            AirportData airportData = new AirportData(this.connectionString);

            // Act
            IEnumerable<Airport> airports = await airportData.GetAirports("Aeropuerto");
            // Assert
            Assert.That(airports.Count(), Is.EqualTo(4));

            System.Console.WriteLine("Aeropuertos encontrados por nombre:");
            foreach (var airport in airports)
            {
                System.Console.WriteLine($"- {airport.Name} ({airport.Iata})");
            }
        }

        [Test]
        public async Task GetAirport_ById_WhenAirportExists()
        {
            // Arrange
            AirportData airportData = new AirportData(this.connectionString);

            // Act
            Airport? airport = await airportData.GetByAirportId(1);
            // Assert
            Assert.That(airport, Is.Not.Null);
            Assert.That(airport.Iata, Is.EqualTo("SJO"));
            Assert.That(airport.Icao, Is.EqualTo("MROC"));
            Assert.That(airport.Name, Is.EqualTo("Aeropuerto Internacional Juan Santamaría"));


            System.Console.WriteLine($"- {airport.Name} ({airport.Iata})");

        }

        [Test]
        public async Task GetAirport_ById_WhenAirportDoesNotExist()
        {
            // Arrange
            AirportData airportData = new AirportData(this.connectionString);

            // Act
            Airport? airport = await airportData.GetByAirportId(5);
            // Assert
            Assert.That(airport, Is.Null);


        }
    }
}

