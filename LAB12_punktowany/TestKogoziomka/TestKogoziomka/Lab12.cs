using System;
using System.Collections.Generic;
using ASD.Graphs;

namespace ASD
{
    public class Lab12 : MarshalByRefObject
    {
        /// <summary>
        /// Etap 1:
        /// Obliczenie statystyki Kołmogorowa-Smirnowa dla dwóch uporządkowanych
        /// niemalejąco próbek.
        ///
        /// Rozwiązanie powinno działać w czasie O(n1 + n2),
        /// gdzie n1 i n2 są długościami odpowiednich próbek.
        /// </summary>
        /// <param name="sample1">Pierwsza próbka, uporządkowana niemalejąco.</param>
        /// <param name="sample2">Druga próbka, uporządkowana niemalejąco.</param>
        /// <returns>
        /// Wartość statystyki D = sup_x |F_1(x) - F_2(x)|.
        /// </returns>
        public double Stage1(double[] sample1, double[] sample2)
        {
           int n = sample1.Length;
           int m = sample2.Length;
           int i = 0;
           int j = 0;
           double max_wynik = 0.0;
           while (i < n || j < m)
           {
               double val;
               if (i < n && j < m) 
                   val = Math.Min(sample1[i], sample2[j]);
               else if (i < n) 
                   val = sample1[i];
               else 
                   val = sample2[j];
               while (i < n && sample1[i] <= val) 
               {
                   i++;
               }
               while (j < m && sample2[j] <= val) 
               {
                   j++;
               }
               double curr_value = Math.Abs((double)i / n - (double)j / m);
               max_wynik = Math.Max(max_wynik, curr_value);
           }
           return max_wynik;
        }

        /// <summary>
        /// Etap 2:
        /// Obliczenie największej odległości pomiędzy wartościami dowolnych dwóch
        /// empirycznych dystrybuant w tym samym punkcie x, dla K próbek.
        ///
        /// Rozwiązanie powinno działać w czasie O(nK), gdzie n jest łączną długością
        /// wszystkich próbek, a K jest liczbą próbek.
        /// </summary>
        /// <param name="samples">
        /// Tablica K próbek. Dla i = 0, 1, ..., K - 1 próbka samples[i]
        /// ma długość n_i i jest uporządkowana niemalejąco.
        /// </param>
        /// <returns>
        /// Wartość statystyki D = sup_x (max_i F_i(x) - min_j F_j(x)).
        /// </returns>
        public double Stage2(double[][] samples)
        {
            int K = samples.Length;
            int[] indexes = new int[K];
            for (int i = 0; i < K; i++)
            {
                indexes[i] = 0;
            }
            bool all_done = false;
            double D = 0.0;
            while (!all_done)
            {
                double min_value = Double.MaxValue;
                all_done = true;
                for (int i = 0; i < K; i++)
                {
                    if (indexes[i] < samples[i].Length) // jesli gdzies nie doszlismy do konca
                    {
                        all_done = false;
                        if (min_value > samples[i][indexes[i]])
                        {
                            min_value = samples[i][indexes[i]];
                        }
                    }
                }
                if(all_done) break;
                
                // teraz przesuwamy pozostale indeksy na nastepna wartosc 
                for (int i = 0; i < K; i++)
                {
                    // przesuwamy indeksy tam gdzie bylismy na najmniejszej wartosci
                    while (indexes[i] < samples[i].Length && samples[i][indexes[i]] == min_value)
                    {
                        indexes[i]++;
                    }
                }
                double min_val = double.MaxValue; // pojemnik na najmniejsza wartosc 
                double max_val = double.MinValue;
                for (int i = 0; i < K; i++)
                {
                    double wart_dyst = (double)indexes[i] / samples[i].Length;
                    
                    min_val = Math.Min(min_val, wart_dyst);
                    max_val = Math.Max(max_val, wart_dyst);
                }
                D = Math.Max(D, max_val - min_val);
            }
            return D;
        }

        /// <summary>
        /// Etap 3:
        /// Obliczenie tej samej statystyki co w Etapie 2, ale z lepszą złożonością.
        ///
        /// Rozwiązanie powinno działać w czasie O(n log K), gdzie n jest łączną długością
        /// wszystkich próbek, a K jest liczbą próbek.
        /// </summary>
        /// <param name="samples">
        /// Tablica K próbek. Dla i = 0, 1, ..., K - 1 próbka samples[i]
        /// ma długość n_i i jest uporządkowana niemalejąco.
        /// </param>
        /// <returns>
        /// Wartość statystyki D = sup_x (max_i F_i(x) - min_j F_j(x)).
        /// </returns>
        public double Stage3(double[][] samples)
        {
            int K = samples.Length;
            int[] indexes = new int[K];
            // najpierw porownuje po double a pozniej po wartosci 
            var comparer = Comparer<(double val, int id)>.Create((a, b) => {
                int cmp = a.val.CompareTo(b.val);
                if (cmp == 0) return a.id.CompareTo(b.id);
                return cmp;
            });

            SortedSet<(double val, int id)> xSet = new SortedSet<(double val, int id)>(comparer); // x posortowane(nastepne dane wejsciowe) 
            SortedSet<(double fraction, int id)> ySet = new SortedSet<(double fraction, int id)>(comparer); // y (wartosci dystrybuant ) 

            for (int i = 0; i < K; i++)
            {
                indexes[i] = 0; // na poczatku wszystkie indeksy na 0
                ySet.Add((0.0, i)); // na poczatku kazda dystrybuant jest na 0
                xSet.Add((samples[i][0], i));
            }
            
            double D = 0.0;
            while (xSet.Count > 0)
            {
                // najmniejszy x
                double min_x = xSet.Min.val;
                // teraz bedziemy patrzec w jakich listach jeszcze musimy sie zupdateowac indeks ( zdejmujemy po porstu te  min_x)
                List<int> arraysToUpdate = new List<int>();
                while(xSet.Count > 0 && xSet.Min.val <= min_x)
                {
                    var min_x_elem = xSet.Min;
                    xSet.Remove(min_x_elem);
                    arraysToUpdate.Add(min_x_elem.id);
                }

                foreach (int i in arraysToUpdate)
                {
                    // patrzymy na stara dystrybuante i ja wywalamy z ySet 
                    double stara_dyst = (double)indexes[i] / samples[i].Length;
                    ySet.Remove((stara_dyst, i));
                    // przesuwamy indeksy tam gdzie mniejsza wartosc jest
                    while (indexes[i] < samples[i].Length && samples[i][indexes[i]] <= min_x)
                    {
                        indexes[i]++;
                    }
                    double nowaDystr = (double)indexes[i] / samples[i].Length;
                    ySet.Add((nowaDystr, i));
                    if (indexes[i] < samples[i].Length)
                    {
                        xSet.Add((samples[i][indexes[i]], i));
                    }
                }
                double current_min_F = ySet.Min.fraction;
                double current_max_F = ySet.Max.fraction;
                D = Math.Max(D, current_max_F - current_min_F);
            }
            return D;
        }
    }
}