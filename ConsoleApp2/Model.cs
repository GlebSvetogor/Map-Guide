using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace ConsoleApp2
{
    internal class Model
    {
        public List<Root> cities;
        public Tuple<string, string> resultRouteInfo;
        private MatrixCreator matrixCreator;
        private CoordinateMatrix coordinateMatrix;
        private Matrix matrix;

        public Model() 
        { 
            
        }

        public async Task<string> FindShortestRouteAsync()
        {
            coordinateMatrix = new CoordinateMatrix();

            matrixCreator = new DistanceMatrixCreator();
            matrix = matrixCreator.CreateMatrix();
            var routeMatrixes = matrix.Count(await coordinateMatrix.Count(cities));
            double[,] distanceMatrix = routeMatrixes.Item1;
            double[,] durationMatrix = routeMatrixes.Item2;

            matrix.PrintMatrix(distanceMatrix, "Матрица расстояний:");
            matrix.PrintMatrix(durationMatrix, "Матрица времени:");

            var result = TSP.Start(distanceMatrix);

            return CreateRouteResponse(distanceMatrix,durationMatrix, result); 
        }

        public string CreateRouteResponse(double[,] distanceMatrix, double[,] durationMatrix, (int[], int) result)
        {

            StringBuilder routeStr = new StringBuilder();

            routeStr.Append("Маршрут: ");

            for (int i = 0; i < distanceMatrix.GetLength(0); i++)
            {
                if (i == distanceMatrix.GetLength(0) - 1)
                {
                    routeStr.Append(cities[result.Item1[i]].name);
                }
                else
                {
                    routeStr.Append(cities[result.Item1[i]].name + " -> ");
                }

                if(i == distanceMatrix.GetLength(0) - 1)
                {
                    routeStr.Append(" -> " + cities[result.Item1[0]].name);
                }
            }

            int j = 0;
            double fullTime = 0;
            for (int i = 0; i < distanceMatrix.GetLength(0); i++)
            {
                j = i + 1;

                if (j == distanceMatrix.GetLength(0))
                {
                    fullTime += durationMatrix[result.Item1[0], result.Item1[i]];
                    break;
                }

                fullTime += durationMatrix[result.Item1[i], result.Item1[j]];
            }

            routeStr.Append("\n" + "Кратчайший путь займет: " + (int)result.Item2 + "км. и " + (int)fullTime + "ч." + "\n\n");

            j = 0;
            for (int i = 0; i < distanceMatrix.GetLength(0); i++)
            {
                j = i + 1;

                if(j == distanceMatrix.GetLength(0))
                {
                    routeStr.Append($"Путь: {cities[result.Item1[i]].name} -> {cities[result.Item1[0]].name} займет {(int)distanceMatrix[result.Item1[0], result.Item1[i]]} км. и {(int)durationMatrix[result.Item1[0], result.Item1[i]]} ч. \n");
                    break;
                }

                routeStr.Append($"Путь: {cities[result.Item1[i]].name} -> {cities[result.Item1[j]].name} займет {(int)distanceMatrix[result.Item1[i], result.Item1[j]]} км. и {(int)durationMatrix[result.Item1[i], result.Item1[j]]} ч. \n");
            }

            cities.Clear();
            return routeStr.ToString();
        }

    }
}
