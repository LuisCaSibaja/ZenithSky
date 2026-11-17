using ZenithSky.Data;
using ZenithSky.Domain;

namespace ZenithSky.Test.Data
{
    [TestFixture]
    public class AirlineDataTest
    {
        private string connectionString;

        [SetUp]
        public void Setup()
        {
            this.connectionString = "Data Source=163.178.173.130;Initial Catalog=ZenithSky_Proyecto_JEDL;User ID=Lenguajes;Password=lenguajesparaiso2025;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False;Command Timeout=300";
        }

        [Test]
        public async Task GetAll_IncluyeAeropuertoBase_Exitoso()
        {
            // Arrange
            AirlineData airlineData = new AirlineData(this.connectionString);

            // Act
            IEnumerable<Airline> airlines = await airlineData.GetAll();

            // Assert
            Assert.That(airlines.Count(), Is.GreaterThanOrEqualTo(3));
            Assert.That(airlines.First().BaseAirport, Is.Not.Null);
            Assert.That(airlines.First().BaseAirport.Iata, Is.EqualTo("SJO"));

            System.Console.WriteLine("Aerolíneas encontradas:");
            foreach (var airline in airlines)
            {
                System.Console.WriteLine($"- {airline.AirlineName} ({airline.Iata}) - Base: {airline.BaseAirport.Name} ({airline.BaseAirport.Iata})");
            }
        }

        [Test]
        public async Task GetById_WhenAirlineExists()
        {
            // Arrange
            AirlineData airlineData = new AirlineData(this.connectionString);

            // Act
            Airline? airline = await airlineData.GetById(1);

            // Assert
            Assert.That(airline, Is.Not.Null);
            Assert.That(airline.Iata, Is.EqualTo("ZS"));
            Assert.That(airline.AirlineName, Is.EqualTo("ZenithSky Airlines"));
            Assert.That(airline.BaseAirport.AirportId, Is.EqualTo(1));
            Assert.That(airline.BaseAirport.Iata, Is.EqualTo("SJO"));

            System.Console.WriteLine($"- {airline.AirlineName} ({airline.Iata}) - Base: {airline.BaseAirport.Name}");
        }

        [Test]
        public async Task GetById_WhenAirlineDoesNotExist()
        {
            // Arrange
            AirlineData airlineData = new AirlineData(this.connectionString);

            // Act
            Airline? airline = await airlineData.GetById(99999);

            // Assert
            Assert.That(airline, Is.Null);
        }

        //Validaciones

        [Test]
        public async Task AirportExists_WhenAirportExists()
        {
            AirlineData airlineData = new AirlineData(this.connectionString);
            bool exists = await airlineData.AirportExists(1);
            Assert.That(exists, Is.True);
        }

        [Test]
        public async Task AirportExists_WhenAirportDoesNotExist()
        {
            AirlineData airlineData = new AirlineData(this.connectionString);
            bool exists = await airlineData.AirportExists(99999);
            Assert.That(exists, Is.False);
        }

        [Test]
        public async Task HasAirplanes_WhenAirlineHasAirplanes()
        {
            AirlineData airlineData = new AirlineData(this.connectionString);
            bool hasAirplanes = await airlineData.HasAirplanes(1);
            Assert.That(hasAirplanes, Is.True);
        }

        [Test]
        public async Task HasAirplanes_WhenAirlineHasNoAirplanes()
        {
            AirlineData airlineData = new AirlineData(this.connectionString);
            bool hasAirplanes = await airlineData.HasAirplanes(3);
            Assert.That(hasAirplanes, Is.False);
        }

        [Test]
        public async Task Insert_Exitoso()
        {
            // Arrange
            AirlineData airlineData = new AirlineData(this.connectionString);
            var request = new AirlineRequest { Iata = "TT", Airlinename = "Test Insert", BaseAirport = 4 };
            int newId = 0;

            try
            {
                // Act
                newId = await airlineData.Insert(request);
                Airline? created = await airlineData.GetById(newId);

                // Assert
                Assert.That(newId, Is.GreaterThan(0));
                Assert.That(created, Is.Not.Null);
                Assert.That(created.Iata, Is.EqualTo("TT"));
                Assert.That(created.AirlineName, Is.EqualTo("Test Insert"));
                Assert.That(created.BaseAirport.AirportId, Is.EqualTo(4));

                System.Console.WriteLine($"Aerolínea creada con ID {newId}");
            }
            finally
            {
                if (newId > 0)
                    await airlineData.Delete(newId);
            }
        }

        [Test]
        public async Task Update_Exitoso()
        {
            // Arrange
            AirlineData airlineData = new AirlineData(this.connectionString);
            int newId = await airlineData.Insert(
                new AirlineRequest { Iata = "TT", Airlinename = "Test Update", BaseAirport = 4 });

            try
            {
                var changes = new AirlineRequest { Iata = "TU", Airlinename = "Test Update Editada", BaseAirport = 1 };

                // Act
                bool updated = await airlineData.Update(newId, changes);
                Airline? airline = await airlineData.GetById(newId);

                // Assert
                Assert.That(updated, Is.True);
                Assert.That(airline, Is.Not.Null);
                Assert.That(airline.Iata, Is.EqualTo("TU"));
                Assert.That(airline.AirlineName, Is.EqualTo("Test Update Editada"));
                Assert.That(airline.BaseAirport.AirportId, Is.EqualTo(1));
            }
            finally
            {
                await airlineData.Delete(newId);
            }
        }

        [Test]
        public async Task Update_WhenAirlineDoesNotExist()
        {
            AirlineData airlineData = new AirlineData(this.connectionString);
            var changes = new AirlineRequest { Iata = "TT", Airlinename = "No existe", BaseAirport = 1 };
            bool updated = await airlineData.Update(99999, changes);
            Assert.That(updated, Is.False);
        }

        [Test]
        public async Task Delete_Exitoso()
        {
            // Arrange
            AirlineData airlineData = new AirlineData(this.connectionString);
            int newId = await airlineData.Insert(
                new AirlineRequest { Iata = "TT", Airlinename = "Test Delete", BaseAirport = 4 });

            // Act
            bool deleted = await airlineData.Delete(newId);
            Airline? airline = await airlineData.GetById(newId);

            // Assert
            Assert.That(deleted, Is.True);
            Assert.That(airline, Is.Null);
        }

        [Test]
        public async Task Delete_WhenAirlineDoesNotExist()
        {
            AirlineData airlineData = new AirlineData(this.connectionString);
            bool deleted = await airlineData.Delete(99999);
            Assert.That(deleted, Is.False);
        }
    }
}