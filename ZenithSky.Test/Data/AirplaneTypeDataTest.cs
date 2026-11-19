using ZenithSky.Data;
using ZenithSky.Domain;

namespace ZenithSky.Test.Data
{
    [TestFixture]
    public class AirplaneTypeDataTest
    {
        private string connectionString;

        [SetUp]
        public void Setup()
        {
            this.connectionString = "Data Source=163.178.173.130;Initial Catalog=ZenithSky_Proyecto_JEDL;User ID=Lenguajes;Password=lenguajesparaiso2025;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False;Command Timeout=300";

        }

        [Test]
        public async Task GetAirplaneTypes_Exitoso()
        {
            // Arrange
            AirplaneTypeData airplaneTypeData = new AirplaneTypeData(this.connectionString);

            // Act
            IEnumerable<AirplaneType> airplaneTypes = await airplaneTypeData.GetAirplaneTypes();
            // Assert
            Assert.That(airplaneTypes.Count(), Is.EqualTo(4));

            System.Console.WriteLine("Tipos de avión encontrados:");
            foreach (var airplaneType in airplaneTypes)
            {
                System.Console.WriteLine($"- {airplaneType.TypeId} {airplaneType.Identifier} ({airplaneType.Description})");
            }
        }
    }
}
