using ASD.Graphs;
using System;
using System.Linq;
using System.Text;


namespace ASD
{
    public class Maze : MarshalByRefObject
    {
        public static int INF = 1*1000000000 + 7; 

        /// <summary>
        /// Wersje zadania I oraz II
        /// Zwraca najkrótszy możliwy czas przejścia przez labirynt bez dynamitów lub z dowolną ich liczbą
        /// </summary>
        /// <param name="maze">labirynt</param>
        /// <param name="withDynamite">informacja, czy dostępne są dynamity 
        /// Wersja I zadania -> withDynamites = false, Wersja II zadania -> withDynamites = true</param>
        /// <param name="path">zwracana ścieżka</param>
        /// <param name="t">czas zburzenia ściany (dotyczy tylko wersji II)</param> 
        public int FindShortestPath(char[,] maze, bool withDynamite, out string path, int t = 0)
        {
            int n = maze.GetLength(0);
            int m = maze.GetLength(1);

            int Start_v = 0;
            int End_v = 0;
            
            DiGraph g = new DiGraph(n * m);

            int[,] d = new int[,] { { 0, 1 }, { 0, -1 }, { 1, 0 }, { -1, 0 } }; 
            
            for (int y = 0; y < n; y++)
            {
                for (int x = 0; x < m; x++)
                {
                    int v_From = y * m + x;
                    if(maze[y,x] == 'S') Start_v = v_From;
                    if(maze[y,x] == 'E') End_v = v_From;
                    for (int di = 0; di < d.GetLength(0); di++)
                    {
                        int neiY = y + d[di, 0];
                        int neiX = x + d[di, 1];
                        
                        if(neiX<0 || neiX >=n || neiY<0 || neiY>=m) continue;
                        if (maze[y, x] == 'O')
                        {
                            g.AddEdge(neiX)
                        } 
                    }
                    

                }
            }
            path = "";

            return -1;
        }

        /// <summary>
        /// Wersja III i IV zadania
        /// Zwraca najkrótszy możliwy czas przejścia przez labirynt z użyciem co najwyżej k lasek dynamitu
        /// </summary>
        /// <param name="maze">labirynt</param>
        /// <param name="k">liczba dostępnych lasek dynamitu, dla wersji III k=1</param>
        /// <param name="path">zwracana ścieżka</param>
        /// <param name="t">czas zburzenia ściany</param>
        public int FindShortestPathWithKDynamites(char[,] maze, int k, out string path, int t)
        {
            path = "";
            return -1;
        }
    }
}