using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp2
{
    abstract class RouteResponse
    {
        public abstract string CreateRouteResponse(double[,] distanceMatrix, double[,] durationMatrix, int[] citiesIndexOrderArray, List<Root> cities);
        
    }

    class RouteDistanceAndDurationResponse : RouteResponse
    {
        private double[,] distanceMatrix;
        private double[,] durationMatrix;
        List<Root> cities;
        int[] citiesIndexOrderArray;
        public override string CreateRouteResponse(double[,] distanceMatrix, double[,] durationMatrix, int[] citiesIndexOrderArray, List<Root> cities)
        {
            this.distanceMatrix = distanceMatrix;
            this.durationMatrix = distanceMatrix;
            this.cities = cities;
            this.citiesIndexOrderArray = citiesIndexOrderArray;

            StringBuilder fullResponse = new StringBuilder();
            fullResponse.Append(CreateRouteStr(distanceMatrix));

            double fullDistance = CountFullExpenses(distanceMatrix);
            double fullDuration = CountFullExpenses(durationMatrix);

            fullResponse.Append("\n" + "Кратчайший путь займет: " + (int)fullDistance + "км. и " + (int)fullDuration + "ч." + "\n\n");

            fullResponse.Append(CreateDetailedRoute());

            return fullResponse.ToString();
            
        }

        public string CreateRouteStr(double[,] matrix)
        {
            StringBuilder routeStr = new StringBuilder();

            routeStr.Append("Маршрут: ");

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                if (i == matrix.GetLength(0) - 1)
                {
                    routeStr.Append(cities[citiesIndexOrderArray[i]].name);
                }
                else
                {
                    routeStr.Append(cities[citiesIndexOrderArray[i]].name + " -> ");
                }

                if (i == distanceMatrix.GetLength(0) - 1)
                {
                    routeStr.Append(" -> " + cities[citiesIndexOrderArray[0]].name);
                }
            }

            return routeStr.ToString();
        }

        public double CountFullExpenses(double[,] matrix)
        {
            int j = 0;
            double fullExpenses = 0;
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                j = i + 1;

                if (j == matrix.GetLength(0))
                {
                    fullExpenses += matrix[citiesIndexOrderArray[0], citiesIndexOrderArray[i]];
                    break;
                }

                fullExpenses += matrix[citiesIndexOrderArray[i], citiesIndexOrderArray[j]];
            }
            return fullExpenses;
        }

        public string CreateDetailedRoute()
        {
            int j = 0;
            StringBuilder detailedRoute = new StringBuilder();
            for (int i = 0; i < distanceMatrix.GetLength(0); i++)
            {
                j = i + 1;

                if (j == distanceMatrix.GetLength(0))
                {
                    detailedRoute.Append($"Путь: {cities[citiesIndexOrderArray[i]].name} -> {cities[citiesIndexOrderArray[0]].name} займет {(int)distanceMatrix[citiesIndexOrderArray[0], citiesIndexOrderArray[i]]} км. и {(int)durationMatrix[citiesIndexOrderArray[0], citiesIndexOrderArray[i]]} ч. \n");
                    break;
                }

                detailedRoute.Append($"Путь: {cities[citiesIndexOrderArray[i]].name} -> {cities[citiesIndexOrderArray[j]].name} займет {(int)distanceMatrix[citiesIndexOrderArray[i], citiesIndexOrderArray[j]]} км. и {(int)durationMatrix[citiesIndexOrderArray[i], citiesIndexOrderArray[j]]} ч. \n");
            }

            return detailedRoute.ToString();
        }
    }

    abstract class RouteResponseCreator
    {
        public abstract RouteResponse CreateRouteResponse();
    }

    class RouteDistanceAndDurationResponseCreator : RouteResponseCreator
    {
        public override RouteResponse CreateRouteResponse() { return new RouteDistanceAndDurationResponse(); }
    }
}
