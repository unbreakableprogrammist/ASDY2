// using ASD.Graphs;
// using System;
// using System.Collections.Generic;
// using System.Linq;
//
// namespace ASD
// {
//     public class chat_rozwiazanie : MarshalByRefObject
//     {
//         private int _min_zbior;
//         private List<int> s;
//         private List<int> best_S;
//
//         private bool[] isRemoved;
//         private bool[] forbidden;
//         private int[] parent;
//         private int[] depth;
//         private Queue<int> Q;
//         private bool[] inPathU;
//         private Graph _currentGraph;
//         private Comparison<int> _degreeComparer;
//         private bool[] visitedForbidden;
//         private int[] parentForbidden;
//         private int[] activeDeg; 
//
//         public List<int> FindCycle(Graph graph)
//         {
//             int n = graph.VertexCount;
//             List<int> bestCycle = null; 
//
//             for (int i = 0; i < n; i++) { parent[i] = -1; depth[i] = -1; }
//             for (int startNode = 0; startNode < n; startNode++)
//             {
//                 if (isRemoved[startNode] || depth[startNode] != -1) continue;
//                 Q.Clear();
//                 Q.Enqueue(startNode);
//                 depth[startNode] = 0;
//                 while (Q.Count > 0)
//                 {
//                     int u = Q.Dequeue();
//                     
//                     if (bestCycle != null && depth[u] >= bestCycle.Count) break;
//
//                     foreach (var v in graph.OutNeighbors(u))
//                     {
//                         if (isRemoved[v]) continue;
//                         if (v == parent[u]) continue;
//                         
//                         if (depth[v] != -1)
//                         {
//                             var cycle = ReconstructCycle(u, v, parent);
//                             if (bestCycle == null || cycle.Count < bestCycle.Count)
//                             {
//                                 bestCycle = cycle;
//                                 if (bestCycle.Count <= 3) return bestCycle; 
//                             }
//                         }
//                         else
//                         {
//                             parent[v] = u;
//                             depth[v] = depth[u] + 1;
//                             Q.Enqueue(v);
//                         }
//                     }
//                 }
//             }
//             return bestCycle;
//         }
//
//         private bool HasForbiddenCycle(int start)
//         {
//             if (!forbidden[start] || isRemoved[start]) return false;
//             int n = _currentGraph.VertexCount;
//             for (int i = 0; i < n; i++) visitedForbidden[i] = false;
//             Q.Clear();
//             Q.Enqueue(start);
//             visitedForbidden[start] = true;
//             parentForbidden[start] = -1;
//             while (Q.Count > 0)
//             {
//                 int u = Q.Dequeue();
//                 foreach (var nei in _currentGraph.OutNeighbors(u))
//                 {
//                     if (!forbidden[nei] || isRemoved[nei]) continue;
//                     if (nei == parentForbidden[u]) continue;
//                     if (visitedForbidden[nei]) return true;
//                     visitedForbidden[nei] = true;
//                     parentForbidden[nei] = u;
//                     Q.Enqueue(nei);
//                 }
//             }
//             return false;
//         }
//
//         private List<int> ReconstructCycle(int u, int v, int[] parent)
//         {
//             List<int> cycle = new List<int>();
//             List<int> pathU = new List<int>();
//             List<int> pathV = new List<int>();
//
//             int curr = u;
//             while (curr != -1) { pathU.Add(curr); inPathU[curr] = true; curr = parent[curr]; }
//             curr = v;
//             while (curr != -1) { pathV.Add(curr); curr = parent[curr]; }
//
//             int lca = -1;
//             foreach (int node in pathV) { if (inPathU[node]) { lca = node; break; } }
//             foreach (int node in pathU) inPathU[node] = false;
//
//             foreach (int node in pathU) { cycle.Add(node); if (node == lca) break; }
//             foreach (int node in pathV) { if (node == lca) break; cycle.Add(node); }
//
//             return cycle;
//         }
//
//         private int CompareByActiveDegree(int a, int b)
//         {
//             return activeDeg[b].CompareTo(activeDeg[a]); 
//         }
//
//         private List<int> Reduce()
//         {
//             var reduced = new List<int>();
//             Q.Clear();
//             int n = _currentGraph.VertexCount;
//             for (int v = 0; v < n; v++)
//                 if (!isRemoved[v] && activeDeg[v] <= 1)
//                     Q.Enqueue(v);
//
//             while (Q.Count > 0)
//             {
//                 int v = Q.Dequeue();
//                 if (isRemoved[v]) continue;
//                 isRemoved[v] = true;
//                 reduced.Add(v);
//                 foreach (var u in _currentGraph.OutNeighbors(v))
//                 {
//                     if (!isRemoved[u])
//                     {
//                         activeDeg[u]--;
//                         if (activeDeg[u] <= 1)
//                             Q.Enqueue(u);
//                     }
//                 }
//             }
//             return reduced;
//         }
//
//         private void Restore(List<int> reduced)
//         {
//             for (int i = reduced.Count - 1; i >= 0; i--)
//             {
//                 int v = reduced[i];
//                 isRemoved[v] = false;
//                 activeDeg[v] = 0;
//                 foreach (var u in _currentGraph.OutNeighbors(v))
//                 {
//                     if (!isRemoved[u])
//                     {
//                         activeDeg[u]++;
//                         activeDeg[v]++;
//                     }
//                 }
//             }
//         }
//         
//         private void PreprocessSubdivisions()
//         {
//             Queue<int> q = new Queue<int>();
//             int n = _currentGraph.VertexCount;
//             for (int i = 0; i < n; i++)
//             {
//                 if (activeDeg[i] == 2) q.Enqueue(i);
//             }
//
//             while (q.Count > 0)
//             {
//                 int v = q.Dequeue();
//                 if (isRemoved[v] || activeDeg[v] != 2) continue;
//
//                 List<int> validNeighbors = new List<int>();
//                 foreach (var w in _currentGraph.OutNeighbors(v))
//                 {
//                     if (!isRemoved[w]) validNeighbors.Add(w);
//                 }
//
//                 if (validNeighbors.Count == 2)
//                 {
//                     int a = validNeighbors[0];
//                     int b = validNeighbors[1];
//                     
//                     if (!_currentGraph.HasEdge(a, b))
//                     {
//                         isRemoved[v] = true;
//                         activeDeg[v] = 0;
//                         _currentGraph.AddEdge(a, b);
//                         _currentGraph.RemoveEdge(a, v);
//                         _currentGraph.RemoveEdge(b, v);
//                     }
//                 }
//             }
//         }
//
//         public void Backtrack(Graph graph)
//         {
//             if (s.Count >= _min_zbior) return;
//             var reduced = Reduce();
//             {
//                 int V = 0, E = 0, maxDeg = 0;
//                 int n = graph.VertexCount;
//                 for (int v = 0; v < n; v++)
//                 {
//                     if (!isRemoved[v])
//                     {
//                         V++;
//                         E += activeDeg[v];
//                         if (activeDeg[v] > maxDeg) maxDeg = activeDeg[v];
//                     }
//                 }
//                 E /= 2;
//                 if (V > 0 && maxDeg > 1)
//                 {
//                     int excess = E - V + 1; 
//                     if (excess > 0)
//                     {
//                         
//                         int lb = (excess + maxDeg - 2) / (maxDeg - 1); 
//                         if (s.Count + lb >= _min_zbior)
//                         {
//                             Restore(reduced);
//                             return;
//                         }
//                     }
//                 }
//             }
//
//             List<int> cycle_vertex = FindCycle(graph);
//
//             if (cycle_vertex == null || cycle_vertex.Count == 0)
//             {
//                 _min_zbior = s.Count;
//                 best_S = new List<int>(s);
//                 Restore(reduced);
//                 return;
//             }
//
//             if (s.Count + 1 >= _min_zbior)
//             {
//                 Restore(reduced);
//                 return;
//             }
//
//             cycle_vertex.Sort(_degreeComparer);
//             var newlyForbidden = new List<int>();
//
//             foreach (var vert in cycle_vertex)
//             {
//                 if (forbidden[vert]) continue;
//
//                 s.Add(vert);
//                 isRemoved[vert] = true;
//                 foreach (var u in graph.OutNeighbors(vert))
//                     if (!isRemoved[u]) activeDeg[u]--;
//
//                 Backtrack(graph);
//                 isRemoved[vert] = false;
//                 activeDeg[vert] = 0;
//                 foreach (var u in graph.OutNeighbors(vert))
//                 {
//                     if (!isRemoved[u])
//                     {
//                         activeDeg[u]++;
//                         activeDeg[vert]++;
//                     }
//                 }
//                 s.RemoveAt(s.Count - 1);
//
//                 forbidden[vert] = true;
//                 newlyForbidden.Add(vert);
//                 if (HasForbiddenCycle(vert)) break;
//             }
//
//             foreach (var f in newlyForbidden)
//                 forbidden[f] = false;
//
//             Restore(reduced);
//         }
//
//         public int Stage1(Graph G, int maxBudget, out int[] S)
//         {
//             _degreeComparer = CompareByActiveDegree;
//             _min_zbior = maxBudget + 1;
//             s = new List<int>(maxBudget + 1);
//             best_S = new List<int>();
//
//             int n = G.VertexCount;
//             
//             Graph H = new Graph(n, G.Representation);
//             for (int i = 0; i < n; i++)
//             {
//                 foreach (var w in G.OutNeighbors(i))
//                 {
//                     if (i < w) H.AddEdge(i, w);
//                 }
//             }
//             _currentGraph = H;
//
//             isRemoved = new bool[n];
//             forbidden = new bool[n];
//             parent = new int[n];
//             depth = new int[n];
//             inPathU = new bool[n];
//             Q = new Queue<int>(n);
//             visitedForbidden = new bool[n];
//             parentForbidden = new int[n];
//
//             activeDeg = new int[n];
//             for (int v = 0; v < n; v++)
//                 foreach (var u in _currentGraph.OutNeighbors(v))
//                     activeDeg[v]++;
//             Reduce();
//             PreprocessSubdivisions();
//
//             Backtrack(_currentGraph);
//
//             if (_min_zbior > maxBudget) { S = new int[0]; return 0; }
//             S = best_S.ToArray();
//             return _min_zbior;
//         }
//
//         public int Stage2(Graph G, int[] cost, int maxBudget, out int[] S)
//         {
//             S = null;
//             return -1;
//         }
//     }
// }