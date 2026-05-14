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
            ///
            /// pomysl : idziemy miotla od lewa do prawa w kazdym evencie wybieramy najmniejszy punkt i najwiekszy i wyliczamyu
            /// maksimum i minimum ( w czasie K ) 
            int K = samples.Length;
            bool all_done = false;
            int[] indexes = new int[K];
            int[] dlugosci = new int[K];
            for (int i = 0; i < K; i++)
            {
                dlugosci[i] = samples[i].Length;
            }
            Array.Fill(indexes, 0);
            double max_wynik = 0.0;
            while (!all_done)
            {
                double curr_wynik = 0.0;
                double min_value = double.MaxValue;
                int min_index = -1;
                double maks_value = 0.0;
                int max_index = -1;
                for (int i = 0; i < K; i++)
                {
                    if ((double)indexes[i] / dlugosci[i] < min_value)
                    {
                        min_value = (double)indexes[i] / dlugosci[i];
                        min_index = i;
                    }

                    if ((double)indexes[i] / dlugosci[i] > maks_value)
                    {
                        maks_value = (double)indexes[i] / dlugosci[i];
                        max_index = i;
                    }
                }
                curr_wynik = Math.Max(max_wynik, curr_wynik);
                max_wynik = Math.Max(max_wynik, curr_wynik);
                all_done = true;
                for (int i = 0; i < K; i++)
                {
                    if (indexes[i] <= min_index) indexes[i]++;
                    if(indexes[i] < dlugosci[i]) all_done = false; 
                }
            }

            return max_wynik;
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
            return -1.0;
        }
    }
}