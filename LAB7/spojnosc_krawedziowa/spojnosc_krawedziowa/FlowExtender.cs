using ASD.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASD
{
    public static class FlowExtender
    {

        private static void bfs_private(int s, DiGraph<double> res, bool[] visited)
        {
            Queue<int> queue = new Queue<int>();
            queue.Enqueue(s);
            visited[s] = true;
            while (queue.Count > 0)
            {
                int v = queue.Dequeue();
                foreach (var edge in res.OutEdges(v))
                {
                    int u = edge.To;
                    double weight = edge.Weight;
                    if (!visited[u] && weight > 0) // bo to double wiec tam 0 jest jakas mala prezycja 
                    {
                        visited[u] = true;
                        queue.Enqueue(u);
                    }
                }
            }
        }
        
        /// <summary>
        /// Metod wylicza minimalny s-t-przekrój.
        /// </summary>
        /// <param name="undirectedGraph">Nieskierowany graf</param>
        /// <param name="s">wierzchołek źródłowy</param>
        /// <param name="t">wierzchołek docelowy</param>
        /// <param name="minCut">minimalny przekrój</param>
        /// <returns>wartość przekroju</returns>
        
        public static double MinCut(this Graph<double> undirectedGraph, int s, int t, out Edge<double>[] minCut)
        {
            int n = undirectedGraph.VertexCount;
            DiGraph<double> res = new DiGraph<double>(n);
            for (int i = 0; i < n; i++)
            {
                foreach(var edge in undirectedGraph.OutEdges(i))
                {
                    res.AddEdge(edge.From, edge.To, edge.Weight);
                }
            }
            // teraz mamy siec resydualna wiec puszczamy na niej forda- fulkersona 
            var (flowValue, f) = Flows.FordFulkerson(res, s, t);
            // teraz musimy zupdateowac siec resydualna robimy to z tym wzorkiem 
            // jestesmy w krawedzi uw:
            // 1. jesli uw nalezy do E(G) to w sieci resydualnej krawedz to c(u->w) - f(u->w), 
            // i puszczamy krawedz w-> u o f(uw) 
            // jeszcze patrzymy sobie ze jesli wu tez nalezy to dodajemy sobie wartosc f(wu)

            for (int u = 0; u < n; u++)
            {
                foreach (var edge in f.OutEdges(u))
                {
                    int v = edge.To;
                    double f_edge_value = edge.Weight; // u->v w f
                    double stara_waga_przod = res.GetEdgeWeight(u, v); // u -> v w res
                    res.SetEdgeWeight(u,v,stara_waga_przod- f_edge_value); // ustawiamy res(u->v) = c(u->v) - f(u->v)
                    double stara_Waga_tyl = res.GetEdgeWeight(v, u); // waga v -> u w res ( wiemy ze na pewno istnieje bo na poczatku byl graf nieskierowany )
                    res.SetEdgeWeight(v, u, stara_Waga_tyl+f_edge_value); // dodajemy do tej kreawedzi w tyl
                }
            }
            // teraz pora na BFS , zauwazmy ze teraz wszyskie waskie gardla
            bool[] visited = new bool[n];
            Array.Fill(visited, false);
            bfs_private(s, res, visited);
            // idziemy po wierzcholkach, jesli jakis jest odwiedzony to patrzymy czy ma nieodwiedzonego sasiada
            List<Edge<double>> cutEdges = new List<Edge<double>>();
            for (int u = 0; u < n; u++)
            {
                if (!visited[u]) continue; // jesli wierzcholek nie odwiedzony to continue 
                
                foreach (var edge in undirectedGraph.OutEdges(u))
                {
                    int v = edge.To;
                    if (!visited[v])
                    {
                        cutEdges.Add(edge);
                    }
                }
            }
            minCut = cutEdges.ToArray();
            return flowValue;
        }

        /// <summary>
        /// Metada liczy spójność krawędziową grafu oraz minimalny zbiór rozcinający.
        /// </summary>
        /// <param name="undirectedGraph">nieskierowany graf</param>
        /// <param name="cutingSet">zbiór krawędzi rozcinających</param>
        /// <returns>spójność krawędziowa</returns>
        public static int EdgeConnectivity(this Graph<double> undirectedGraph, out Edge<double>[] cutingSet)
        {
            cutingSet = null;
            return 0;
        }
        
    }
}