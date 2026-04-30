using ASD.Graphs;

public class Lab10 : MarshalByRefObject
{
    static bool[] visited;      // czy wierzchołek jest już na ścieżce
    static int[] colors;        // kolory wierzchołków
    static Graph g;             
    static int bridge;          // wierzchołek łączący obie połówki (s2)
    
    static int[] bestPath;      // najlepsza znaleziona ścieżka (tablica wierzchołków)
    static List<int> half1;     // aktualna pierwsza połówka ścieżki
    static List<int> half2;     // aktualna druga połówka ścieżki

    static void SyncDFS(int u, int v)
    {
        foreach (var nu in g.OutNeighbors(u))
        {
            if (visited[nu]) continue;

            foreach (var nv in g.OutNeighbors(v))
            {
                if (visited[nv]) continue;
                if (nu == nv) continue;
                if (colors[nu] != colors[nv]) continue; // kolory muszą się zgadzać

                visited[nu] = true;
                visited[nv] = true;
                half1.Add(nu);
                half2.Add(nv);

                // Sprawdź czy koniec pierwszej połówki łączy się z bridge
                // Ścieżka: [half1] -> bridge -> [half2 odwrotnie]
                if (g.HasEdge(nu, bridge))
                {
                    // Zbuduj pełną ścieżkę: half1 + bridge + half2 odwrotnie
                    var candidate = new List<int>(half1);
                    candidate.Add(bridge);
                    for (int i = half2.Count - 1; i >= 0; i--)
                        candidate.Add(half2[i]);

                    // Zapisz jeśli lepsza od dotychczasowej
                    // Długość powtórzenia = half1.Count + 1 + half2.Count (parzysta)
                    // ale uwaga: powtórzenie to [half1 + bridge] i [half2 odwrotnie + s2start]
                    // Sprawdź czy candidate.Length > bestPath.Length
                    if (bestPath == null || candidate.Count > bestPath.Length)
                        bestPath = candidate.ToArray();
                }

                SyncDFS(nu, nv);

                half1.RemoveAt(half1.Count - 1);
                half2.RemoveAt(half2.Count - 1);
                visited[nu] = false;
                visited[nv] = false;
            }
        }
    }

    public int[] FindLongestRepetition(Graph graph, int[] colorArray)
    {
        g = graph;
        colors = colorArray;
        int n = g.VertexCount;
        visited = new bool[n];
        bestPath = null;

        for (int s1 = 0; s1 < n; s1++)
        {
            for (int s2 = 0; s2 < n; s2++)
            {
                if (s1 == s2) continue;
                if (colors[s1] != colors[s2]) continue;

                bridge = s2;
                visited[s1] = true;
                visited[s2] = true;

                // Inicjuj połówki: s1 to pierwsza połówka, s2 to druga
                half1 = new List<int> { s1 };
                half2 = new List<int> { s2 };

                // Bazowy przypadek: powtórzenie s1->s2
                // Ścieżka: s1, s2 — długość 2
                if (g.HasEdge(s1, s2))
                {
                    if (bestPath == null || 2 > bestPath.Length)
                        bestPath = new int[] { s1, s2 };
                }

                SyncDFS(s1, s2);

                visited[s1] = false;
                visited[s2] = false;
            }
        }

        return bestPath; // null jeśli nie znaleziono żadnego powtórzenia
    }
}