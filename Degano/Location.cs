using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Degano
{
    public struct Location
    {
        public double lat { set; get; }
        public double lng { set; get; }

        public Location(double _lat, double _lng)
        {
            lat = _lat;
            lng = _lng;
        }
    }
}
