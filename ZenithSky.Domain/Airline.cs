using System;
using System.Collections.Generic;
using System.Text;

namespace ZenithSky.Domain
{
    public class Airline
    {
        private int airlineId;
        private string iata;
        private string airlineName;
        private Airport baseAirport;

        public Airline()
        {

        }

        public Airline(int airlineId, string iata, string airlineName, Airport baseAirport)
        {
            this.airlineId = airlineId;
            this.iata = iata;
            this.airlineName = airlineName;
            this.baseAirport = baseAirport;
        }

        public int AirlineId { get => airlineId; set => airlineId = value; }
        public string Iata { get => iata; set => iata = value; }

        public string AirlineName { get => airlineName; set => airlineName = value; }
        public Airport BaseAirport { get => baseAirport; set => baseAirport = value; }
    }
}
