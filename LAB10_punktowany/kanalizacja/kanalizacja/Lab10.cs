using ASD.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics;

namespace ASD
{
    public class Lab10 : MarshalByRefObject
    {
        /// <summary>
        /// Etap 1: Szukanie najmniejszego zbioru wierzchołków S, takiego że G - S jest lasem.
        /// </summary>
        /// <param name="G">Graf nieskierowany; wierzchołki = skrzyżowania, krawędzie = rury</param>
        /// <param name="maxBudget">Górne ograniczenie na rozmiar szukanego rozwiązania</param>
        /// <param name="S">Najmniejsza tablica wierzchołków S taka, że G - S jest lasem</param>
        /// <returns>Rozmiar najmniejszego zbioru S</returns>
        ///

        private int _min_zbior;
        private List<int> s;
        private List<int> best_S;
        private bool zmieniono = false;
        public int DFS(int v, int p, Graph graph, List<int> path, bool[] visited)
        {
            visited[v] = true;
            path.Add(v);
            foreach (var nei in graph.OutNeighbors(v))
            {
                if (nei == p) continue;
                if (s.Contains(nei)) continue; 
                if (visited[nei]) return nei; 
                if (!visited[nei])
                {
                    int koniec = DFS(nei, v, graph, path, visited);
                    if (koniec != -1) return koniec; 
                }
            }
            path.RemoveAt(path.Count - 1);
            return -1;
        }

        public List<int> FindCycle(Graph graph)
        {
            int n = graph.VertexCount;
            List<int> wierzcholki_na_cyklu = new List<int>();
            List<int> path = new List<int>();
            bool[] visited = new bool[n];
            int v = -1;
            for (int i = 0; i < n; i++)
            {
                if (s.Contains(i)) continue; 
                if (!visited[i]) 
                {
                    v = DFS(i, -1, graph, path, visited);
                    if (v != -1) break;
                }
            }
            if (v == -1) return null;
            else
            {
                bool czy_byl_v = false;
                foreach (var wierz in path)
                {
                    if (wierz == v) czy_byl_v = true;
                    if (czy_byl_v)
                    {
                        wierzcholki_na_cyklu.Add(wierz);
                    }
                }
            }
            return wierzcholki_na_cyklu;
        }

        public void Backtrack(Graph graph)
        {
            if (s != null && s.Count >= _min_zbior) return;
            
            List<int> cycle_vertex = FindCycle(graph);
            
            if (cycle_vertex == null || cycle_vertex.Count == 0)
            {
                if (s.Count < _min_zbior) 
                {
                    _min_zbior = s.Count;
                    best_S = new List<int>(s); 
                }
                return;
            }
            
            foreach (var vert in cycle_vertex)
            {
                s.Add(vert);
                Backtrack(graph);
                s.RemoveAt(s.Count - 1);
            }
        }

        public int Stage1(Graph G, int maxBudget, out int[] S)
        {
            _min_zbior = maxBudget + 1;
            s = new List<int>();
            best_S = new List<int>(); 
            Backtrack(G);
            if (_min_zbior > maxBudget)
            {
                S = new int[0];
                return 0;
            }
            S = best_S.ToArray();
            return _min_zbior;
        }

        /// <summary>
        /// Etap 2: Szukanie zbioru wierzchołków S, takiego że G - S jest lasem, o minimalnym łącznym koszcie.
        /// </summary>
        /// <param name="G">Graf nieskierowany; wierzchołki = skrzyżowania, krawędzie = rury</param>
        /// <param name="cost">Koszt montażu zaworu w każdym wierzchołku (cost[v] >= 0)</param>
        /// <param name="maxBudget">Górne ograniczenie kosztu szukanego rozwiązania</param>
        /// <param name="S">Tablica wierzchołków S o minimalnym łącznym koszcie, taka że G - S jest lasem</param>
        /// <returns>Suma kosztów montażu zaworów w wierzchołkach z S</returns>
        public int Stage2(Graph G, int[] cost, int maxBudget, out int[] S)
        {
            S = null;
            return -1;
        }
    }
}