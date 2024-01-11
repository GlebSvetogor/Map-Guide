using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace ConsoleApp2
{
    abstract class Matrix
    {
        public double[,] matrix { get; protected set; }
        public APIRequest request { get; protected set; }
        public abstract double[,] Count(string[] citiesCoordinates);
        public abstract void fillMatrix(string[] coordinates, string requestUrl);
        public void PrintMatrix(double [,]matrix, string message)
        {
            Console.WriteLine(message);

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Console.Write((int)matrix[i, j] + "\t");
                }
                Console.WriteLine();
            }
        }

    }

    class DistanceMatrix : Matrix
    {
        public override double[,] Count(string[] citiesCoordinates)
        {
            matrix = new double[citiesCoordinates.Count(), citiesCoordinates.Count()];
            string url = "https://maps.googleapis.com/maps/api/directions/json?origin=coordinatesFrom&destination=coordinatesTo&key=AIzaSyBGykNf1-zcVrXeSSkuYqRc01Gc02nh0Ho";
            fillMatrix(citiesCoordinates,url);
            return matrix;
        }
        public override void fillMatrix(string[] coordinates, string requestUrl)
        {
            request = new APIRequest();
            string url = requestUrl;
            for (int i = 0; i < coordinates.Length; i++)
            {
                for (int j = 0; j < coordinates.Length; j++)
                {
                    if (i == j)
                    {
                        matrix[i, j] = 0;
                        continue;
                    };
                    url = requestUrl.Replace("coordinatesFrom", coordinates[i]).Replace("coordinatesTo", coordinates[j]);
                    double distance = request.MakeDistanceRequestAsync(url).Result;
                    matrix[i, j] = distance;
                }
            }
        }
    }

    class DurationMatrix : Matrix
    {
        public override double[,] Count(string[] citiesCoordinates)
        {
            matrix = new double[citiesCoordinates.Count(), citiesCoordinates.Count()];
            string url = "https://maps.googleapis.com/maps/api/directions/json?origin=coordinatesFrom&destination=coordinatesTo&key=AIzaSyBGykNf1-zcVrXeSSkuYqRc01Gc02nh0Ho";
            fillMatrix(citiesCoordinates, url);
            return matrix;
        }

        public override void fillMatrix(string[] coordinates, string requestUrl)
        {
            request = new APIRequest();
            string url = requestUrl;
            for (int i = 0; i < coordinates.Length; i++)
            {
                for (int j = 0; j < coordinates.Length; j++)
                {
                    if (i == j)
                    {
                        matrix[i, j] = 0;
                        continue;
                    };
                    url = requestUrl.Replace("coordinatesFrom", coordinates[i]).Replace("coordinatesTo", coordinates[j]);
                    double distance = request.MakeDurationRequestAsync(url).Result;
                    matrix[i, j] = distance;
                }
            }
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

    class DurationMatrixCreator : MatrixCreator
    {
        public override Matrix CreateMatrix() => new DurationMatrix();
    }

    class CoordinateMatrix
    {
        public string[] array;
        public string[] Count(List<City> cities)
        {
            array = new string[cities.Count];
            int i = 0;
            foreach (City city in cities)
            {
                array[i] = city.lat + ',' + city.lng;
                ++i;
            }
            return array;

        }
    }


}
