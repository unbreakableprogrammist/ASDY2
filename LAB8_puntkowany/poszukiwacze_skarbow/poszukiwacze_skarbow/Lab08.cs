using ASD.Graphs;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;

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

        int flow(int day, DiGraph map, int s, int t, int[] durability, int[] opensOn)
        {
            int n = durability.Length;
            DiGraph<int> res = new DiGraph<int>(n*2);
            for (int i = 0; i < n; i++)
            {
                if (opensOn[i] <= day)
                {
                    //Console.WriteLine(i.ToString(),opensOn[i].ToString());
                    res.AddEdge(i,n+i,durability[i]);
                    foreach (var neighbor in map.OutNeighbors(i))
                    {
                        res.AddEdge(i+n,neighbor,Int32.MaxValue);
                    }
                }
            }
            var(flowVal,f) = Flows.FordFulkerson(res, s,t);
            return flowVal;
        }

        public int? Stage1(DiGraph map, int startChamber, int endChamber, int[] durability, int[] opensOn,
            int expeditionSize)
        {
            int flowVal = 0;
            List<Tuple<int,int>> lista = new List<Tuple<int, int>>(opensOn.Length);
            for (int i = 0; i < opensOn.Length; i++)
            {
                lista.Add(new Tuple<int, int>(opensOn[i],i ));
            }

            lista.Sort((t1, t2) => t1.Item1.CompareTo(t2.Item1));
            int min_day = 0;
            int max_day = opensOn.Length - 1;
            int? first_availble_day = null;
            while (min_day <= max_day)
            {
                int mid = (min_day + max_day) / 2;
                flowVal = flow(lista[mid].Item1, map, startChamber, endChamber+durability.Length, durability, opensOn);
                if (flowVal >= expeditionSize)
                {
                    first_availble_day = lista[mid].Item1;
                    max_day = mid-1;
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
        public int? Stage2(DiGraph map, int startChamber, int endChamber, int[] durability)
        {
            int n =  durability.Length;
            int[] opensOn = new int[n];
            Array.Fill<int>(opensOn, 0);
            int must_flow = flow(0,map,startChamber,endChamber,durability,opensOn);
            NetworkWithCosts<int, int> graf = new NetworkWithCosts<int, int>(durability.Length);
            for (int i = 0; i < n; i++)
            {
                graf.AddEdge(i,n+i,durability[i],-1);
                foreach (var neighbor in map.OutNeighbors(i))
                {
                    graf.AddEdge(n+i,neighbor,Int32.MaxValue,0);
                }
            }

            int min_expedition = 0;
            int mid;
            int max_expedition = must_flow;
            int? best = null;
            var(max_flow , min_cost,f) = Flows.MinCostMaxFlow(graf,startChamber,endChamber);
            while (min_expedition <= max_expedition)
            {
                mid = (max_expedition + min_expedition) / 2;
                var(now_flow,)
            }
            return best;
        }
    }
}