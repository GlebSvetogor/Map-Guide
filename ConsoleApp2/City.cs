using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp2
{
    class City
    {

        public string longName { get; set; }

        public string lat { get; private set; }
        public string lng { get; private set; }

        public City(string longName, string lat, string lng)
        {
            this.longName = longName;
            this.lat = lat;
            this.lng = lng;
        }
    }
    
}
