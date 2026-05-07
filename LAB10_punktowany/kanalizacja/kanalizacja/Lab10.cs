using System;
using System.Collections.Generic;
using ASD.Graphs;

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
        private int min_s_siz;

        private List<int> bestS;
        private List<int> curS;
        private bool[] Removed; // wierzcholki ktora maja stopien 0 lub 1 lub sa w S ( w danym momencie rekurencji)
        private Queue<int> Q; // kolejka do BFS ( zeby nie realkokowac nowej w kazdym wywolaniu ) 
        private int n;

        private int[]
            active_degree; // obecny stopien kazdego wierzcholka ( wierzcholki odznaczone w Removed na true sie nie licza) 

        private bool[] forbiden; // zabronione wierzcholki na tym poziomie rekurencji 

        private int[] parent; // dla bfs do trzymania rodzica 

        // generacje to po prostu zeby nie resetowac depth w BFS tylko tak jest szybciej
        private int[] _visitedGen; // Zapisuje, z jakiej generacji zwiadowca tu był
        private int _currentGen; // Aktualny numer generacji zwiadowcy
        private int[] dist; // Dystans od startu 
        private bool[] inPathU; // tablica do "malowania farba" przy szukaniu LCA

        private Comparison<int> _degreeComparer;

        /// <summary>
        /// Główna funkcja : wchodzimy , szukamy jak najmniejszego cyklu , rekurencyjnie sprawdzamy co jestli ten wierzcholek z cyklu usuniemy 
        /// </summary>
        public void Backtrack(Graph graph)
        {
            if (curS.Count >= min_s_siz)
                return; // odcinamy galaz rekurencji nie ma sensu wchodzic jesli nasz obecny zbior juz jest wiekszy
            var reduced = Reduce(graph); // usuwamy te wierzcholki ktore maja za maly stopien

            // Teraz ta sekcja jest od tego ze patrzymy czy wogole damy rade poprawic wynik, jak?
            // wiemy ze w lesie |V| > |E|, wiec wogole patrzymy ile razy musielibysmy usunac ten wierzcholek zeby
            // byl spelniowny warunek lasu 

            // V - liczba wierzcholkow, E - liczba krawedzi, maxDeg - maksymalny deg
            int V = 0, E = 0, maxDeg = 0;
            for (int v = 0; v < n; v++)
            {
                if (!Removed[v])
                {
                    V++;
                    E += active_degree[v];
                    if (active_degree[v] > maxDeg) maxDeg = active_degree[v];
                }
            }

            E /= 2; // bo policzylismy dwa razy 
            if (V > 0 && maxDeg > 1) // jesli sa jakies wierzcholki i deg > 1 
            {
                int ile_usunac_przynajmniej =
                    E - V + 1; // czyli ile wiecej mamy krawedzi > wierz ( no + 1 bo to ma isc w druga strone )
                if (ile_usunac_przynajmniej > 0)
                {
                    /*
                     * 1. excess = E - V + 1  -> Tyle krawędzi "za dużo" ma graf w stosunku do idealnego lasu (to nasz "dług").
                     * 2. maxDeg - 1          -> Dokładnie o tyle zmniejsza się nasz dług, gdy usuniemy najgrubszy wierzchołek.
                     * 3. Cel: Obliczyć sufit z (excess / (maxDeg - 1)), czyli MINIMALNĄ liczbę wierzchołków do usunięcia.
                     */
                    int estimate = (ile_usunac_przynajmniej + maxDeg - 2) / (maxDeg - 1);
                    if (curS.Count + estimate >= min_s_siz) // nasze dolne oszacowanie nadal zbyt slabe to koniec 
                    {
                        Restore(reduced, graph);
                        return;
                    }
                }
            }

            // nasza dodatkowa sekcja sprawdzeniowa skonczona wiec lecimy dalej 
            List<int> cycle_vertex = FindCycle(graph); // dostajemy liste wierzcholkow na cyklu

            if (cycle_vertex == null || cycle_vertex.Count == 0)
            {
                // wiemy ze skoro doszlismy do tego miejsca to min_s_siz > curS , wiec skoro nie ma cykli to mamy potencjalnie najlepszy wynik 
                min_s_siz = curS.Count;
                bestS = new List<int>(curS);
                Restore(reduced, graph);
                return;
            }

            // jeszcze jedno zabezpiecznie , jesli jestesmy tutaj to wiemy ze jest jakis cykl wiec spradzamy czy jak dodamy wierzcholek (a musimy to czy nie popsujemy calosci)
            if (curS.Count + 1 >= min_s_siz)
            {
                Restore(reduced, graph);
                return;
            }

            // usprawnienie i Heurystyka, na logike powinnismy usuwac wierzcholki ktore maja najwiekszy stopien 
            cycle_vertex.Sort(_degreeComparer);

            // forbiden dziala tak ze na jednym poziomie rekurencji jesli sprawdzimy jakis wierzcholek A jako dodany do S to nie bedzie on dodany do S na tym poziomie lub nizej 
            var newForbidden = new List<int>();
            foreach (var vert in cycle_vertex)
            {
                if (forbiden[vert]) continue; // jesli jest zabroniony to go nie dodajemy do S
                curS.Add(vert); // dodajemy do S
                Removed[vert] = true; // jesli jest w S to jest jakby removed
                foreach (var u in graph.OutNeighbors(vert))
                {
                    if (!Removed[u]) active_degree[u]--;
                }

                Backtrack(graph); // rekurencja w dol 

                // teraz wracamy z rekurencji 
                Removed[vert] = false;
                active_degree[vert] = 0;
                foreach (var u in graph.OutNeighbors(vert))
                {
                    if (!Removed[u])
                    {
                        active_degree[u]++; // dodajemy sasiadom
                        active_degree[vert]++; // dodajemy sobie
                    }
                }

                curS.RemoveAt(curS.Count - 1); // w powrocie jestesmy ostatni dodani wiec usuwamy siebie z S
                forbiden[vert] = true;
                newForbidden.Add(vert);
                if (HasForbidenCycle(vert, graph)) break;
            }

            // teraz jakby wiemy ze jestesmy juz po sprawdzeniu calego poziomu rekurencji wiec odblokowujemy siebie
            foreach (var f in newForbidden)
            {
                forbiden[f] = false;
            }

            Restore(reduced, graph);
        }

        /// <summary>
        /// szukamy BFS-em krótkiego cyklu w czasie liniowym O(V+E).
        /// </summary>
        public List<int> FindCycle(Graph graph)
        {
            List<int> bestCycle = null;
            _currentGen++;

            for (int startNode = 0; startNode < n; startNode++)
            {
                if (Removed[startNode] || active_degree[startNode] < 2 ||
                    _visitedGen[startNode] == _currentGen) continue;

                Q.Clear();
                Q.Enqueue(startNode);

                _visitedGen[startNode] = _currentGen;
                parent[startNode] = -1;
                dist[startNode] = 0;

                while (Q.Count > 0)
                {
                    int u = Q.Dequeue();

                    if (bestCycle != null && dist[u] >= bestCycle.Count) break;

                    foreach (var v in graph.OutNeighbors(u))
                    {
                        if (Removed[v]) continue;
                        if (v == parent[u]) continue;

                        if (_visitedGen[v] == _currentGen)
                        {
                            var cycle = ReconstructCycle(u, v, parent);

                            if (bestCycle == null || cycle.Count < bestCycle.Count)
                            {
                                bestCycle = cycle;
                                if (bestCycle.Count <= 3) return bestCycle;
                            }
                        }
                        else
                        {
                            _visitedGen[v] = _currentGen;
                            parent[v] = u;
                            dist[v] = dist[u] + 1;
                            Q.Enqueue(v);
                        }
                    }
                }
            }

            return bestCycle;
        }

        /// <summary>
        /// Sprawdza czy jest w jakiejsc iteracji zablokowany cykl. 
        /// </summary>
        private bool HasForbidenCycle(int start, Graph graph)
        {
            // Jeśli wierzchołek startowy nie jest zabroniony, albo już go usunęliśmy, to nie ma o czym gadać
            if (!forbiden[start] || Removed[start]) return false;
            // zaczynamy nowa generacje 
            _currentGen++;

            Q.Clear();
            Q.Enqueue(start);
            _visitedGen[start] = _currentGen;
            parent[start] = -1;

            while (Q.Count > 0)
            {
                int u = Q.Dequeue();

                foreach (var nei in graph.OutNeighbors(u))
                {
                    // interesuja nas tylko wierzcholki zabronione bo z nich chcemy cykl zrobic 
                    if (!forbiden[nei] || Removed[nei]) continue;

                    if (nei == parent[u]) continue; // nie wracamy po tej samej rurze

                    // jesli widzimy zabronionego sasiada to mamy cykl
                    if (_visitedGen[nei] == _currentGen)
                    {
                        return true;
                    }

                    // Zaznaczamy odwiedziny w obecnej generacji i idziemy dalej
                    _visitedGen[nei] = _currentGen;
                    parent[nei] = u;
                    Q.Enqueue(nei);
                }
            }

            return false;
        }

        /// <summary>
        /// jak mamy sytuacje ze 1 - 2 - 3 i  1 i 3 nie sasiaduja i 2 ma stopien 2 to mozemy zlaczyc 1 i 3 w jedno.
        /// </summary>
        private void PreprocessSubdivisions(Graph graph)
        {
            Queue<int> q = new Queue<int>();
            for (int i = 0; i < n; i++)
            {
                if (active_degree[i] == 2) q.Enqueue(i);
            }

            while (q.Count > 0)
            {
                int v = q.Dequeue();
                if (Removed[v] || active_degree[v] != 2) continue;

                List<int> validNeighbors = new List<int>();
                foreach (var w in graph.OutNeighbors(v))
                {
                    if (!Removed[w]) validNeighbors.Add(w);
                }

                // Jeśli wierzchołek faktycznie wciąż ma dokładnie 2 sąsiadów
                if (validNeighbors.Count == 2)
                {
                    int a = validNeighbors[0];
                    int b = validNeighbors[1];

                    // Zamiast wierzchołka V, łączymy jego sąsiadów bezpośrednio (jeśli jeszcze nie są połączeni)
                    if (!graph.HasEdge(a, b))
                    {
                        Removed[v] = true;
                        active_degree[v] = 0;
                        graph.AddEdge(a, b);
                        graph.RemoveEdge(a, v);
                        graph.RemoveEdge(b, v);
                    }
                }
            }
        }

        /// <summary>
        /// Funkcja ktora robi dwie rzeczy, przchodzi po calym grafie i odznacza te wierzcholki ktore maja degree <= 1
        /// jako sasiedzi nie sa liczeni wierzcholki oznaczone jako Removed
        /// </summary>
        private List<int> Reduce(Graph graph)
        {
            var reduced = new List<int>();
            Q.Clear();
            for (int v = 0; v < n; v++)
            {
                if (!Removed[v] && active_degree[v] <= 1)
                {
                    Q.Enqueue(v);
                }
            }

            // na kolejce sa tylko wierzcholki ktore maja obecnie deg <= 1
            while (Q.Count > 0)
            {
                int v = Q.Dequeue();
                if (Removed[v]) continue;
                Removed[v] = true;
                reduced.Add(v); // dodajemy ze usuwamy v bo obecnie ma stopien <= 1
                // idziemy po wszyskich sasiadach v i zmniejszamy im stopien o 1
                foreach (var u in graph.OutNeighbors(v))
                {
                    if (!Removed[u])
                    {
                        active_degree[u]--;
                        if (active_degree[u] <= 1) Q.Enqueue(u);
                    }
                }
            }

            return reduced;
        }

        /// <summary>
        /// Odtwarza cykl po śladach w tablicy parent. 
        /// Szuka miejsca, gdzie drogi się rozwidliły (Najniższy Wspólny Przodek - LCA).
        /// </summary>
        private List<int> ReconstructCycle(int u, int v, int[] parent)
        {
            List<int> cycle = new List<int>();
            List<int> pathU = new List<int>(); // trasa od u do korzenia
            List<int> pathV = new List<int>(); // trasa od v do korzenia

            // Idziemy od 'u' do góry i zostawiamy "ślad z farby"
            int curr = u;
            while (curr != -1)
            {
                pathU.Add(curr);
                inPathU[curr] = true; // malujemy wierzcholek na true, ze tu bylismy
                curr = parent[curr];
            }

            // Idziemy od 'v' do góry w stronę korzenia
            curr = v;
            while (curr != -1)
            {
                pathV.Add(curr);
                curr = parent[curr];
            }

            // Szukamy punktu przecięcia (LCA) - pierwszego wierzchołka z naszą farbą
            int lca = -1;
            foreach (int node in pathV)
            {
                if (inPathU[node]) // to jest miejsce dzie u i v sie spotykaja
                {
                    lca = node;
                    break;
                }
            }

            // MUSIMY ZMYĆ FARBĘ, żeby nie popsuć innych iteracji!
            foreach (int node in pathU) inPathU[node] = false;

            // idziemy z u do LCA
            foreach (int node in pathU)
            {
                cycle.Add(node);
                if (node == lca) break;
            }

            // A potem trasa V, też tylko do LCA
            foreach (int node in pathV)
            {
                if (node == lca) break;
                cycle.Add(node);
            }

            return cycle;
        }

        /// <summary>
        /// Restore ->  przywraca wierzcholki usuniete przez Reduce 
        /// </summary>
        private void Restore(List<int> reduced, Graph graph)
        {
            // idziemy od tylu bo w reduced szlismy od przodu czyli teraz jak bedziemy sie cofac to chcemy robic to od tylu
            for (int i = reduced.Count - 1; i >= 0; i--)
            {
                int v = reduced[i];
                Removed[v] = false;
                active_degree[v] = 0;
                foreach (var u in graph.OutNeighbors(v))
                {
                    if (!Removed[u])
                    {
                        active_degree[v]++;
                        active_degree[u]++;
                    }
                }
            }
        }

        public int Stage1(Graph G, int maxBudget, out int[] S)
        {
            n = G.VertexCount;
            min_s_siz = maxBudget + 1; // to jest nasz gorny limit
            curS = new List<int>(maxBudget + 1);
            bestS = new List<int>();

            Graph kopia = new Graph(n, G.Representation);
            for (int i = 0; i < n; i++)
            {
                foreach (var w in G.OutNeighbors(i))
                {
                    if (i < w) kopia.AddEdge(i, w);
                }
            }

            Removed = new bool[n];
            Q = new Queue<int>(n);
            active_degree = new int[n];
            parent = new int[n];
            _visitedGen = new int[n];
            dist = new int[n];
            forbiden = new bool[n];
            inPathU = new bool[n];
            _currentGen = 0;

            _degreeComparer = (a, b) => active_degree[b].CompareTo(active_degree[a]);

            for (int v = 0; v < n; v++)
            {
                foreach (var u in kopia.OutNeighbors(v))
                {
                    active_degree[v]++;
                }
            }

            Reduce(kopia); // na poczatku usuwamy wszystkie wierzcholki o stopniu 0 i 1
            PreprocessSubdivisions(kopia);

            Backtrack(kopia);

            if (min_s_siz > maxBudget)
            {
                S = null;
                return -1;
            }

            S = bestS.ToArray();
            return min_s_siz;
        }


        /// <summary>
        /// Etap 2: Szukanie zbioru wierzchołków S o minimalnym koszcie.
        /// </summary>
        private int min_s_cost;

        private int current_cost;
        private int[] v_costs;
        private int[] sortedCosts; // posortowane koszty wszystkich wierzchołków do LB

        public int Stage2(Graph G, int[] cost, int maxBudget, out int[] S)
        {
            n = G.VertexCount;
            v_costs = cost;
            min_s_cost = maxBudget + 1;
            current_cost = 0;
            curS = new List<int>();
            bestS = new List<int>();

            Removed = new bool[n];
            forbiden = new bool[n];
            Q = new Queue<int>(n);
            active_degree = new int[n];
            parent = new int[n];
            _visitedGen = new int[n];
            dist = new int[n];
            inPathU = new bool[n];
            _currentGen = 0;

            sortedCosts = (int[])cost.Clone();
            Array.Sort(sortedCosts);

            
            _degreeComparer = (a, b) =>
            {
                double ratioA = (double)active_degree[a] / Math.Max(1, v_costs[a]);
                double ratioB = (double)active_degree[b] / Math.Max(1, v_costs[b]);
                return ratioB.CompareTo(ratioA);
            };

            Graph kopia = new Graph(n, G.Representation);
            for (int i = 0; i < n; i++)
            {
                foreach (var w in G.OutNeighbors(i))
                    if (i < w)
                        kopia.AddEdge(i, w);
            }

            for (int v = 0; v < n; v++)
            {
                foreach (var u in kopia.OutNeighbors(v))
                {
                    active_degree[v]++;
                }
            }
            var reduced = Reduce(kopia);
            Backtrack2(kopia);
            Restore(reduced, kopia);

            if (min_s_cost > maxBudget)
            {
                S = null;
                return -1;
            }

            S = bestS.ToArray();
            return min_s_cost;
        }

        public void Backtrack2(Graph graph)
        {
            if (current_cost >= min_s_cost) return;

            var reduced = Reduce(graph);
            int V = 0, E = 0, maxDeg = 0;
            for (int i = 0; i < n; i++)
            {
                if (!Removed[i])
                {
                    V++;
                    int deg = active_degree[i];
                    E += deg;
                    if (deg > maxDeg) maxDeg = deg;
                }
            }
            E /= 2;
            if (V > 0 && maxDeg > 1)
            {
                int excess = E - V + 1;
                if (excess > 0)
                {
                    int estimate = (excess + maxDeg - 2) / (maxDeg - 1);
                    
                    int lb_bonus = 0;
                    for (int i = 0; i < estimate && i < sortedCosts.Length; i++)
                        lb_bonus += sortedCosts[i];

                    if (current_cost + lb_bonus >= min_s_cost)
                    {
                        Restore(reduced, graph);
                        return;
                    }
                }
            }
            
            List<int> cycle_vertex = FindCycle(graph);
            
            if (cycle_vertex == null || cycle_vertex.Count == 0)
            {
                if (current_cost < min_s_cost)
                {
                    min_s_cost = current_cost;
                    bestS = new List<int>(curS);
                }
                Restore(reduced, graph);
                return;
            }
            
            // Odfiltrowujemy bezsensowne wierzchołki o stopniu 2
            cycle_vertex = FilterCycle(cycle_vertex);

            cycle_vertex.Sort(_degreeComparer);

            var newForbidden = new List<int>();
            foreach (var vert in cycle_vertex)
            {
                if (forbiden[vert]) continue;

                curS.Add(vert);
                current_cost += v_costs[vert];
                Removed[vert] = true;

                foreach (var u in graph.OutNeighbors(vert))
                    if (!Removed[u]) active_degree[u]--;

                Backtrack2(graph);
                
                Removed[vert] = false;
                active_degree[vert] = 0;
                foreach (var u in graph.OutNeighbors(vert))
                {
                    if (!Removed[u])
                    {
                        active_degree[u]++;
                        active_degree[vert]++;
                    }
                }
                current_cost -= v_costs[vert];
                curS.RemoveAt(curS.Count - 1);

                forbiden[vert] = true;
                newForbidden.Add(vert);
                if (HasForbidenCycle(vert, graph)) break;
            }

            foreach (var f in newForbidden) forbiden[f] = false;
            Restore(reduced, graph);
        }
        private List<int> FilterCycle(List<int> cycle)
        {
            List<int> filtered = new List<int>();
            int bestDeg2 = -1;
            int minCost = int.MaxValue;

            foreach (int v in cycle)
            {
                if (active_degree[v] == 2)
                {
                    if (v_costs[v] < minCost)
                    {
                        minCost = v_costs[v];
                        bestDeg2 = v;
                    }
                }
            }
            foreach (int v in cycle)
            {
                if (active_degree[v] > 2)
                {
                    filtered.Add(v); 
                }
                else if (v == bestDeg2)
                {
                    filtered.Add(v); 
                }
            }

            return filtered;
        }
    }
    
}