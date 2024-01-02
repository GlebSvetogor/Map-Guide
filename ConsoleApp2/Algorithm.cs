using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp2
{
    abstract class Algorithm
    {
        public abstract List<int> FindRoute(double[,] distance);
    }

    class FastestAlgorithm : Algorithm
    {
        public override List<int> FindRoute(double[,] distance)
        {
            return new List<int>();
        }
    }

    class ShortestAlgorithm : Algorithm
    {
        public override List<int> FindRoute(double[,] distances)
        {
            int n = distances.GetLength(0);
            int[] path = new int[n];
            bool[] visited = new bool[n];
            double[] distance = new double[n];

            for (int i = 0; i < n; i++)
            {
                path[i] = -1;
                visited[i] = false;
                distance[i] = double.MaxValue;
            }

            distance[0] = 0.0;

            for (int i = 0; i < n - 1; i++)
            {
                int u = -1;
                double minDistance = double.MaxValue;

                for (int j = 0; j < n; j++)
                {
                    if (!visited[j] && distance[j] < minDistance)
                    {
                        u = j;
                        minDistance = distance[j];
                    }
                }

                visited[u] = true;

                for (int v = 0; v < n; v++)
                {
                    if (!visited[v] && distances[u, v] > 0.0 && distance[u] + distances[u, v] < distance[v])
                    {
                        distance[v] = distance[u] + distances[u, v];
                        path[v] = u;
                    }
                }
            }

            int currentNode = n - 1;
            List<int> shortestPath = new List<int>();

            while (currentNode != -1)
            {
                shortestPath.Add(currentNode);
                currentNode = path[currentNode];
            }

            shortestPath.Reverse();

            return shortestPath;
        }
    }

    abstract class AlgorithmCreator
    {
        public abstract Algorithm CreateAlgorithm();
    }

    class FastestAlgorithmCreator : AlgorithmCreator
    {
        public override Algorithm CreateAlgorithm()
        {
            return new FastestAlgorithm();
        }
    }

    class ShortestAlgorithmCreator : AlgorithmCreator
    {
        public override Algorithm CreateAlgorithm()
        {
            return new ShortestAlgorithm();
        }
    }
}
