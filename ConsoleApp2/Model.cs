using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

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

        public Model() { }

        public async Task<string> FindShortestRouteAsync()
        {
            coordinateMatrix = new CoordinateMatrix();

            matrixCreator = new DistanceMatrixCreator();
            matrix = matrixCreator.CreateMatrix();
            double[,] distanceMatrix = matrix.Count(coordinateMatrix.Count(cities));
            matrixCreator = new DurationMatrixCreator();
            matrix = matrixCreator.CreateMatrix();
            double[,] durationMatrix = matrix.Count(coordinateMatrix.Count(cities));

            matrix.PrintMatrix(distanceMatrix, "Матрица расстояний:");
            matrix.PrintMatrix(durationMatrix, "Матрица времени:");

            citiesIndexesRouteOrder = TSP.Start(distanceMatrix);

            Console.WriteLine("Результат был получен");

            routeResponseCreator = new RouteDistanceAndDurationResponseCreator();
            var routeResponse = routeResponseCreator.CreateRouteResponse();
            return routeResponse.CreateRouteResponse(distanceMatrix,durationMatrix, citiesIndexesRouteOrder, cities); 
        }

        public string MakeMapLink()
        {
            mapLinkCreator = new GoogleMapLinkCreator();
            MapLink googleMapLink = mapLinkCreator.CreateMapLink();
            return googleMapLink.CreateMapLink(citiesIndexesRouteOrder, coordinateMatrix.Count(cities));
        }

    }
}
