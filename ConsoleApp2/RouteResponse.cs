using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp2
{
    abstract class RouteResponse
    {
        public abstract string CreateRouteResponse(double[,] distanceMatrix, double[,] durationMatrix, int[] citiesIndexOrderArray, List<City> cities);
        
    }

    class RouteDistanceAndDurationResponse : RouteResponse
    {
        private double[,] distanceMatrix;
        private double[,] durationMatrix;
        List<City> cities;
        int[] citiesIndexOrderArray;
        public override string CreateRouteResponse(double[,] distanceMatrix, double[,] durationMatrix, int[] citiesIndexOrderArray, List<City> cities)
        {
            this.distanceMatrix = distanceMatrix;
            this.durationMatrix = durationMatrix;
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
                    routeStr.Append(cities[citiesIndexOrderArray[i]].longName);
                }
                else
                {
                    routeStr.Append(cities[citiesIndexOrderArray[i]].longName + " -> ");
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
                    detailedRoute.Append($"Путь: {cities[citiesIndexOrderArray[i]].longName} -> {cities[citiesIndexOrderArray[0]].longName} займет {(int)distanceMatrix[citiesIndexOrderArray[0], citiesIndexOrderArray[i]]} км. и {(int)durationMatrix[citiesIndexOrderArray[0], citiesIndexOrderArray[i]]} ч. \n");
                    break;
                }

                detailedRoute.Append($"Путь: {cities[citiesIndexOrderArray[i]].longName} -> {cities[citiesIndexOrderArray[j]].longName} займет {(int)distanceMatrix[citiesIndexOrderArray[i], citiesIndexOrderArray[j]]} км. и {(int)durationMatrix[citiesIndexOrderArray[i], citiesIndexOrderArray[j]]} ч. \n");
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
