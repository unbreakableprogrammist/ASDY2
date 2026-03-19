using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASD
{
    public class Lab02 : MarshalByRefObject
    {
        /// <summary>
        /// Optymalne rozmieszczenie parasolek w wariancie, w którym każda parasolka ma taki sam promień
        /// oraz mamy do dyspozycji tylko zadaną liczbę parasolek (rozmieszczenie parasolek nie wiąże się z żadnym kosztem)
        /// </summary>
        /// <param name="Z">Tablica zysków, Z[i] to zysk za pokrycie punktu o numerze i</param>
        /// <param name="umbrellaCount">Liczba dostępnych parasolek</param>
        /// <param name="umbrellaRadius">Promień parasolki (parasolka o promieniu r umieszczona w punkcie i pokrywa punkty i-r, i-r+1, ..., i+r)</param>
        /// <returns></returns>
        public (int profit, int[] umbrellaPosition) Stage1(int[] Z, int umbrellaCount, int umbrellaRadius)
        {
            // na poczatku suma prefiksowa po proficie 
            int[] suma_pref = new int[Z.Length];
            suma_pref[0] = Z[0];
            for (int i = 1; i < Z.Length; i++)
            {
                suma_pref[i] = Z[i]+suma_pref[i-1];
            }
            int[] profit = new int[Z.Length]; // profit okresla jaki zysk z tego mamy 
            for (int i = 0; i < Z.Length; i++)
            {
                int L = Math.Max(0, i - umbrellaRadius);
                int R = Math.Min(Z.Length - 1, i + umbrellaRadius);

                if (L == 0) 
                {
                    // jesli lewa strona wychodzimy za mape
                    profit[i] = suma_pref[R];
                }
                else 
                {
                    profit[i] = suma_pref[R] - suma_pref[L - 1];
                }
            }
            int[,] DP = new int[umbrellaCount+1, Z.Length];
            for (int k = 0; k <= umbrellaCount; k++) // ile parasolek uzywamy 
            {
                for (int i = 0; i < Z.Length; i++)
                {
                    DP[k, i] = 0;
                }
            }

            for (int k = 1; k <= umbrellaCount; k++) // ile parasolek uzywamy 
            {
                for (int i = 0; i < Z.Length; i++)
                {
                    if (i == 0) // jesli jestesmy na pierwszej pozycji 
                    {
                        DP[k,i] = Math.Max(DP[k-1,i],profit[i]); 
                    }
                    else
                    {
                        int opcja1 = DP[k - 1, i]; // nie bierzemy parasolki
                        int opcja2 = DP[k, i-1]; // nie ustawiamy parasolki
                        int opcja3 = profit[i]; // ustawiamy parasolke
                        if (i - 2 * umbrellaRadius - 1 >= 0) // jesli sie da wziasc cos z lewej
                        {
                            opcja3 += DP[k - 1, i - 2 * umbrellaRadius - 1];
                        }
                        DP[k,i] = Math.Max(opcja1, Math.Max(opcja2,opcja3));
                    }
                }
            }
            int[] umbrellaPosition = new int[umbrellaCount];
            // teraz musimy oddtwarzac to zadanie, 
            int par = umbrellaCount;
            int pos = Z.Length - 1;
            int INDX = 0;
            while (par > 0 && pos >= 0)
            {
                if (pos > 0 && DP[par, pos] == DP[par, pos - 1] )
                {
                    pos -= 1;
                }else if (DP[par, pos] == DP[par - 1, pos])
                {
                    par -= 1;
                }
                else
                {
                    umbrellaPosition[INDX] = pos;
                    pos -= (umbrellaRadius*2+1);
                    par -= 1;
                    INDX++;
                }
            }
            return (DP[umbrellaCount,Z.Length-1],umbrellaPosition);
        }


        /// <summary>
        /// Optymalne rozmieszczenie parasolek w wariancie, w którym mamy dostępne modele parasolek o różnych promieniach.
        /// Każdego modelu możemy użyć dowolną liczbę razy, jednak za każdym razem musimy ponieść jego koszt.
        /// </summary>
        /// <param name="Z">Tablica zysków, Z[i] to zysk za pokrycie punktu o numerze i</param>
        /// <param name="umbrellaType">Tablice dostępnych modeli parasolek, gdzie i-ty model ma promień umbrellaType[i].radius i koszt umbrellaType[i].cost</param>
        /// <returns></returns>
        public (int profit, (int position, int model)[] umbrellas) Stage2(int[] Z, (int radius, int cost)[] umbrellaType)
        {
            int n = Z.Length;
            if (n == 0) return (0, new (int, int)[0]);
            int[] profit = new int[n + 1];
            profit[0] = 0; 
            for (int i = 0; i < n; i++)
            {
                profit[i + 1] = profit[i] + Z[i];
            }

            int[] DP = new int[n + 1];
            int[] prev = new int[n + 1]; // do jakiego indeksu mamy sie cofnac
            int[] chosenModel = new int[n + 1]; // jaki model
            int[] chosenPos = new int[n + 1]; // jaka pozycja
            DP[0] = 0;
    
            for (int j = 1; j <= n; j++)
            {
                DP[j] = DP[j - 1]; 
                prev[j] = j-1; // bo nie stawiamy parasolki przez nas wiec po prostu kopiujemy od lewa
                chosenModel[j] = -1; // nie wybieramy modelu
                
                for (int i = 0; i < umbrellaType.Length; i++)
                {
                    int r = umbrellaType[i].radius;
                    int cost = umbrellaType[i].cost;

                    int mid = (j - 1) - r; 
                    int l = (j - 1) - 2 * r;
                    int left_indx = Math.Max(0, l); 
                    int safe_mid = Math.Max(0, Math.Min(n - 1, mid));
                    int zysk_z_parasolki = profit[j] - profit[left_indx]; 
            
                    int zysk = DP[left_indx] + zysk_z_parasolki - cost;
            
                    if (zysk > DP[j]) 
                    {
                        DP[j] = zysk;
                        prev[j] = left_indx;
                        chosenModel[j] = i;
                        chosenPos[j] = safe_mid;
                    }
                }
            }
            List<(int position, int model)> umbrellasList = new List<(int position, int model)>();

            int curr = n;

            while (curr > 0)
            {
                if (chosenModel[curr] != -1)
                {
                    umbrellasList.Add((chosenPos[curr], chosenModel[curr]));
                }

                curr = prev[curr];
            }
            umbrellasList.Reverse();
            return (DP[n], umbrellasList.ToArray());
        }
    }
}