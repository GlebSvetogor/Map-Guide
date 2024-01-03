using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace ConsoleApp2
{
    internal class Model
    {
        public List<string> cities;

        public Tuple<string, string> resultRouteInfo;
        private MatrixCreator matrixCreator;
        private CoordinateMatrix coordinateMatrix;
        private Matrix matrix;

        public Model() 
        { 
            
        }

        public async Task<(int, string)> FindFastestRouteAsync()
        {
            coordinateMatrix = new CoordinateMatrix();

            matrixCreator = new TimeMatrixCreator();
            matrix = matrixCreator.CreateMatrix();

            var timeMatrix = matrix.Count(await coordinateMatrix.Count(cities));

            matrix.PrintMatrix(timeMatrix);

            var result = TSP.Start(timeMatrix);

            return CreateRouteResponse(timeMatrix, result);

        }

        public async Task<(int, string)> FindShortestRouteAsync()
        {
            coordinateMatrix = new CoordinateMatrix();

            matrixCreator = new DistanceMatrixCreator();
            matrix = matrixCreator.CreateMatrix();
            var distanceMatrix = matrix.Count(await coordinateMatrix.Count(cities));

            matrix.PrintMatrix(distanceMatrix);

            var result = TSP.Start(distanceMatrix);

            matrixCreator = new TimeMatrixCreator();
            matrix = matrixCreator.CreateMatrix();
            var timeMatrix = matrix.Count(await coordinateMatrix.Count(cities));

            int j = 0;
            int sum = 0;
            for(int i = 0; i < cities.Count; i++)
            {
                if(i == cities.Count - 1)
                {
                    break;
                }
                j = i + 1;
                Console.WriteLine($"Время между {result.Item1[i]} городом и {result.Item1[j]} городом = {timeMatrix[result.Item1[i], result.Item1[j]]}");
                sum += timeMatrix[result.Item1[i], result.Item1[j]];
            }

            matrix.PrintMatrix(timeMatrix);

            Console.WriteLine($"Время затраченное на самый короткий маршрут = {sum} ч.");

            return CreateRouteResponse(distanceMatrix, result); 
        }

        public (int, string) CreateRouteResponse(int[,] matrix, Tuple<int[], int> result)
        {
            foreach (var i in result.Item1)
            {
                Console.WriteLine(i + "\t");
            }

            StringBuilder routeStr = new StringBuilder();
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                if (i == matrix.GetLength(0) - 1)
                {
                    routeStr.Append(cities[result.Item1[i]]);
                }
                else
                {
                    routeStr.Append(cities[result.Item1[i]] + " -> ");
                }
            }

            cities.Clear();
            return (result.Item2, routeStr.ToString());
        }

    }
}
