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
        public double[,] matrix;
        public abstract double[,] Count(string[] citiesCoordinates);
    }

    class DistanceMatrix : Matrix
    {
        public override double[,] Count(string[] citiesCoordinates)
        {
            matrix = new double[citiesCoordinates.Count(), citiesCoordinates.Count()];

            for (int i = 0; i < citiesCoordinates.Length; i++)
            {
                for(int j = 0; j < citiesCoordinates.Length; j++)
                {
                    if (i == j)
                    {
                        matrix[i, j] = 0;
                        continue;
                    };
                    matrix[i,j] = APIRequest.MakeDistanceRequest(citiesCoordinates[i], citiesCoordinates[j]).Result;
                }
            }

            return matrix;
        }

        
    }

    class TimeMatrix : Matrix
    {
        public override double[,] Count(string[] citiesCoordinates)
        {
            matrix = new double[citiesCoordinates.Count(), citiesCoordinates.Count()];

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
