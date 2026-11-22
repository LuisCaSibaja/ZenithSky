using ZenithSky.Data;
using ZenithSky.Domain;
using ZenithSky.Dto;

namespace ZenithSky.Test.Data
{
    [TestFixture]
    public class AirplaneDataTest
    {
        private string connectionString;

        [SetUp]
        public void Setup()
        {
            this.connectionString = "Data Source=163.178.173.130;Initial Catalog=ZenithSky_Proyecto_JEDL;User ID=Lenguajes;Password=lenguajesparaiso2025;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False;Command Timeout=300";
        }

        [Test]
        public async Task GetAll_IncluyeTipoYAerolinea_Exitoso()
        {
            // Arrange
            AirplaneData airplaneData = new AirplaneData(this.connectionString);

            // Act
            IEnumerable<Airplane> airplanes = await airplaneData.GetAll();

            // Assert
            Assert.That(airplanes.Count(), Is.GreaterThanOrEqualTo(6));
            Assert.That(airplanes.First().AirplaneType, Is.Not.Null);
            Assert.That(airplanes.First().Airline, Is.Not.Null);
            Assert.That(airplanes.First().AirplaneType.Identifier, Is.EqualTo("A320"));
            Assert.That(airplanes.First().Airline.AirlineName, Is.EqualTo("ZenithSky Airlines"));

            System.Console.WriteLine("Aviones encontrados:");
            foreach (var airplane in airplanes)
            {
                System.Console.WriteLine($"- #{airplane.AirplaneId} {airplane.AirplaneType.Identifier} ({airplane.Capacity} pax) - {airplane.Airline.AirlineName}");
            }
        }

        [Test]
        public async Task GetAll_FiltroPorTipo_Exitoso()
        {
            // Arrange
            AirplaneData airplaneData = new AirplaneData(this.connectionString);

            // Act
            IEnumerable<Airplane> airplanes = await airplaneData.GetAll(typeId: 2);

            // Assert
            Assert.That(airplanes.Count(), Is.GreaterThanOrEqualTo(2));
            Assert.That(airplanes.All(a => a.AirplaneType.TypeId == 2), Is.True);

            System.Console.WriteLine("Aviones del tipo 2:");
            foreach (var airplane in airplanes)
            {
                System.Console.WriteLine($"- #{airplane.AirplaneId} {airplane.AirplaneType.Identifier} - {airplane.Airline.AirlineName}");
            }
        }

        [Test]
        public async Task GetAll_FiltroPorAerolinea_Exitoso()
        {
            // Arrange
            AirplaneData airplaneData = new AirplaneData(this.connectionString);

            // Act
            IEnumerable<Airplane> airplanes = await airplaneData.GetAll(airlineId: 1);

            // Assert
            Assert.That(airplanes.Count(), Is.GreaterThanOrEqualTo(3));
            Assert.That(airplanes.All(a => a.Airline.AirlineId == 1), Is.True);

            System.Console.WriteLine("Aviones de la aerolínea 1:");
            foreach (var airplane in airplanes)
            {
                System.Console.WriteLine($"- #{airplane.AirplaneId} {airplane.AirplaneType.Identifier} - {airplane.Airline.AirlineName}");
            }
        }

        [Test]
        public async Task GetAll_FiltroPorTipoYAerolinea_Exitoso()
        {
            // Arrange
            AirplaneData airplaneData = new AirplaneData(this.connectionString);

            // Act
            IEnumerable<Airplane> airplanes = await airplaneData.GetAll(typeId: 1, airlineId: 2);

            // Assert
            Assert.That(airplanes.Count(), Is.GreaterThanOrEqualTo(1));
            Assert.That(airplanes.All(a => a.AirplaneType.TypeId == 1 && a.Airline.AirlineId == 2), Is.True);
        }

        [Test]
        public async Task GetAll_FiltroSinResultados()
        {
            AirplaneData airplaneData = new AirplaneData(this.connectionString);
            IEnumerable<Airplane> airplanes = await airplaneData.GetAll(typeId: 99999);
            Assert.That(airplanes, Is.Empty);
        }

        [Test]
        public async Task GetById_WhenAirplaneExists()
        {
            // Arrange
            AirplaneData airplaneData = new AirplaneData(this.connectionString);

            // Act
            Airplane? airplane = await airplaneData.GetById(1);

            // Assert
            Assert.That(airplane, Is.Not.Null);
            Assert.That(airplane.Capacity, Is.EqualTo(186));
            Assert.That(airplane.AirplaneType.TypeId, Is.EqualTo(2));
            Assert.That(airplane.AirplaneType.Identifier, Is.EqualTo("A320"));
            Assert.That(airplane.Airline.AirlineId, Is.EqualTo(1));
            Assert.That(airplane.Airline.Iata, Is.EqualTo("Z3"));
            Assert.That(airplane.Airline.BaseAirport.Iata, Is.EqualTo("SJO"));

            System.Console.WriteLine($"- #{airplane.AirplaneId} {airplane.AirplaneType.Identifier} ({airplane.Capacity} pax) - {airplane.Airline.AirlineName}");
        }

        [Test]
        public async Task GetById_WhenAirplaneDoesNotExist()
        {
            AirplaneData airplaneData = new AirplaneData(this.connectionString);
            Airplane? airplane = await airplaneData.GetById(99999);
            Assert.That(airplane, Is.Null);
        }

        //Validaciones

        [Test]
        public async Task AirplaneTypeExists_WhenTypeExists()
        {
            AirplaneData airplaneData = new AirplaneData(this.connectionString);
            bool exists = await airplaneData.AirplaneTypeExists(1);
            Assert.That(exists, Is.True);
        }

        [Test]
        public async Task AirplaneTypeExists_WhenTypeDoesNotExist()
        {
            AirplaneData airplaneData = new AirplaneData(this.connectionString);
            bool exists = await airplaneData.AirplaneTypeExists(99999);
            Assert.That(exists, Is.False);
        }

        [Test]
        public async Task AirlineExists_WhenAirlineExists()
        {
            AirplaneData airplaneData = new AirplaneData(this.connectionString);
            bool exists = await airplaneData.AirlineExists(1);
            Assert.That(exists, Is.True);
        }

        [Test]
        public async Task AirlineExists_WhenAirlineDoesNotExist()
        {
            AirplaneData airplaneData = new AirplaneData(this.connectionString);
            bool exists = await airplaneData.AirlineExists(99999);
            Assert.That(exists, Is.False);
        }

        [Test]
        public async Task Insert_Exitoso()
        {
            // Arrange
            AirplaneData airplaneData = new AirplaneData(this.connectionString);
            var request = new AirplaneDTO { Capacity = 150, TypeId = 3, AirlineId = 3 };
            int newId = 0;

            try
            {
                // Act
                newId = await airplaneData.Insert(request);
                Airplane? created = await airplaneData.GetById(newId);

                // Assert
                Assert.That(newId, Is.GreaterThan(0));
                Assert.That(created, Is.Not.Null);
                Assert.That(created.Capacity, Is.EqualTo(150));
                Assert.That(created.AirplaneType.TypeId, Is.EqualTo(3));
                Assert.That(created.Airline.AirlineId, Is.EqualTo(3));

                System.Console.WriteLine($"Avión creado con ID {newId}");
            }
            finally
            {
                if (newId > 0)
                    await airplaneData.Delete(newId);
            }
        }

        [Test]
        public async Task Update_Exitoso()
        {
            // Arrange
            AirplaneData airplaneData = new AirplaneData(this.connectionString);
            int newId = await airplaneData.Insert(
                new AirplaneDTO { Capacity = 150, TypeId = 3, AirlineId = 3 });

            try
            {
                var changes = new AirplaneDTO { Capacity = 300, TypeId = 3, AirlineId = 3 };

                // Act
                bool updated = await airplaneData.Update(newId, changes);
                Airplane? airplane = await airplaneData.GetById(newId);

                // Assert
                Assert.That(updated, Is.True);
                Assert.That(airplane, Is.Not.Null);
                Assert.That(airplane.Capacity, Is.EqualTo(300));
                Assert.That(airplane.AirplaneType.TypeId, Is.EqualTo(3));
                Assert.That(airplane.Airline.AirlineId, Is.EqualTo(3));
            }
            finally
            {
                await airplaneData.Delete(newId);
            }
        }

        [Test]
        public async Task Update_WhenAirplaneDoesNotExist()
        {
            AirplaneData airplaneData = new AirplaneData(this.connectionString);
            var changes = new AirplaneDTO { Capacity = 150, TypeId = 3, AirlineId = 3 };
            bool updated = await airplaneData.Update(99999, changes);
            Assert.That(updated, Is.False);
        }

        [Test]
        public async Task Delete_Exitoso()
        {
            // Arrange
            AirplaneData airplaneData = new AirplaneData(this.connectionString);
            int newId = await airplaneData.Insert(
                new AirplaneDTO { Capacity = 150, TypeId = 3, AirlineId = 3 });

            // Act
            bool deleted = await airplaneData.Delete(newId);
            Airplane? airplane = await airplaneData.GetById(newId);

            // Assert
            Assert.That(deleted, Is.True);
            Assert.That(airplane, Is.Null);
        }

        [Test]
        public async Task Delete_WhenAirplaneDoesNotExist()
        {
            AirplaneData airplaneData = new AirplaneData(this.connectionString);
            bool deleted = await airplaneData.Delete(99999);
            Assert.That(deleted, Is.False);
        }
    }
}