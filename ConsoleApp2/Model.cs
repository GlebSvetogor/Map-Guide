using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using static System.Net.WebRequestMethods;

namespace ConsoleApp2
{
    internal class Model
    {
        public MapLinkCreator mapLinkCreator;
        public MatrixCreator matrixCreator;
        public List<City> cities;
        public CoordinateMatrix coordinateMatrix;
        private RouteResponseCreator routeResponseCreator;
        private Matrix matrix;
        public int[] citiesIndexesRouteOrder;
        public string url;

        public Model() { }

        public async Task<string> FindShortestRouteAsync()
        {
            coordinateMatrix = new CoordinateMatrix();

            matrixCreator = new DistanceMatrixCreator();
            matrix = matrixCreator.CreateMatrix();
            matrix.url = url;
            try
            {
                double[,] distanceMatrix = matrix.Count(coordinateMatrix.Count(cities));
                if (distanceMatrix.Cast<double>().Contains(-1))
                {
                    return "Не удалось найти маршрут между городами, попробуйте ввести другой маршрут ...";
                }
                matrixCreator = new DurationMatrixCreator();
                matrix = matrixCreator.CreateMatrix();
                matrix.url = url;
                double[,] durationMatrix = matrix.Count(coordinateMatrix.Count(cities));

                matrix.PrintMatrix(distanceMatrix, "Матрица расстояний:");
                matrix.PrintMatrix(durationMatrix, "Матрица времени:");

                citiesIndexesRouteOrder = TSP.Start(distanceMatrix);

                Console.WriteLine("Результат был получен");

                routeResponseCreator = new RouteDistanceAndDurationResponseCreator();
                var routeResponse = routeResponseCreator.CreateRouteResponse();
                return routeResponse.CreateRouteResponse(distanceMatrix,durationMatrix, citiesIndexesRouteOrder, cities); 
            }catch(NullReferenceException ex)
            {
                Console.WriteLine(ex.Message); return "Не удалось найти маршрут, возможно вы ввели неправильные названия городов или такого маршрута не существует,попробуйте ввсти другой маршрут";
            }
        }

        public string MakeMapLink()
        {
            mapLinkCreator = new GoogleMapLinkCreator();
            MapLink googleMapLink = mapLinkCreator.CreateMapLink();
            return googleMapLink.CreateMapLink(citiesIndexesRouteOrder, coordinateMatrix.Count(cities));
        }

    }
}
