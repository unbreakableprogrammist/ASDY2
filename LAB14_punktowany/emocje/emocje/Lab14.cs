using System;

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
            string conct = wzorzec+"$"+slowo;
            p[0] = 0;
            p[1] = 0;
            int lenght = conct.Length;
            int t = 0;
            for (int j = 2; j <= lenght; j++) 
            {
                t = p[j - 1]; 
                while (t > 0 && conct[j-1] != conct[t]) // dopoki nasze s[j-1] != s[t]
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
            int[] p  = new int[pattern.Length + 1 + text.Length + 1];
            KMP(p, pattern, text);
            /*Console.WriteLine(pattern,text);
            foreach (var wart in p)
            {
                Console.WriteLine(wart);
            }*/
            
            // 
            int[] diff = new int[text.Length+1];
            int m = pattern.Length;
            Array.Fill(diff,0);
            for (int j = m + 1; j < p.Length; j++)
            {
                if (p[j] == m)
                {
                    int end = j - m - 2;
                    int start = end - m + 1;
                    diff[start]++;
                    diff[end+1]--;
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
            result = text.Substring(Math.Max(0,bestEnd-maxlen),maxlen);
            return maxlen;
        }
		
        /// <summary>Etap II</summary>
        /// <param name="word">Wejściowe słowo.</param>
        /// <param name="result">Najkrótsze podsłowo pokrywające `word`.</param>
        /// <returns>Długość najkrótszego podsłowa pokrywającego `word`.</returns>
        public int Stage2(string word, out string result)
        {
            int answ = word.Length;
            int zwroc = 0;
            
            for (int i = 0; i <= word.Length; i++)
            {
                string wzorzec = word.Substring(0, i);
                string  res= null;
                answ = Stage1(word,wzorzec,out res);
                if (answ == word.Length)
                {
                    zwroc = i;
                    break;
                }
            }
            
            result = word.Substring(0,zwroc);
            return zwroc;
        }
    }
}