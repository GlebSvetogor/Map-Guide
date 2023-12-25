using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    abstract class Matrix
    {
        public int[,] matrix;
        public abstract int[,] Count(List<string> cities);
    }

    class DistanceMatrix : Matrix
    {
        public override int[,] Count(List<string> cities)
        {
            matrix = new int[cities.Count, cities.Count];
            for (int i = 0; i < cities.Count; i++)
            {
                for (int j = 0; j < cities.Count; j++)
                {
                    if (i == j)
                    {
                        matrix[i, j] = 0;
                        continue;
                    }
                    matrix[i, j] = CoordinatesRequest(cities[i], cities[j]).Result;
                }
            }
            Console.WriteLine("Output from distance matric:");

            for (int i = 0; i < cities.Count; i++)
            {
                for (int j = 0; j < cities.Count; j++)
                {
                    Console.WriteLine($"distance between {cities[i]} and {cities[j]} = {matrix[i, j]}");
                }
            }
            return matrix;
        }

        static async Task<int> CoordinatesRequest(string city1, string city2)
        {
            JObject json1 = await APIRequest.MakeRequest(city1);
            JObject json2 = await APIRequest.MakeRequest(city2);
            JArray geonames1 = (JArray)json1["geonames"];
            JArray geonames2 = (JArray)json2["geonames"];
            double lat1 = (double)geonames1[0]["lat"];
            double lon1 = (double)geonames1[0]["lng"];
            double lat2 = (double)geonames2[0]["lat"];
            double lon2 = (double)geonames2[0]["lng"];
            int distance = (int)CountDistance(lat1, lon1, lat2, lon2);

            return distance;

        }

        private static double CountDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double R = 6371; // Radius of the earth in km
            double dLat = Deg2Rad(lat2 - lat1);  // deg2rad below
            double dLon = Deg2Rad(lon2 - lon1);
            double a =
              Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
              Math.Cos(Deg2Rad(lat1)) * Math.Cos(Deg2Rad(lat2)) *
              Math.Sin(dLon / 2) * Math.Sin(dLon / 2)
              ;
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double d = R * c; // Distance in km
            return d;
        }

        private static double Deg2Rad(double deg)
        {
            return deg * (Math.PI / 180);
        }
    }

    class TimeMatrix : Matrix
    {
        public override int[,] Count(List<string> cities)
        {
            return matrix;
        }
    }

    abstract class MatrixCreator
    {
        public abstract Matrix CreateMatrix();
    }

    class DistanceMatrixCreator : MatrixCreator
    {
        public override Matrix CreateMatrix() => new DistanceMatrix();
    }

    class TimeMatrixCreator : MatrixCreator
    {
        public override Matrix CreateMatrix() => new TimeMatrix();
    }
}
