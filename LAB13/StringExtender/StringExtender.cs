using System;
using System.Text;

namespace Lab15
{
    public static class stringExtender
    {
        public static void kmp(int[] p,int lenght,string s) // mamy za zadanie wypelnic p
        {
            // s[j-1] - nasz rozpatrywany indeks slowa 
            // t - najdluzszy prefiksosufiks w poprzednim slowie 
            // p[j] - najdluzszy prefiksosufiks w slowie s na indeksie j-1 
            // s[t-1] = ostatni znak aktualnego prefiksosufiksu
            // s[t] - nowy rozpatrywany znak w prefiksie
            p[0] = 0;
            p[1] = 0;
            int t = 0;
            for (int j = 2; j <= lenght; j++) // j-1 to jest indeks slowa ktore sprawdzamy
            {
                t = p[j - 1]; // patrzymy na koniec prefiksosufiksu w poprzednim
                while (t > 0 && s[j-1] != s[t]) // dopoki nasze s[j-1] != s[t]
                {
                    t = p[t-1]; // cofamy sie na ostatni znak (t to dlugosc dopasowania czyli o jeden wiecej niz indeks)
                    // np t = 5 przy slowie |ababa|byyyyyy|ababa|x (x - rozpatrujemy) wiec musimy isc na indeks 4
                }

                if (s[t] == s[j-1]) // jesli nasze s[t] jest takie samo jak s
                {
                    t++;
                }
                p[j] = t;
            }
        }
        
        /// <summary>
        /// Metoda zwraca okres słowa s, tzn. najmniejszą dodatnią liczbę p taką, że s[i]=s[i+p] dla każdego i od 0 do |s|-p-1.
        /// 
        /// Metoda musi działać w czasie O(|s|)
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        static public int Period(this string s)
        {
            int n = s.Length;
            int[] p = new int[s.Length+1];
            kmp(p,s.Length,s);
            return n - p[s.Length];
        }

        /// <summary>
        /// Metoda wyznacza największą potęgę zawartą w słowie s.
        /// 
        /// Jeżeli x jest słowem, wówczas przez k-tą potęgę słowa x rozumiemy k-krotne powtórzenie słowa x
        /// (na przykład xyzxyzxyz to trzecia potęga słowa xyz).
        /// 
        /// Należy zwrócić największe k takie, że k-ta potęga jakiegoś słowa jest zawarta w s jako spójny podciąg.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="startIndex">Pierwszy indeks fragmentu zawierającego znalezioną potęgę</param>
        /// <param name="endIndex">Pierwszy indeks po fragmencie zawierającym znalezioną potęgę</param>
        /// <returns></returns>
        public static int MaxPower(this string s, out int startIndex, out int endIndex)
        {
            int max_power = 1;
            int best_start = 0;
            int best_end = 0;
            for (int start = 0; start < s.Length; start++)
            {
                int len = s.Length - start;
                int[] p = new int[len + 1];
                kmp(p,len, s.Substring(start));
                for (int i = 0; i <= len; i++) // iterujemy sie po P 
                {
                    int L = i; // obecnego podslowa 
                    int w = p[i];
                    int period = L - w;
                    if (period > 0 && L % period == 0)
                    {
                        int current_power = L / period;
                        if (current_power > max_power)
                        {
                            max_power = current_power;
                            best_start = start;
                            best_end = start + L;
                        }
                    }
                }
            }

            startIndex = best_start;
            endIndex = best_end;
            return max_power;
        }
    }
}