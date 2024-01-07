using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    abstract class Matrix
    {
        public double[,] distanceMatrix;
        public double[,] durationMatrix;
        public abstract (double[,], double[,]) Count(string[] citiesCoordinates);
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
        public override (double[,], double[,]) Count(string[] citiesCoordinates)
        {
            distanceMatrix = new double[citiesCoordinates.Count(), citiesCoordinates.Count()];
            durationMatrix = new double[citiesCoordinates.Count(), citiesCoordinates.Count()];

            for (int i = 0; i < citiesCoordinates.Length; i++)
            {
                for(int j = 0; j < citiesCoordinates.Length; j++)
                {
                    if (i == j)
                    {
                        distanceMatrix[i, j] = 0;
                        durationMatrix[i, j] = 0;
                        continue;
                    };
                    var durationAndDistance = APIRequest.MakeDefaultRequest(citiesCoordinates[i], citiesCoordinates[j]).Result;
                    distanceMatrix[i, j] = durationAndDistance.Item1;
                    durationMatrix[i, j] = durationAndDistance.Item2;
                }
            }

            return (distanceMatrix, durationMatrix);
        }
    }

    class CoordinateMatrix
    {
        public string[] array;
        public async Task<string[]> Count(List<Root> cities)
        {
            array = new string[cities.Count];
            int i = 0;
            foreach (Root city in cities)
            {
                Console.WriteLine(city.lat + ',' + city.lng);
                array[i] = city.lat + ',' + city.lng;
                ++i;
            }
            return array;

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

}
