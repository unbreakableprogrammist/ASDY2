using System;
using System.Collections.Generic;
using ASD.Graphs;

namespace ASD2
{
    public class GraphColorer : MarshalByRefObject
    {
        private int n;
        private List<int>[] adj;

        private int[] coloring;
        private int[] uncoloredDegrees;
        private Stack<int> deferred;
        private int currentK;
        
        private int[] colorUsedGen;
        private int currentGen;

        public (int numberOfColors, int[] coloring) FindBestColoring(Graph g)
        {
            n = g.VertexCount;
            if (n == 0) return (0, new int[0]);

            adj = new List<int>[n];
            int maxDegree = 0;
            for (int i = 0; i < n; i++)
            {
                adj[i] = new List<int>();
                foreach (int neighbor in g.OutNeighbors(i))
                {
                    if (neighbor != i) adj[i].Add(neighbor);
                }
                if (adj[i].Count > maxDegree) maxDegree = adj[i].Count;
            }

            for (int k = 1; k <= maxDegree + 1; k++)
            {
                if (k == 1)
                {
                    if (maxDegree == 0) return (1, new int[n]);
                    continue;
                }
                if (k == 2)
                {
                    if (TryColorBipartite(out int[] bipartiteColoring)) return (2, bipartiteColoring);
                    continue;
                }

                int[] coloringResult = TryColorWithK(k);
                if (coloringResult != null) return (k, coloringResult);
            }

            return (0, null);
        }

        private bool TryColorBipartite(out int[] colors)
        {
            colors = new int[n];
            for (int i = 0; i < n; i++) colors[i] = -1;

            for (int i = 0; i < n; i++)
            {
                if (colors[i] == -1)
                {
                    colors[i] = 0;
                    Queue<int> q = new Queue<int>();
                    q.Enqueue(i);

                    while (q.Count > 0)
                    {
                        int u = q.Dequeue();
                        foreach (int v in adj[u])
                        {
                            if (colors[v] == -1)
                            {
                                colors[v] = 1 - colors[u];
                                q.Enqueue(v);
                            }
                            else if (colors[v] == colors[u])
                            {
                                return false; 
                            }
                        }
                    }
                }
            }
            return true;
        }

        private int[] TryColorWithK(int k)
        {
            currentK = k;
            coloring = new int[n];
            for (int i = 0; i < n; i++) coloring[i] = -1;

            uncoloredDegrees = new int[n];
            for (int i = 0; i < n; i++) uncoloredDegrees[i] = adj[i].Count;

            deferred = new Stack<int>();
            colorUsedGen = new int[k];
            currentGen = 0;

            if (SolveDec(0, -1))
            {
                while (deferred.Count > 0)
                {
                    int v = deferred.Pop();
                    bool[] used = new bool[k];
                    foreach (int neighbor in adj[v])
                    {
                        int c = coloring[neighbor];
                        if (c >= 0) used[c] = true;
                    }
                    int chosen = 0;
                    while (chosen < k && used[chosen]) chosen++;
                    coloring[v] = chosen;
                }
                return coloring;
            }
            return null;
        }

        private bool SolveDec(int coloredCount, int maxColorUsed)
        {
            int initialDeferredCount = deferred.Count;

            while (true)
            {
                if (coloredCount == n) return true;

                int bestV = -1;
                int minAvailable = int.MaxValue;
                int maxUncolDeg = -1;
                bool foundSafe = false;

                for (int i = 0; i < n; i++)
                {
                    if (coloring[i] == -1)
                    {
                        currentGen++; 
                        int available = currentK;
                        foreach (int neighbor in adj[i])
                        {
                            int c = coloring[neighbor];
                            if (c >= 0 && colorUsedGen[c] != currentGen)
                            {
                                colorUsedGen[c] = currentGen;
                                available--;
                            }
                        }

                        if (available > uncoloredDegrees[i])
                        {
                            coloring[i] = -2;
                            deferred.Push(i);
                            coloredCount++;
                            foreach (int neighbor in adj[i])
                            {
                                if (coloring[neighbor] == -1) uncoloredDegrees[neighbor]--;
                            }
                            foundSafe = true;
                            break; 
                        }

                        if (available < minAvailable || (available == minAvailable && uncoloredDegrees[i] > maxUncolDeg))
                        {
                            minAvailable = available;
                            maxUncolDeg = uncoloredDegrees[i];
                            bestV = i;
                        }
                    }
                }

                if (foundSafe) continue; 

                if (bestV == -1 || minAvailable == 0) break; 

                
                bool[] bestUsed = new bool[currentK];
                foreach (int neighbor in adj[bestV])
                {
                    int c = coloring[neighbor];
                    if (c >= 0) bestUsed[c] = true;
                }

                
                int limit = Math.Min(currentK - 1, maxColorUsed + 1);
                
                for (int c = 0; c <= limit; c++)
                {
                    if (bestUsed[c]) continue;

                    coloring[bestV] = c;
                    foreach (int neighbor in adj[bestV])
                        if (coloring[neighbor] == -1) uncoloredDegrees[neighbor]--;

                    if (SolveDec(coloredCount + 1, Math.Max(maxColorUsed, c))) return true;
                    coloring[bestV] = -1;
                    foreach (int neighbor in adj[bestV])
                        if (coloring[neighbor] == -1) uncoloredDegrees[neighbor]++;
                }

                break; 
            }

            int addedDeferred = deferred.Count - initialDeferredCount;
            for (int i = 0; i < addedDeferred; i++)
            {
                int v = deferred.Pop();
                foreach (int neighbor in adj[v])
                {
                    if (coloring[neighbor] == -1) uncoloredDegrees[neighbor]++;
                }
                coloring[v] = -1;
            }

            return false;
        }
    }
}