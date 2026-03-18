using System;
using ASD.Graphs;
using ASD;
using System.Collections.Generic;

namespace ASD
{

    public class Lab03GraphFunctions : System.MarshalByRefObject
    {

        // Część 1
        // Wyznaczanie odwrotności grafu
        //   0.5 pkt
        // Odwrotność grafu to graf skierowany o wszystkich krawędziach przeciwnie skierowanych niż w grafie pierwotnym
        // Parametry:
        //   g - graf wejściowy
        // Wynik:
        //   odwrotność grafu
        // Uwagi:
        //   1) Graf wejściowy pozostaje niezmieniony
        //   2) Graf wynikowy musi być w takiej samej reprezentacji jak wejściowy
        public DiGraph Lab03Reverse(DiGraph g)
        {
            DiGraph result = new DiGraph(g.VertexCount, g.Representation); // robimy kopie grafu o tej takiej samej liczbie wierzcholkow 
            foreach (Edge e in g.DFS().SearchAll())
            {
                result.AddEdge(e.To, e.From);
            }
            return result;
            
        }

        // Część 2
        // Badanie czy graf jest dwudzielny
        //   0.5 pkt
        // Graf dwudzielny to graf nieskierowany, którego wierzchołki można podzielić na dwa rozłączne zbiory
        // takie, że dla każdej krawędzi jej końce należą do róźnych zbiorów
        // Parametry:
        //   g - badany graf
        //   vert - tablica opisująca podział zbioru wierzchołków na podzbiory w następujący sposób
        //          vert[i] == 1 oznacza, że wierzchołek i należy do pierwszego podzbioru
        //          vert[i] == 2 oznacza, że wierzchołek i należy do drugiego podzbioru
        // Wynik:
        //   true jeśli graf jest dwudzielny, false jeśli graf nie jest dwudzielny (w tym przypadku parametr vert ma mieć wartość null)
        // Uwagi:
        //   1) Graf wejściowy pozostaje niezmieniony
        //   2) Podział wierzchołków może nie być jednoznaczny - znaleźć dowolny
        //   3) Pamiętać, że każdy z wierzchołków musi być przyporządkowany do któregoś ze zbiorów
        //   4) Metoda ma mieć taki sam rząd złożoności jak zwykłe przeszukiwanie (za większą będą kary!)
        // --- METODA POMOCNICZA ---
        // Rekurencyjnie odwiedza wierzchołki i próbuje nadać im naprzemienne kolory.
        // Zwraca true, jeśli podgraf jest dwudzielny, w przeciwnym razie false.
        bool DFS_color(int v, Graph g, int[] vert, int color)
        {
            // 1. Oznaczamy bieżący wierzchołek zadanym kolorem (1 lub 2)
            vert[v] = color; 
            
            // 2. Przechodzimy po wszystkich sąsiadach bieżącego wierzchołka
            foreach (int u in g.OutNeighbors(v)) 
            {
                // Przypadek A: Sąsiad nie był jeszcze odwiedzony
                if (vert[u] == -1)
                {
                    // Ustalamy dla niego kolor przeciwny do naszego (1 -> 2, 2 -> 1)
                    int next_color = (color == 1) ? 2 : 1; 
                    
                    // Rekurencyjnie wchodzimy w sąsiada. Jeśli głębiej znajdzie się konflikt,
                    // przerywamy działanie i od razu zwracamy false (tzw. propagacja błędu w górę).
                    if (!DFS_color(u, g, vert, next_color)) 
                    { 
                        return false;
                    }
                }
                // Przypadek B: Sąsiad był już odwiedzony
                else
                {
                    // Jeśli sąsiad ma taki sam kolor jak my, znaleźliśmy konflikt (cykl nieparzystej długości).
                    // Graf na pewno nie jest dwudzielny.
                    if (vert[u] == color)
                    {
                        return false;
                    }
                }
            }
            
            // Jeśli sprawdziliśmy wszystkich sąsiadów i nie było konfliktów - wszystko jest OK.
            return true;
        }

        // --- METODA GŁÓWNA ---
        public bool Lab03IsBipartite(Graph g, out int[] vert)
        {
            // Inicjalizacja tablicy wynikowej. Używamy -1, by odróżnić wierzchołki nieodwiedzone
            // od docelowych kolorów (1 i 2).
            vert = new int[g.VertexCount];
            Array.Fill(vert, -1);
            
            // Pętla przechodząca po wszystkich wierzchołkach gwarantuje, 
            // że odwiedzimy wszystkie spójne składowe grafu (nawet te odcięte od wierzchołka 0).
            for (int i = 0; i < g.VertexCount; i++)
            {
                // Zaczynamy przeszukiwanie tylko dla nieodwiedzonych wierzchołków
                if (vert[i] == -1)
                {
                    // Próbujemy pokolorować daną spójną składową, zaczynając od koloru 1.
                    // Jeśli wystąpi konflikt (false), natychmiast przerywamy.
                    if (!DFS_color(i, g, vert, 1))
                    {
                        vert = null; // Zgodnie ze specyfikacją przy porażce zwracamy null
                        return false;
                    }
                }
            }
            
            // Udało się pokolorować cały graf bez konfliktów!
            return true;
        }
        public bool Lab03IsBipartite_v2(Graph g, out int[] vert)
        {
            // Inicjalizacja tablicy. W C# domyślnie wypełnia się zerami.
            // Uznajemy wartość 0 za znacznik "wierzchołek nieodwiedzony".
            vert = new int[g.VertexCount];

            // Iterujemy po wszystkich krawędziach grafu znalezionych przez algorytm DFS z biblioteki
            foreach (Edge e in g.DFS().SearchAll())
            {
                // 1. Jeśli wierzchołek startowy krawędzi nie ma koloru, nadajemy mu startowy kolor 1
                if (vert[e.From] == 0)
                {
                    vert[e.From] = 1;
                }

                // 2. Jeśli wierzchołek docelowy nie ma koloru, 
                // nadajemy mu kolor przeciwny do wierzchołka startowego (1 -> 2, 2 -> 1)
                if (vert[e.To] == 0)
                {
                    vert[e.To] = (vert[e.From] == 1) ? 2 : 1;
                }

                // 3. Sprawdzenie konfliktu: jeśli oba końce tej samej krawędzi mają ten sam kolor,
                // graf nie jest dwudzielny. Zwracamy null i kończymy.
                if (vert[e.From] == vert[e.To])
                {
                    vert = null;
                    return false;
                }
            }

            // 4. Korekta dla wierzchołków izolowanych.
            // DFS.SearchAll() przechodzi tylko po krawędziach. Jeśli w grafie są wierzchołki,
            // które nie mają żadnych krawędzi, ich kolor pozostałby 0, co łamie specyfikację zadania.
            for (int i = 0; i < g.VertexCount; i++)
            {
                if (vert[i] == 0)
                {
                    vert[i] = 1; // Izolowany wierzchołek może dostać dowolny ważny kolor, np. 1
                }
            }

            // Żadna z krawędzi nie wywołała konfliktu, a wszystkie wierzchołki mają prawidłowy kolor.
            return true;
        }

        // Część 3
        // Wyznaczanie minimalnego drzewa rozpinającego algorytmem Kruskala
        //   1 pkt
        // Schemat algorytmu Kruskala
        //   1) wrzucić wszystkie krawędzie do "wspólnego worka"
        //   2) wyciągać z "worka" krawędzie w kolejności wzrastających wag
        //      - jeśli krawędź można dodać do drzewa to dodawać, jeśli nie można to ignorować
        //      - punkt 2 powtarzać aż do skonstruowania drzewa (lub wyczerpania krawędzi)
        // Parametry:
        //   g - graf wejściowy
        //   mstw - waga skonstruowanego drzewa (lasu)
        // Wynik:
        //   skonstruowane minimalne drzewo rozpinające (albo las)
        // Uwagi:
        //   1) Graf wejściowy pozostaje niezmieniony
        //   2) Wykorzystać klasę UnionFind z biblioteki Graph
        //   3) Jeśli graf g jest niespójny to metoda wyznacza las rozpinający
        //   4) Graf wynikowy (drzewo) musi być w takiej samej reprezentacji jak wejściowy
        public Graph<int> Lab03Kruskal(Graph<int> g, out int mstw)
        {
            mstw = 0;
            //PriorityQueue<int,Edge<int>> priorityQueue = new PriorityQueue<int,Edge>();
            
        }

        // Część 4
        // Badanie czy graf nieskierowany jest acykliczny
        //   0.5 pkt
        // Parametry:
        //   g - badany graf
        // Wynik:
        //   true jeśli graf jest acykliczny, false jeśli graf nie jest acykliczny
        // Uwagi:
        //   1) Graf wejściowy pozostaje niezmieniony
        //   2) Najpierw pomysleć jaki, prosty do sprawdzenia, warunek spełnia acykliczny graf nieskierowany
        //      Zakodowanie tego sprawdzenia nie powinno zająć więcej niż kilka linii!
        //      Zadanie jest bardzo łatwe (jeśli wydaje się trudne - poszukać prostszego sposobu, a nie walczyć z trudnym!)
        bool DFS(int v, Graph g, bool[] odw,int parent)
        {
            odw[v] = true;
            foreach (int u in g.OutNeighbors(v))
            {
                if (odw[u] && u != parent)
                {
                    return  false; // znalezlismy cykl 
                }

                if (!odw[u])
                {
                    if(!DFS(u, g, odw,v)) return false;
                }
            }
            return true;
        }
        public bool Lab03IsUndirectedAcyclic(Graph g)
        {
            bool[] odw = new bool[g.VertexCount];
            Array.Fill(odw, false);
            for (int i = 0; i < g.VertexCount; i++)
            {
                if (odw[i] == false)
                {
                    if(!DFS(i, g, odw,-1)) return false;
                }
            }
            return true;
        }
        

    }

}
