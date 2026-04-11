using ASD.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

namespace ASD2
{
    public class CoinFlow : MarshalByRefObject
    {
        /// <summary>
        /// Etap I: Arbitraż.
        /// Celem jest wykrycie, czy na rynku istnieje możliwość darmowego wzbogacenia się, 
        /// czyli znalezienie cyklu, dla którego iloczyn kursów wymiany jest ściśle większy niż 1.
        /// </summary>
        /// <param name="G">Ważony graf skierowany, gdzie waga krawędzi w(u,v) to mnożnik kapitału.</param>
        /// <returns>Wartość true, jeśli istnieje cykl o iloczynie wag > 1, w przeciwnym razie false.</returns>
        public bool Stage1(DiGraph<double> G)
        {
            return false;
        }

        /// <summary>
        /// Etap II: Ograniczone ryzyko.
        /// Wyznaczenie najlepszej możliwej ścieżki wymiany między walutą startNode i endNode,
        /// przy zachowaniu limitu k "słabych" walut na ścieżce (waluty startowa i końcowa również się wliczają).
        /// Zakładamy, że rynek jest stabilny - brak cykli o iloczynie > 1.
        /// </summary>
        /// <param name="G">Graf dostępnych kursów wymiany.</param>
        /// <param name="isStrong">Tablica informująca, czy dana waluta jest "mocna" (false oznacza walutę "słabą").</param>
        /// <param name="k">Maksymalna dopuszczalna liczba słabych walut na ścieżce.</param>
        /// <param name="startNode">Indeks waluty początkowej.</param>
        /// <param name="endNode">Indeks waluty docelowej.</param>
        /// <returns>Najlepsza ścieżka wymiany (ciąg indeksów) lub null, jeśli ścieżka spełniająca warunki nie istnieje.</returns>
        public int[] Stage2(DiGraph<double> G, bool[] isStrong, int k, int startNode, int endNode)
        {
            return null;
        }

        /// <summary>
        /// Etap III: Diamentowa Droga.
        /// Znalezienie ścieżki maksymalizującej końcową ilość złota uzyskaną z jednego diamentu.
        /// Proces obejmuje: wymianę diamentu na walutę początkową, dowolną liczbę wymian rynkowych 
        /// oraz końcową wymianę waluty na złoto.
        /// </summary>
        /// <param name="G">Graf dostępnych kursów wymiany walut.</param>
        /// <param name="diamondToCurrency">Lista ofert wymiany diamentu na konkretną walutę (indeks waluty, kurs).</param>
        /// <param name="currencyToGold">Lista ofert wymiany waluty na złoto (indeks waluty, kurs).</param>
        /// <returns>Ciąg indeksów walut (od pierwszej zakupionej za diament do ostatniej sprzedanej za złoto) prowadzący do najlepszego wyniku.</returns>
        public int[] Stage3(DiGraph<double> G, (int currencyIdx, double rate)[] diamondToCurrency, (int currencyIdx, double goldRate)[] currencyToGold)
        {
            return null;
        }
    }
}