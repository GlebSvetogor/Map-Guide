using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using static System.Net.WebRequestMethods;

namespace ConsoleApp2
{
    internal class Model
    {
        public List<Root> cities;
        public Tuple<string, string> resultRouteInfo;
        private MatrixCreator matrixCreator;
        public CoordinateMatrix coordinateMatrix;
        private Matrix matrix;
        private RouteResponseCreator routeResponseCreator;
        public int[] citiesIndexesRouteOrder;

        public Model() { }

        public async Task<string> FindShortestRouteAsync()
        {
            coordinateMatrix = new CoordinateMatrix();

            matrixCreator = new DistanceMatrixCreator();
            matrix = matrixCreator.CreateMatrix();
            var routeMatrixes = matrix.Count(coordinateMatrix.Count(cities));
            double[,] distanceMatrix = routeMatrixes.Item1;
            double[,] durationMatrix = routeMatrixes.Item2;

            matrix.PrintMatrix(distanceMatrix, "Матрица расстояний:");
            matrix.PrintMatrix(durationMatrix, "Матрица времени:");

            citiesIndexesRouteOrder = TSP.Start(distanceMatrix);

            Console.WriteLine("Результат был получен");

            routeResponseCreator = new RouteDistanceAndDurationResponseCreator();
            var routeResponse = routeResponseCreator.CreateRouteResponse();
            return routeResponse.CreateRouteResponse(distanceMatrix,durationMatrix, citiesIndexesRouteOrder, cities); 
        }

    }
}
