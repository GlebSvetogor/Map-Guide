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
        public int[,] matrix;
        public abstract int[,] Count(string[] citiesCoordinates);
        public void PrintMatrix(int[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Console.Write(matrix[i, j] + "\t");
                }
                Console.WriteLine();
            }
        }

    }

    class DistanceMatrix : Matrix
    {
        public override int[,] Count(string[] citiesCoordinates)
        {
            matrix = new int[citiesCoordinates.Count(), citiesCoordinates.Count()];

            for (int i = 0; i < citiesCoordinates.Length; i++)
            {
                for(int j = 0; j < citiesCoordinates.Length; j++)
                {
                    if (i == j)
                    {
                        matrix[i, j] = 0;
                        continue;
                    };
                    matrix[i,j] = (int)APIRequest.MakeDistanceRequest(citiesCoordinates[i], citiesCoordinates[j]).Result;
                }
            }

            return matrix;
        }

        
    }

    class TimeMatrix : Matrix
    {
        public override int[,] Count(string[] citiesCoordinates)
        {
            matrix = new int[citiesCoordinates.Count(), citiesCoordinates.Count()];

            for (int i = 0; i < citiesCoordinates.Length; i++)
            {
                for (int j = 0; j < citiesCoordinates.Length; j++)
                {
                    if (i == j)
                    {
                        matrix[i, j] = 0;
                        continue;
                    };
                    matrix[i, j] = APIRequest.MakeDurationRequest(citiesCoordinates[i], citiesCoordinates[j]).Result;
                }
            }

            return matrix;
        }
    }

    class CoordinateMatrix
    {
        public string[] array;
        public async Task<string[]> Count(List<string> cities)
        {
            array = new string[cities.Count];
            int i = 0;
            foreach (string city in cities)
            {
                string [] coordinates = (await APIRequest.MakeCoordinateRequest(city)).Split(' ');
                string resultCoordinatesStr = string.Join(",", coordinates);
                array[i] = resultCoordinatesStr;
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

    class TimeMatrixCreator : MatrixCreator
    {
        public override Matrix CreateMatrix() => new TimeMatrix();
    }
}
