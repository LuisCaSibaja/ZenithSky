using System;
using System.Collections.Generic;
using System.Text;

namespace ZenithSky.Domain
{
    public class AirplaneType
    {
        private int typeId;
        private string identifier;
        private string description;

        public AirplaneType()
        {

        }

        public AirplaneType(int typeId, string identifier, string description)
        {
            this.typeId = typeId;
            this.identifier = identifier;
            this.description = description;
        }

        public int TypeId { get => typeId; set => typeId = value; }
        public string Identifier { get => identifier; set => identifier = value; }
        public string Description { get => description; set => description = value; }
    }
}
