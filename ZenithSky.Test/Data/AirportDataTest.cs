using ZenithSky.Data;
using ZenithSky.Domain;
using ZenithSky.Dto;

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
            Assert.That(airport.Icao, Is.EqualTo("CASS"));
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

        //Validaciones

        [Test]
        public async Task IataExists_WhenIataIsDuplicated()
        {
            AirportData airportData = new AirportData(this.connectionString);
            bool exists = await airportData.IataExists("SJO");
            Assert.That(exists, Is.True);
        }

        [Test]
        public async Task IataExists_WhenIataIsNew()
        {
            AirportData airportData = new AirportData(this.connectionString);
            bool exists = await airportData.IataExists("QQQ");
            Assert.That(exists, Is.False);
        }

        [Test]
        public async Task IataExists_ExcluyeAlMismoAeropuerto()
        {
            // En el PUT, el aeropuerto 1 puede conservar su propio IATA
            AirportData airportData = new AirportData(this.connectionString);
            bool exists = await airportData.IataExists("SJO", 1);
            Assert.That(exists, Is.False);
        }

        [Test]
        public async Task IcaoExists_WhenIcaoIsDuplicated()
        {
            AirportData airportData = new AirportData(this.connectionString);
            bool exists = await airportData.IcaoExists("CASS");
            Assert.That(exists, Is.True);
        }

        [Test]
        public async Task IcaoExists_WhenIcaoIsNew()
        {
            AirportData airportData = new AirportData(this.connectionString);
            bool exists = await airportData.IcaoExists("QQQQ");
            Assert.That(exists, Is.False);
        }

        [Test]
        public async Task IcaoExists_ExcluyeAlMismoAeropuerto()
        {
            AirportData airportData = new AirportData(this.connectionString);
            bool exists = await airportData.IcaoExists("MROC", 1);
            Assert.That(exists, Is.False);
        }

        [Test]
        public async Task IsBaseAirport_WhenAirportIsBaseOfAnAirline()
        {
            // El aeropuerto 1 (SJO) es base de ZenithSky Airlines -> DELETE debe dar 409
            AirportData airportData = new AirportData(this.connectionString);
            bool isBase = await airportData.IsBaseAirport(1);
            Assert.That(isBase, Is.True);
        }

        [Test]
        public async Task IsBaseAirport_WhenAirportIsNotBase()
        {
            // El aeropuerto 4 (LIR) no es base de ninguna aerolínea -> se puede eliminar
            AirportData airportData = new AirportData(this.connectionString);
            bool isBase = await airportData.IsBaseAirport(4);
            Assert.That(isBase, Is.False);
        }

        [Test]
        public async Task Insert_Exitoso()
        {
            // Arrange
            AirportData airportData = new AirportData(this.connectionString);
            var request = new AirportDTO { Iata = "QQQ", Icao = "QQQQ", Name = "Test Insert" };
            int newId = 0;

            try
            {
                // Act
                newId = await airportData.Insert(request);
                Airport? created = await airportData.GetByAirportId(newId);

                // Assert
                Assert.That(newId, Is.GreaterThan(0));
                Assert.That(created, Is.Not.Null);
                Assert.That(created.Iata, Is.EqualTo("QQQ"));
                Assert.That(created.Icao, Is.EqualTo("QQQQ"));
                Assert.That(created.Name, Is.EqualTo("Test Insert"));

                System.Console.WriteLine($"Aeropuerto creado con ID {newId}");
            }
            finally
            {
                if (newId > 0)
                    await airportData.Delete(newId);
            }
        }

        [Test]
        public async Task Update_Exitoso()
        {
            // Arrange
            AirportData airportData = new AirportData(this.connectionString);
            int newId = await airportData.Insert(
                new AirportDTO { Iata = "QQQ", Icao = "QQQQ", Name = "Test Update" });

            try
            {
                var changes = new AirportDTO { Iata = "QQW", Icao = "QQQW", Name = "Test Update Editado" };

                // Act
                bool updated = await airportData.Update(newId, changes);
                Airport? airport = await airportData.GetByAirportId(newId);

                // Assert
                Assert.That(updated, Is.True);
                Assert.That(airport, Is.Not.Null);
                Assert.That(airport.Iata, Is.EqualTo("QQW"));
                Assert.That(airport.Icao, Is.EqualTo("QQQW"));
                Assert.That(airport.Name, Is.EqualTo("Test Update Editado"));
            }
            finally
            {
                await airportData.Delete(newId);
            }
        }

        [Test]
        public async Task Update_WhenAirportDoesNotExist()
        {
            AirportData airportData = new AirportData(this.connectionString);
            var changes = new AirportDTO { Iata = "QQQ", Icao = "QQQQ", Name = "No existe" };
            bool updated = await airportData.Update(99999, changes);
            Assert.That(updated, Is.False);
        }

        [Test]
        public async Task Delete_Exitoso()
        {
            // Arrange
            AirportData airportData = new AirportData(this.connectionString);
            int newId = await airportData.Insert(
                new AirportDTO { Iata = "QQQ", Icao = "QQQQ", Name = "Test Delete" });

            // Act
            bool deleted = await airportData.Delete(newId);
            Airport? airport = await airportData.GetByAirportId(newId);

            // Assert
            Assert.That(deleted, Is.True);
            Assert.That(airport, Is.Null);
        }

        [Test]
        public async Task Delete_WhenAirportDoesNotExist()
        {
            AirportData airportData = new AirportData(this.connectionString);
            bool deleted = await airportData.Delete(99999);
            Assert.That(deleted, Is.False);
        }
    }
}

