using System;
using System.Linq;

namespace ConsoleApp2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            int n = Convert.ToInt32(Console.ReadLine()); // количество строк
            int m = Convert.ToInt32(Console.ReadLine()); // количество столбцов
            int[,] matrix = new int[n, m]; // создание матрицы

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    Console.Write($"Введите элемент [{i}, {j}]: ");
                    matrix[i, j] = int.Parse(Console.ReadLine());
                }
            }

            string result = String.Join(" ,", NearestNeighbor(matrix));

            Console.WriteLine(result);

            string result2 = String.Join(" ,", TSP(matrix));

            Console.WriteLine(result2);


        }

        public static int Branch(int[,] matrix)
        {
            int n = matrix.GetLength(0);
            int[] visited = new int[n];
            for (int i = 0; i < n; i++)
            {
                visited[i] = 1 << i;
            }
            int[,] memo = new int[n, 1 << n];
            int[] path = new int[n + 1];
            path[0] = 0;
            TSPHelper(matrix, visited, memo, 0, 1, path, 1);
            return memo[0, 1];
        }

        public static int BranchHelper(int[,] matrix, int[] visited, int[,] memo, int current, int mask, int[] path, int index)
        {
            if (mask == (1 << visited.Length) - 1)
            {
                return matrix[current, 0];
            }
            if (memo[current, mask] > 0)
            {
                return memo[current, mask];
            }
            int min = int.MaxValue;
            int next = -1;
            for (int i = 0; i < visited.Length; i++)
            {
                if ((mask & visited[i]) == 0)
                {
                    int cost = matrix[current, i] + TSPHelper(matrix, visited, memo, i, mask | visited[i], path, index + 1);
                    if (cost < min)
                    {
                        min = cost;
                        next = i;
                    }
                }
            }
            memo[current, mask] = min;
            path[index] = next;
            return min;
        }

        public static int[] NearestNeighbor(int[,] matrix)
        {
            int n = matrix.GetLength(0);
            int[] path = new int[n + 1];
            bool[] visited = new bool[n];
            path[0] = 0;
            visited[0] = true;
            for (int i = 1; i < n; i++)
            {
                int min = int.MaxValue;
                int index = -1;
                for (int j = 0; j < n; j++)
                {
                    if (!visited[j] && matrix[path[i - 1], j] < min)
                    {
                        min = matrix[path[i - 1], j];
                        index = j;
                    }
                }
                path[i] = index;
                visited[index] = true;
            }
            path[n] = 0;
            return path;
        }

        public static int[] TSP(int[,] matrix)
        {
            int n = matrix.GetLength(0);
            int[] visited = new int[n];
            for (int i = 0; i < n; i++)
            {
                visited[i] = 1 << i;
            }
            int[,] memo = new int[n, 1 << n];
            int[] path = new int[n + 1];
            path[0] = 0;
            TSPHelper(matrix, visited, memo, 0, 1, path, 1);
            return path;
        }

        public static int TSPHelper(int[,] matrix, int[] visited, int[,] memo, int current, int mask, int[] path, int index)
        {
            if (mask == (1 << visited.Length) - 1)
            {
                return matrix[current, 0];
            }
            if (memo[current, mask] > 0)
            {
                return memo[current, mask];
            }
            int min = int.MaxValue;
            int next = -1;
            for (int i = 0; i < visited.Length; i++)
            {
                if ((mask & visited[i]) == 0)
                {
                    int cost = matrix[current, i] + TSPHelper(matrix, visited, memo, i, mask | visited[i], path, index + 1);
                    if (cost < min)
                    {
                        min = cost;
                        next = i;
                    }
                }
            }
            memo[current, mask] = min;
            path[index] = next;
            return min;
        }
    }
}
