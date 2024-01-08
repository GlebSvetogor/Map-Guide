using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{

    abstract class MapLink
    {
        public abstract string CreateMapLink(int[] citiesRouteOrder, string[] citiesCoordinatesArray);
    }

    class GoogleMapLink : MapLink
    {
        public override string CreateMapLink(int[] citiesIndexesRouteOrder, string[] citiesCoordinatesArray) 
        {

            string googleMapsUrl = $"https://www.google.com/maps/dir/?api=1&destination={citiesCoordinatesArray[citiesIndexesRouteOrder[0]]}&origin={citiesCoordinatesArray[citiesIndexesRouteOrder[0]]}&travelmode=driving&waypoints=wayPointsCoordinates&dir_action=navigate";
            var waypoints = new StringBuilder();
            for (int i = 1; i < citiesIndexesRouteOrder.Length; i++)
            {
                if (i == citiesIndexesRouteOrder.Length - 1)
                {
                    waypoints.Append(citiesCoordinatesArray[citiesIndexesRouteOrder[i]]);
                    break;
                }
                waypoints.Append(citiesCoordinatesArray[citiesIndexesRouteOrder[i]] + "|");
            }
            string url = googleMapsUrl.Replace("wayPointsCoordinates", waypoints.ToString());
            return url;
        }
    }

    abstract class MapLinkCreator
    {
        public abstract MapLink CreateMapLink();
    }

    class GoogleMapLinkCreator : MapLinkCreator
    {
        public override MapLink CreateMapLink() { return new GoogleMapLink(); }
    }

    

}
