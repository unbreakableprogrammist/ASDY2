using System;
using System.Collections.Generic;

namespace ASD
{
    
    //   aababab
    //  00101010
    public class LZ77 : MarshalByRefObject
    {
        
        // ==========================================================
        // 1. DEKODOWANIE (Super-szybkie na tablicy char[])
        // ==========================================================
        public string Decode(List<EncodingTriple> encoding)
        {
            // Najpierw liczymy dokładny rozmiar pliku
            int totalLength = 0;
            for (int i = 0; i < encoding.Count; i++)
            {
                totalLength += encoding[i].c + 1;
            }

            // Tworzymy surową tablicę o idealnym rozmiarze
            char[] res = new char[totalLength];
            int currentPos = 0;

            foreach (var triple in encoding)
            {
                int p = triple.p;
                int c = triple.c;
                char s = triple.s;

                int start = currentPos - p - 1;

                // Błyskawiczne kopiowanie w pamięci 
                for (int i = 0; i < c; i++)
                {
                    res[currentPos] = res[start + i];
                    currentPos++;
                }
                
                // Doklejenie nowej litery
                res[currentPos] = s;
                currentPos++;
            }

            // Zamiana tablicy na string
            return new string(res);
        }

        // ==========================================================
        // 2. SZUKACZ KMP (Zero Substringów, czysta praca na indeksach)
        // ==========================================================
        public static void SzukajKMP(string s, int w_start, int current_index, int[] P, out int najlepszy_poczatek, out int najlepsze_c)
        {
            najlepszy_poczatek = 0;
            najlepsze_c = 0;

            int n = s.Length;
            int r_len = n - current_index; 
            
            if (r_len == 0) return;

            int w_len = current_index - w_start; 
            int wr_len = n - w_start; // Zamiast długości sklejonego stringa

            int matched = 0;       
            int p_policzone = 0;   

            // Spacer po wirtualnie sklejonym tekście
            for (int i = 0; i < wr_len; i++)
            {
                // ETAP 1: Leniwy budowniczy
                while (p_policzone <= matched && p_policzone < r_len)
                {
                    if (p_policzone == 0)
                    {
                        P[0] = 0;
                    }
                    else
                    {
                        int k = P[p_policzone - 1]; 
                        while (k > 0 && s[current_index + p_policzone] != s[current_index + k])
                        {
                            k = P[k - 1]; 
                        }
                        if (s[current_index + p_policzone] == s[current_index + k])
                        {
                            k++;
                        }
                        P[p_policzone] = k;
                    }
                    p_policzone++; 
                }

                // ETAP 2: Mechanizm cofania (na podstawie oryginalnego tekstu)
                while (matched > 0 && s[w_start + i] != s[current_index + matched])
                {
                    matched = P[matched - 1]; 
                }

                if (s[w_start + i] == s[current_index + matched])
                {
                    matched++;
                }

                // ETAP 3: Bariera historii LZ77
                int aktualny_pocz = i - matched + 1;

                if (aktualny_pocz < w_len)
                {
                    if (matched > najlepsze_c)
                    {
                        najlepsze_c = matched;
                        najlepszy_poczatek = aktualny_pocz;
                    }
                }
                else
                {
                    break; // Przekroczyliśmy granicę historii
                }

                // Zabezpieczenie przed końcem wzorca
                if (matched == r_len)
                {
                    matched = P[matched - 1];
                }
            }
        }

        // ==========================================================
        // 3. GŁÓWNA PĘTLA KODUJĄCA 
        // ==========================================================
        public List<EncodingTriple> Encode(string s, int maxP)
        {
            List<EncodingTriple> result = new List<EncodingTriple>();
            int current_index = 0;
            int n = s.Length;

            // Tworzymy tablicę P tylko jeden raz przed pętlą!
            int[] P = new int[n]; 

            while (current_index < n)
            {
                int start = Math.Max(0, current_index - maxP - 1);
                int w_len = current_index - start; 
                
                if (w_len == 0)
                {
                    result.Add(new EncodingTriple(0, 0, s[current_index]));
                    current_index++;
                    continue;
                }
                
                // Wywołujemy zoptymalizowane KMP z gotową tablicą i indeksami
                SzukajKMP(s, start, current_index, P, out int najlepszy_poczatek, out int najlepsze_c);
                
                // Zabezpieczenie przed pożarciem końcówki pliku
                if (current_index + najlepsze_c >= n)
                {
                    najlepsze_c = n - current_index - 1;
                }
                
                int p = 0;
                if (najlepsze_c > 0)
                {
                    int j = najlepszy_poczatek + 1; 
                    p = w_len - j; 
                }
                
                int c = najlepsze_c;
                char t = s[current_index + c];
                result.Add(new EncodingTriple(p, c, t));
                
                current_index += c + 1;
            }
            
            return result;   
        }
        
    }

    [Serializable]
    public struct EncodingTriple
    {
        public int p, c;
        public char s;

        public EncodingTriple(int p, int c, char s)
        {
            this.p = p;
            this.c = c;
            this.s = s;
        }
    }
}