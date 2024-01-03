using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp2
{
    class TSP
    {
        static int INF = int.MaxValue;

        static int n;
        static int[,] input_matrix;
        static int[] path;
        static bool[] visited;
        static int best_len = INF;
        static int[] best_path;

        static void BranchAndBound(int cur, int len)
        {
            if (len >= best_len)
            {
                return;
            }

            if (cur == n)
            {
                if (len + input_matrix[path[n - 1], 0] < best_len)
                {
                    Array.Copy(path, best_path, n);
                    best_len = len + input_matrix[path[n - 1], 0];
                }
                return;
            }

            for (int i = 1; i < n; i++)
            {
                if (!visited[i])
                {
                    visited[i] = true;
                    path[cur] = i;
                    BranchAndBound(cur + 1, len + input_matrix[path[cur - 1], i]);
                    visited[i] = false;
                }
            }
        }

        public static Tuple<int[], int> Start(int[,] matrix)
        {
            n = matrix.GetLength(0);

            input_matrix = new int[n, n];
            path = new int[n];
            visited = new bool[n];
            best_path = new int[n];


            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    input_matrix[i, j] = matrix[i, j];
                }
            }

            visited[0] = true;
            path[0] = 0;
            BranchAndBound(1, 0);
            var result = new Tuple<int[], int>(best_path, best_len);

            return result;
        }
    }

}
