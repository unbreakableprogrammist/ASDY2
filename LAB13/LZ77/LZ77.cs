using System;
using System.Collections.Generic;

namespace ASD
{
    
    //   aababab
    //  00101010
    public class LZ77 : MarshalByRefObject
    {
        
        /// <summary>
        /// Odkodowywanie napisu zakodowanego algorytmem LZ77. Dane kodowanie jest poprawne (nie trzeba tego sprawdzać).
        /// </summary>
        public string Decode(List<EncodingTriple> encoding)
        {
            string res = "";
            foreach (var triple in encoding)
            {
                int p = triple.p;
                int c = triple.c;
                char s = triple.s;
                int start = res.Length - p - 1;
                for (int i = 0; i < c; i++)
                {
                    res += res[start + i];
                }
                res += s;
            }
            return res;
        }

        void kmp(string wzorzec, string tekst, int[] P,out int poczatek,out int dlugosc)
        {
            poczatek = 0;
            dlugosc = 0;
            P[0] = 0;
            P[1] = 0;
            int t = 0;
            for (int i = 2; i < wzorzec.Length; i++)
            {
                
            }
        }
        /// <summary>
        /// Kodowanie napisu s algorytmem LZ77
        /// </summary>
        /// <returns></returns>
        public List<EncodingTriple> Encode(string s, int maxP)
        {
            string w = "";
            string r = "";
            int n = s.Length;
            if (w.Length == 0)
            {
                
            }
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