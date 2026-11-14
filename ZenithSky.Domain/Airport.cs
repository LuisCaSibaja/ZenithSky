using System;
using System.Collections.Generic;
using System.Text;

namespace ZenithSky.Domain
{
    public class Airport
    {
        private int airportId;
        private string iata;
        private string icao;
        private string name;

        public Airport()
        {

        }

        public Airport(int airportId, string iata, string icao, string name)
        {
            this.airportId = airportId;
            this.iata = iata;
            this.icao = icao;
            this.name = name;
        }

        public int AirportId { get => airportId; set => airportId = value; }
        public string Iata { get => iata; set => iata = value; }
        public string Icao { get => icao; set => icao = value; }
        public string Name { get => name; set => name = value; }
    }
}

