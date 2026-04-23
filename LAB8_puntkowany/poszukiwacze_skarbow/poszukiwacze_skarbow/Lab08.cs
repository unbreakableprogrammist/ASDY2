using ASD.Graphs;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;

namespace ASD2
{
    public class TreasureTrackers : MarshalByRefObject
    {
        /// <summary>
        /// Etap I: Wybór dnia ekspedycji.
        /// Wyznaczenie pierwszego dnia, w którym cała ekspedycja będzie w stanie
        /// przejść przez podziemia.
        /// </summary>
        /// <param name="map">Graf skierowany reprezentujący połączenia pomiędzy komnatami w podziemiach.</param>
        /// <param name="startChamber">Wierzchołek będący wejściem do podziemi.</param>
        /// <param name="endChamber">Wierzchołek będący wyjściem z podziemi.</param>
        /// <param name="durability">Tablica utrzymująca wytrzymałość każdej komnaty.</param>
        /// <param name="opensOn">Tablica informująca, którego dnia otwiera się dana komnata.</param>
        /// <param name="expeditionSize">Rozmiar ekspedycji, chcącej przejść przez podziemia.</param>

        int flow(int day, DiGraph map, int s, int t, int[] durability, int[] opensOn, int expeditionSize)
        {
            int n = durability.Length;
            DiGraph<int> res = new DiGraph<int>(n * 2);
            for (int i = 0; i < n; i++)
            {
                if (opensOn[i] <= day)
                {
                    int capacity = durability[i];
                    if (i == s) 
                    {
                        capacity = Math.Min(capacity, expeditionSize);
                    }
            
                    res.AddEdge(i, n + i, capacity);
            
                    foreach (var neighbor in map.OutNeighbors(i))
                    {
                        res.AddEdge(i + n, neighbor, expeditionSize);
                    }
                }
            }
            var (flowVal, f) = Flows.FordFulkerson(res, s, t);
            return flowVal;
        }

        public int? Stage1(DiGraph map, int startChamber, int endChamber, int[] durability, int[] opensOn, int expeditionSize)
        {
            int[] uniqueDays = opensOn.Distinct().OrderBy(d => d).ToArray();
            int min_day = 0;
            int max_day = uniqueDays.Length - 1;
            int? first_availble_day = null;
            while (min_day <= max_day)
            {
                int mid = (min_day + max_day) / 2;
                int currentDay = uniqueDays[mid]; 
                int flowVal = flow(currentDay, map, startChamber, endChamber + durability.Length, durability, opensOn, expeditionSize);
        
                if (flowVal >= expeditionSize)
                {
                    first_availble_day = currentDay;
                    max_day = mid - 1;
                }
                else
                {
                    min_day = mid + 1;
                }
            }
            return first_availble_day;
        }

        /// <summary>
        /// Etap II: 
        /// Wyznaczenie minimalnej liczby poszukiwaczy skarbów,
        /// która będzie w stanie zebrać wszystkie skarby.
        /// </summary>
        /// <param name="map">Acykliczny graf skierowany reprezentujący połączenia pomiędzy komnatami w podziemiach.</param>
        /// <param name="startChamber">Wierzchołek będący wejściem do podziemi.</param>
        /// <param name="endChamber">Wierzchołek będący wyjściem z podziemi.</param>
        /// <param name="durability">Tablica utrzymująca wytrzymałość każdej komnaty.</param>
        NetworkWithCosts<int, int> buildNetwork(int mid, DiGraph map, int startChamber, int[] durability)
        {
            int n = durability.Length;
            NetworkWithCosts<int, int> res = new NetworkWithCosts<int, int>(3 * n + 1);
            int SS = 3 * n;

            for (int i = 0; i < n; i++)
            {
                int capacity = Math.Min(durability[i], mid);

                res.AddEdge(i, i + n, 1, -1);

                if (capacity > 1)
                {
                    res.AddEdge(i, i + 2 * n, capacity - 1, 0);
                    res.AddEdge(i + 2 * n, i + n, capacity - 1, 0);
                }

                foreach (var neighbor in map.OutNeighbors(i))
                {
                    res.AddEdge(i + n, neighbor, mid, 0);
                }
            }
            res.AddEdge(SS, startChamber, mid, 0);
            return res;
        }
        int maxflow(DiGraph map, int startChamber, int endChamber, int[] durability)
        {
            int n = durability.Length;
            DiGraph<int> res = new DiGraph<int>(2 * n);
            for (int i = 0; i < n; i++)
            {
                res.AddEdge(i, i + n, durability[i]);
                foreach (var neighbor in map.OutNeighbors(i))
                {
                    res.AddEdge(i + n, neighbor, Math.Min(durability[startChamber],durability[i]));
                }
            }
    
            var (flowVal, f) = Flows.FordFulkerson(res, startChamber, endChamber + n);
            return flowVal;
        }
        public int? Stage2(DiGraph map, int startChamber, int endChamber, int[] durability)
        {
            int n = durability.Length;
            int left = 1;
            int right = maxflow(map, startChamber, endChamber, durability);
            int? odp = null;

            while (left <= right)
            {
                int mid = (left + right) / 2;
                int SS = 3 * n;

                NetworkWithCosts<int, int> res = buildNetwork(mid, map, startChamber, durability);
                var (capacityVal, cost, f) = Flows.MinCostMaxFlow(res, SS, endChamber + n);
                if (cost == -n)
                {
                    odp = mid;
                    right = mid - 1;
                }
                else
                {
                    left = mid + 1;
                }
            }

            return odp;
        
        }
    }
}