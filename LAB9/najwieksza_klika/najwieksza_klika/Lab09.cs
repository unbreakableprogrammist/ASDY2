using System.Collections.Generic;
using System.Linq;
using ASD.Graphs;

/// <summary>
/// Klasa rozszerzająca klasę Graph o rozwiązania problemów największej kliki i izomorfizmu grafów metodą pełnego przeglądu (backtracking)
/// </summary>
public static class Lab10GraphExtender
{
    ///<summary>
    /// funkcja pomocnicza do rekurencyjnego liczenia maksymalnej kliki
    /// </summary>
    /// <param name="k">od którego wierzchołka możemy sprawdzać</param>
    /// <param name="S">zbior wierzcholkow obecnie w klice</param>
    /// <param name="maxClique">najwieksza klika do tej pory</param>
    static int MaxCliqueRec(int k, List<int> S, int maxClique, int n, Graph g, ref int[] bestClique)
    {
        var C = new List<int>();
        for (int i = k; i < n; i++)
        {
            if (S.All(s => g.HasEdge(i, s)))
                C.Add(i);
        }
        
        if (S.Count + C.Count <= maxClique)
            return maxClique;
        
        if (S.Count > maxClique)
        {
            maxClique = S.Count;
            bestClique = S.ToArray();
        }

        foreach (int vert in C)
        {
            S.Add(vert);                                                    
            maxClique = MaxCliqueRec(vert + 1, S, maxClique, n, g, ref bestClique);
            S.Remove(vert);                                                 
        }
        return maxClique;
    }
    /// <summary>
    /// Wyznacza największą klikę w grafie i jej rozmiar metodą pełnego przeglądu (backtracking)
    /// </summary>
    /// <param name="g">Badany graf</param>
    /// <param name="clique">Wierzchołki znalezionej największej kliki - parametr wyjściowy</param>
    /// <returns>Rozmiar największej kliki</returns>
    /// <remarks>
    /// Nie wolno modyfikować badanego grafu.
    /// </remarks>
    
    public static int MaxClique(this Graph g, out int[] clique)
    {
        clique = new int[0];
        var S = new List<int>();
        MaxCliqueRec(0, S, 0, g.VertexCount, g, ref clique);
        return clique.Length;
    }

    /// <summary>
    /// Bada izomorfizm grafów metodą pełnego przeglądu (backtracking)
    /// </summary>
    /// <param name="g">Pierwszy badany graf</param>
    /// <param name="h">Drugi badany graf</param>
    /// <param name="map">Mapowanie wierzchołków grafu h na wierzchołki grafu g (jeśli grafy nie są izomorficzne to null) - parametr wyjściowy</param>
    /// <returns>Informacja, czy grafy g i h są izomorficzne</returns>
    /// <remarks>
    /// 1) Uwzględniamy wagi krawędzi
    /// 3) Nie wolno modyfikować badanych grafów.
    /// </remarks>
    ///
    static bool IsConsistent(int k, int v, int[] map, Graph<int> g, Graph<int> h)
    {
        // na pewno wierzcholki musza miec te same stopnie 
        if (g.OutNeighbors(v).Count() != h.OutNeighbors(k).Count()) return false;
        
        // idziemy po wszystkich juz sprawdzonych wierzcholkach 
        for (int j = 0; j < k; j++)
        {
            bool hMaKrawedz = h.HasEdge(k, j); 
            bool gMaKrawedz = g.HasEdge(v, map[j]);

            if (hMaKrawedz != gMaKrawedz) return false; // jesli jakis ma a drugi nie to na pewno nie zgodne 
            if (hMaKrawedz) // jesli h ( a wiec i g tez ) ma krawedz 
            {
                if (h.GetEdgeWeight(k, j) != g.GetEdgeWeight(v, map[j])) // jesli wagi sa rozne to ble 
                    return false;
            }
        }
        return true;
    }
    // n - liczba wierz, k - do k-tego wierzcholka z g szukamy dopasowania, uzyte - uzyte wierz wczesniej
    public static bool GeneratePermutations(int n, int k, bool[] uzyte, Graph<int> g, Graph<int> h, int[] map)
    {
        // jesli doszlismy do konca to finito ( ostatni wierzcholek to n-1
        if (k == n) return true; 
        for (int i = 0; i < n; i++)
        {
            if (uzyte[i]) continue;
            if (IsConsistent(k, i, map, g, h)) // sprawdzamy czy te dwa wierzchoki sa ze soba zgodne 
            {
                map[k] = i;
                uzyte[i] = true;
                if (GeneratePermutations(n, k + 1, uzyte, g, h, map)) 
                    return true;
                uzyte[i] = false;
            }
        }
        return false;
    }

    public static bool IsomorphismTest(this Graph<int> g, Graph<int> h, out int[] map)
    {
        map = null;
        if (g.VertexCount != h.VertexCount) return false;
        int n = g.VertexCount;
        bool[] uzyte = new bool[n];
        map = new int[n];
        if (GeneratePermutations(n, 0, uzyte, g, h, map))
            return true;
        map = null;
        return false;
    }

}