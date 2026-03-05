using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASD
{
    public class Lab02 : MarshalByRefObject
    {
        /// <summary>
        /// Etap 1 - wyznaczenie najtańszej trasy, zgodnie z którą pionek przemieści się z pozycji poczatkowej (0,0) na pozycję docelową
        /// </summary>
        /// <param name="n">wysokość prostokąta</param>
        /// <param name="m">szerokość prostokąta</param>
        /// <param name="moves">tablica z dostępnymi ruchami i ich kosztami (di - o ile zwiększamy numer wiersza, dj - o ile zwiększamy numer kolumnj, cost - koszt ruchu)</param>
        /// <returns>(bool result, int cost, (int, int)[] path) - result ma wartość true jeżeli trasa istnieje, false wpp., cost to minimalny koszt, path to wynikowa trasa</returns>
        public (bool result, int cost, (int i, int j)[] path) Lab02Stage1(int n, int m, ((int di, int dj) step, int cost)[] moves)
        {
            // 1. Zmiana na long, żeby zmieścić absolutnie każdy koszt trasy
            long inf = long.MaxValue; 
            long[,] DP = new long[n, m]; 
            
            (int w, int k)[,] prev = new (int w, int k)[n, m];
            
            // Wypełniamy tablicę nową nieskończonością
            for (int wiersz = 0; wiersz < n; wiersz++)
            {
                for (int kol = 0; kol < m; kol++)
                {
                    DP[wiersz, kol] = inf;
                }
            }
            
            DP[0, 0] = 0; 
            
            for (int wiersz = 0; wiersz < n; wiersz++)
            {
                for (int kol = 0; kol < m; kol++)
                {
                    foreach (var para in moves)
                    {
                        if (wiersz >= para.step.di && kol >= para.step.dj) 
                        {
                            int prevW = wiersz - para.step.di;
                            int prevK = kol - para.step.dj;

                            // Upewniamy się, że poprzednie pole istnieje i jest osiągalne
                            if (DP[prevW, prevK] != inf) 
                            {
                                // Nowy koszt też musi być longiem
                                long nowyKoszt = DP[prevW, prevK] + para.cost;
                                
                                if (DP[wiersz, kol] > nowyKoszt)
                                {
                                    DP[wiersz, kol] = nowyKoszt;
                                    prev[wiersz, kol] = (prevW, prevK); 
                                }
                            }
                        }
                    }
                }
            }
            
            // Szukanie najtańszego dojścia w ostatnim wierszu
            long minCost = inf;
            int bestKol = -1;

            for (int kol = 0; kol < m; kol++)
            {
                if (DP[n - 1, kol] < minCost)
                {
                    minCost = DP[n - 1, kol];
                    bestKol = kol;
                }
            }

            if (minCost == inf)
            {
                return (false, 0, null); 
            }

            // Odtwarzanie ścieżki
            List<(int i, int j)> pathList = new List<(int i, int j)>();
            int currWiersz = n - 1;
            int currKol = bestKol;

            while (currWiersz != 0 || currKol != 0)
            {
                pathList.Add((currWiersz, currKol));

                var poprzednik = prev[currWiersz, currKol];
                currWiersz = poprzednik.w;
                currKol = poprzednik.k;
            }

            pathList.Add((0, 0));
            pathList.Reverse();

            // Rzutujemy minCost z powrotem na int, bo tego wymaga sygnatura metody
            return (true, (int)minCost, pathList.ToArray()); 
        }


        /// <summary>
        /// Etap 2 - wyznaczenie najtańszej trasy, zgodnie z którą pionek przemieści się z pozycji poczatkowej (0,0) na pozycję docelową - dodatkowe założenie, każdy ruch może być wykonany co najwyżej raz
        /// </summary>
        /// <param name="n">wysokość prostokąta</param>
        /// <param name="m">szerokość prostokąta</param>
        /// <param name="moves">tablica z dostępnymi ruchami i ich kosztami (di - o ile zwiększamy numer wiersza, dj - o ile zwiększamy numer kolumnj, cost - koszt ruchu)</param>
        /// <returns>(bool result, int cost, (int, int)[] path) - result ma wartość true jeżeli trasa istnieje, false wpp., cost to minimalny koszt, path to wynikowa trasa</returns>
        public (bool result, int cost, (int i, int j)[] path) Lab02Stage2(int n, int m, ((int di, int dj) step, int cost)[] moves)
        {
            long inf = long.MaxValue;
            long[,] DP = new long[n, m];
            
            // Tablica 3D do śledzenia decyzji: [indeks_ruchu, wiersz, kolumna]
            // Powie nam "czy optymalny wynik na polu (wiersz, kol) użył k-tego ruchu?"
            bool[,,] usedMove = new bool[moves.Length, n, m];

            // Inicjalizacja nieskończonością
            for (int wiersz = 0; wiersz < n; wiersz++)
            {
                for (int kol = 0; kol < m; kol++)
                {
                    DP[wiersz, kol] = inf;
                }
            }
            
            DP[0, 0] = 0;

            // 1. ZEWNĘTRZNA PĘTLA: Bierzemy pojedynczy ruch do ręki
            for (int k = 0; k < moves.Length; k++)
            {
                var ruch = moves[k];
                
                // 2. WEWNĘTRZNE PĘTLE: Idziemy po planszy OD TYŁU!
                for (int wiersz = n - 1; wiersz >= ruch.step.di; wiersz--)
                {
                    for (int kol = m - 1; kol >= ruch.step.dj; kol--)
                    {
                        int prevW = wiersz - ruch.step.di;
                        int prevK = kol - ruch.step.dj;

                        if (DP[prevW, prevK] != inf)
                        {
                            long nowyKoszt = DP[prevW, prevK] + ruch.cost;
                            
                            if (nowyKoszt < DP[wiersz, kol])
                            {
                                DP[wiersz, kol] = nowyKoszt;
                                usedMove[k, wiersz, kol] = true; // Zaznaczamy, że ten bilet został tu użyty
                            }
                        }
                    }
                }
            }

            // 3. Szukamy najlepszego wyniku w ostatnim wierszu (analogicznie do Etapu 1)
            long minCost = inf;
            int bestKol = -1;

            for (int kol = 0; kol < m; kol++)
            {
                if (DP[n - 1, kol] < minCost)
                {
                    minCost = DP[n - 1, kol];
                    bestKol = kol;
                }
            }

            if (minCost == inf)
            {
                return (false, 0, null);
            }

            // 4. Odtwarzanie użytych ruchów
            List<int> wybraneRuchyIndeksy = new List<int>();
            int currWiersz = n - 1;
            int currKol = bestKol;

            // Sprawdzamy wszystkie ruchy od ostatniego w dół
            for (int k = moves.Length - 1; k >= 0; k--)
            {
                // Jeśli ten ruch pomógł nam dojść do obecnego pola:
                if (usedMove[k, currWiersz, currKol])
                {
                    wybraneRuchyIndeksy.Add(k); // Zapisujemy go
                    
                    // I cofamy się na planszy o wektor tego ruchu
                    currWiersz -= moves[k].step.di;
                    currKol -= moves[k].step.dj;
                }
            }

            // 5. Budowanie ostatecznej ścieżki
            // Z uwagi na to, że ruchy to tylko wektory dodatnie, możemy 
            // złożyć z nich ścieżkę w dowolnej kolejności od pola (0,0)
            List<(int i, int j)> pathList = new List<(int i, int j)>();
            int startW = 0;
            int startK = 0;
            
            pathList.Add((startW, startK)); // Zaczynamy na starcie

            // Symulujemy pionka dokładając wybrane ruchy
            foreach (int idx in wybraneRuchyIndeksy)
            {
                startW += moves[idx].step.di;
                startK += moves[idx].step.dj;
                pathList.Add((startW, startK)); 
            }

            return (true, (int)minCost, pathList.ToArray());
        }
    }
}