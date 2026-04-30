using ASD.Graphs;

public class Lab10 : MarshalByRefObject
{
    static bool[] visited;
    static int[] colors;
    static Graph g;
    static int bridge;          // s2 = początek drugiej połówki

    static int[] bestPath;
    static List<int> half1;     // pierwsza połówka: s1 -> ... -> u
    static List<int> half2;     // druga połówka:   s2 -> ... -> v

    static void SyncDFS(int u, int v)
    {
        foreach (var nu in g.OutNeighbors(u))
        {
            if (visited[nu]) continue;

            foreach (var nv in g.OutNeighbors(v))
            {
                if (visited[nv]) continue;
                if (nu == nv) continue;
                if (colors[nu] != colors[nv]) continue;

                visited[nu] = true;
                visited[nv] = true;
                half1.Add(nu);
                half2.Add(nv);

                // Pełna ścieżka: [s1,...,nu] -> [s2,...,nv]
                // nu musi łączyć się z s2 (bridge) żeby ścieżka była spójna
                if (g.HasEdge(nu, bridge))
                {
                    // half1 + half2 = parzysta długość ✓
                    // bo obie połówki mają zawsze taki sam rozmiar
                    var candidate = new List<int>(half1);
                    candidate.AddRange(half2);

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

                half1 = new List<int> { s1 };
                half2 = new List<int> { s2 };

                // Bazowy przypadek: ścieżka [s1, s2], długość 2
                // s1 i s2 mają ten sam kolor, i są połączone krawędzią
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

        return bestPath;
    }
}