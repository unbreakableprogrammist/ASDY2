using System;
using ASD.Graphs;
using ASD;
using System.Collections.Generic;

namespace ASD
{

    public class Lab04 : System.MarshalByRefObject
    {
        /// <summary>
        /// Etap 1 - szukanie trasy z miasta start_v do miasta end_v, startując w dniu day
        /// </summary>
        /// <param name="g">Ważony graf skierowany będący mapą</param>
        /// <param name="start_v">Indeks wierzchołka odpowiadającego miastu startowemu</param>
        /// <param name="end_v">Indeks wierzchołka odpowiadającego miastu docelowemu</param>
        /// <param name="day">Dzień startu (w tym dniu należy wyruszyć z miasta startowego)</param>
        /// <param name="days_number">Liczba dni uwzględnionych w rozkładzie (tzn. wagi krawędzi są z przedziału [0, days_number-1])</param>
        /// <returns>(result, route) - result ma wartość true gdy podróż jest możliwa, wpp. false, 
        /// route to tablica z indeksami kolejno odwiedzanych miast (pierwszy indeks to indeks miasta startowego, ostatni to indeks miasta docelowego),
        /// jeżeli result == false to route ustawiamy na null</returns>
        Tuple<bool,int> BFS(List<int>[,] tab, int v, int end, int days_number, int my_day ,bool[,] odwiedzone,int[,] trasa,out int final_day)
        {
            final_day = 0;
            Queue<Tuple<int,int>> queue = new Queue<Tuple<int,int>>(); // kolejka do BFS
            queue.Enqueue(new Tuple<int,int>(v, my_day));
            bool result = false;
            odwiedzone[v, my_day] = true;
            while (queue.Count > 0) // dopoki cos jest na kolejce 
            {
                Tuple<int,int> tuple = queue.Dequeue();
                v =  tuple.Item1;
                int d = tuple.Item2;
                if (end == v)
                {
                    final_day = d;
                    result = true;
                    break;
                }
                
                // tab[6,1] = "3" 
                // tab[6,0[ = "4"
                int next_day = (d+1) % days_number;
                // idziemy po wszystkich sasiadach 
                foreach (int u in tab[v,d])
                {
                    if (!odwiedzone[u, next_day])
                    {
                        odwiedzone[u, next_day] = true;
                        queue.Enqueue(new Tuple<int,int>(u, next_day));
                        trasa[u, next_day] = v;
                    }
                }
            }
            return new Tuple<bool, int>(result,final_day);
        }
        public (bool result, int[] route) Lab04_FindRoute(DiGraph<int> g, int start_v, int end_v, int day, int days_number)
        {
            bool[,]odwiedzone = new bool[g.VertexCount, days_number]; // ta tablica bedzie oznaczac czy danego dnia juz odwiedzilismy wierzcholek
            List<int>[,] tab= new List<int>[g.VertexCount,days_number]; // to bedzie tablica ze tab[v,d] = lista wierzcholkow ktore mozna odwiedzic z v w d-tym dniu
            int[,] trasa = new int[g.VertexCount,days_number];
            for (int i = 0; i < g.VertexCount; i++)
            {
                for (int j = 0; j < days_number; j++)
                {
                    tab[i, j] = new List<int>();
                }
            }

            for (int i = 0; i < g.VertexCount; i++)
            {
                foreach (var edge in g.OutEdges(i))
                {
                    tab[i,edge.Weight].Add(edge.To);
                }
            }

            int final_day = 0;
            Tuple<bool,int> t = BFS(tab, start_v, end_v, days_number, day,odwiedzone,trasa,out final_day);
            bool zwroc = t.Item1;
            final_day = t.Item2;
            if (zwroc)
            {
                List<int> trip = new List<int>();
                int current_day = final_day;
                int current_vert = end_v;
                while (current_vert != start_v || current_day != day)
                {
                    trip.Add(current_vert);
                    int prev_vert = trasa[current_vert, current_day];
                    current_day = (current_day - 1 + days_number) % days_number;
                    current_vert = prev_vert;
                }
                trip.Add(start_v); 
                trip.Reverse(); 
                return (true, trip.ToArray());
            }
            return (zwroc, null);
            
        }
        
        /// <summary>
        /// Etap 2 - szukanie trasy z jednego z miast z tablicy start_v do jednego z miast z tablicy end_v (startować można w dowolnym dniu)
        /// </summary>
        /// <param name="g">Ważony graf skierowany będący mapą</param>
        /// <param name="start_v">Tablica z indeksami wierzchołków startowych (trasę trzeba zacząć w jednym z nich)</param>
        /// <param name="end_v">Tablica z indeksami wierzchołków docelowych (trasę trzeba zakończyć w jednym z nich)</param>
        /// <param name="days_number">Liczba dni uwzględnionych w rozkładzie (tzn. wagi krawędzi są z przedziału [0, days_number-1])</param>
        /// <returns>(result, route) - result ma wartość true gdy podróż jest możliwa, wpp. false, 
        /// route to tablica z indeksami kolejno odwiedzanych miast (pierwszy indeks to indeks miasta startowego, ostatni to indeks miasta docelowego),
        /// jeżeli result == false to route ustawiamy na null</returns>
        
        
        Tuple<bool, int, int> BFS2(List<int>[,] tab, int[] starts, HashSet<int> ends, int days_number, bool[,] odwiedzone, int[,] trasa)
        {
            Queue<Tuple<int,int>> queue = new Queue<Tuple<int,int>>(); 
            foreach (int v in starts)
            {
                for (int d = 0; d < days_number; d++)
                {
                    queue.Enqueue(new Tuple<int,int>(v,d));
                    odwiedzone[v,d] = true;
                    trasa[v, d] = -1; 
                }
            }
    
            while (queue.Count > 0) 
            {
                Tuple<int,int> tuple = queue.Dequeue();
                int v =  tuple.Item1;
                int d = tuple.Item2;
        
                if (ends.Contains(v)) // trafiliśmy do któregoś z miast docelowych
                {
                    return new Tuple<bool, int, int>(true, v, d);
                }
        
                int next_day = (d+1) % days_number;
                foreach (int u in tab[v,d])
                {
                    if (!odwiedzone[u, next_day])
                    {
                        odwiedzone[u, next_day] = true;
                        queue.Enqueue(new Tuple<int,int>(u, next_day));
                        trasa[u, next_day] = v;
                    }
                }
            }
            return new Tuple<bool, int, int>(false, -1, -1);
        }
        public (bool result, int[] route) Lab04_FindRouteSets(DiGraph<int> g, int[] start_v, int[] end_v, int days_number)
        {
            bool[,]odwiedzone = new bool[g.VertexCount, days_number]; 
            List<int>[,] tab= new List<int>[g.VertexCount,days_number]; 
            int[,] trasa = new int[g.VertexCount,days_number];
            for (int i = 0; i < g.VertexCount; i++)
            {
                for (int j = 0; j < days_number; j++)
                {
                    tab[i, j] = new List<int>();
                }
            }

            for (int i = 0; i < g.VertexCount; i++)
            {
                foreach (var edge in g.OutEdges(i))
                {
                    tab[i,edge.Weight].Add(edge.To);
                }
            }
            // przerzucamy konce do hashsetu 
            HashSet<int> konce = new HashSet<int>(end_v);
            
            var res = BFS2(tab,start_v,konce,days_number,odwiedzone,trasa);
            
            bool result = res.Item1;
            int which_end = res.Item2;
            int final_day = res.Item3;
            
            if (result)
            {
                List<int> trip = new List<int>();
                int current_day = final_day;
                int current_vert = which_end;
        
                while (current_vert != -1)
                {
                    trip.Add(current_vert);
                    int prev_vert = trasa[current_vert, current_day];
            
                    if (prev_vert == -1) 
                    {
                        break; 
                    }
                    current_day = (current_day - 1 + days_number) % days_number;
                    current_vert = prev_vert;
                }
        
                trip.Reverse(); 
                return (true, trip.ToArray());
            }
    
            return (false, null);
            
        }
    }
}
