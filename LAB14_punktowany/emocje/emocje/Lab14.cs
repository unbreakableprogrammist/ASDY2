using System;
using System.Reflection;

namespace ASD
{
    public class Lab14 : MarshalByRefObject
    {
        /// <summary>Etap I</summary>
        /// <param name="text">Wejściowy ciąg znaków</param>
        /// <param name="pattern">Wzorzec do wyszukania najdłuższego pokrytego podsłowa ciągu `text` przez niego.</param>
        /// <param name="result">Najdłuższe podsłowo `text` pokryte przez `pattern`.</param>
        /// <returns>Długość najdłuższego podsłowa `text` pokrytego przez `pattern`.</returns>
        public static void KMP(int[] p, string wzorzec, string slowo)
        {
            string conct = wzorzec + "$" + slowo;
            p[0] = 0;
            p[1] = 0;
            int lenght = conct.Length;
            int t = 0;
            for (int j = 2; j <= lenght; j++) 
            {
                t = p[j - 1]; 
                while (t > 0 && conct[j-1] != conct[t]) 
                {
                    t = p[t]; 
                }

                if (conct[t] == conct[j-1])
                {
                    t++;
                }
                p[j] = t;
            }
        }
        
        public int Stage1(string text, string pattern, out string result)
        {
            if (string.IsNullOrEmpty(pattern) || string.IsNullOrEmpty(text))
            {
                result = "";
                return 0;
            }

            int[] p = new int[pattern.Length + 1 + text.Length + 1];
            KMP(p, pattern, text);
            
            int[] diff = new int[text.Length + 1];
            int m = pattern.Length;
            
            for (int i = 0; i < diff.Length; i++)
            {
                diff[i] = 0;
            }

            for (int j = m + 1; j < p.Length; j++)
            {
                if (p[j] == m)
                {
                    int end = j - m - 2;
                    int start = end - m + 1;
                    diff[start]++;
                    diff[end + 1]--;
                }
            }

            int maxlen = 0;
            int currLen = 0;
            int bestEnd = -1;
            int sum = 0;
            result = null;
            bool started = false;
            
            for (int i = 0; i < diff.Length; i++)
            {
                sum += diff[i];
                if (sum > 0)
                {
                    started = true;
                    currLen++;
                }

                if (started && sum == 0)
                {
                    started = false;
                    if (maxlen < currLen)
                    {
                        bestEnd = i;
                        maxlen = currLen;
                    }

                    currLen = 0;
                }
            }
            
            if (bestEnd == -1)
            {
                result = "";
                return 0;
            }

            result = text.Substring(Math.Max(0, bestEnd - maxlen), maxlen);
            return maxlen;
        }

        /// <summary>Etap II</summary>
        /// <param name="word">Wejściowe słowo.</param>
        /// <param name="result">Najkrótsze podsłowo pokrywające `word`.</param>
        /// <returns>Długość najkrótszego podsłowa pokrywającego `word`.</returns>

        public static void ComputeP(string w, int[] P)
        {
            P[0] = 0;
            int t = 0;
            for (int j = 1; j < w.Length; j++)
            {
                // tutaj t dlugosc najwiekszego prefikso-sufiksu czyli jak liczymy od 0 to tak na prawde nastepna literka w slowie
                while (t > 0 && w[j] != w[t])
                {
                    t = P[t]; // skaczemy na nowa literke 
                }
                if (w[j] == w[t])
                {
                    t++;
                }
                P[j+1] = t;
            }

        }
        public int Stage2(string word, out string result)
        {
            int[] P = new int[word.Length+1]; // prefikso-sufiksy
            int[] DP = new int[word.Length + 1]; // tablica taka ze w DP[i] - trzymamy najkrosze dopasowanie takie ze pokrywa prefiks slowa w o dl i
            int[] max_covered = new int[word.Length + 1]; // trzyma dla prefiksu dliugosci i do ktorego moemntu w slowie ma pokryciue 
            DP[0] = 0;
            max_covered[0] = 0;
            ComputeP(word, P);

            for (int i = 1; i <= word.Length; i++)
            {
                int p = P[i];
                int k = DP[p]; // najlepsze dopasownie takie ze pokrywa p 
                if (max_covered[k] >= i - p) // jesli nasz kandydat k pokrywa dalej niz i - p to jest naszym   ( bo pokrywa tez p) 
                {
                    DP[i] = k;
                }
                else
                {
                    DP[i] = i; // jesli k nie pokrywa dalej niz i - p to cale slowo to nasz kandydat 
                }

                max_covered[DP[i]] = i; // jesli nasz k zadzialal to przedluzamy mu cover jesli nie to ustawiamy nowy 
            }
            
            int answ = DP[word.Length]; // nasza odpowiedza jest dla slowa rownymn jego dlugosci 
            result = word.Substring(0, answ);
            return result.Length;
        }
    }
}