using System;
using System.Linq;
using ASD.Graphs;

namespace ASD
{
    public class ProductionPlanner : MarshalByRefObject
    {
        /// <summary>
        /// Flaga pozwalająca na włączenie wypisywania szczegółów skonstruowanego planu na konsolę.
        /// Wartość <code>true</code> spoeoduje wypisanie planu.
        /// </summary>
        public bool ShowDebug { get; } = false;

        /// <summary>
        /// Część 1. zadania - zaplanowanie produkcji telewizorów dla pojedynczego kontrahenta.
        /// </summary>
        /// <remarks>
        /// Do przeprowadzenia testów wyznaczających maksymalną produkcję i zysk wymagane jest jedynie zwrócenie obiektu <see cref="PlanData"/>.
        /// Testy weryfikujące plan wymagają przypisania tablicy z planem do parametru wyjściowego <see cref="weeklyPlan"/>.
        /// </remarks>
        /// <param name="production">
        /// Tablica obiektów zawierających informacje o produkcji fabryki w kolejnych tygodniach.
        /// Wartości pola <see cref="PlanData.Quantity"/> oznaczają limit produkcji w danym tygodniu,
        /// a pola <see cref="PlanData.Value"/> - koszt produkcji jednej sztuki.
        /// </param>
        /// <param name="sales">
        /// Tablica obiektów zawierających informacje o sprzedaży w kolejnych tygodniach.
        /// Wartości pola <see cref="PlanData.Quantity"/> oznaczają maksymalną sprzedaż w danym tygodniu,
        /// a pola <see cref="PlanData.Value"/> - cenę sprzedaży jednej sztuki.
        /// </param>
        /// <param name="storageInfo">
        /// Obiekt zawierający informacje o magazynie.
        /// Wartość pola <see cref="PlanData.Quantity"/> oznacza pojemność magazynu,
        /// a pola <see cref="PlanData.Value"/> - koszt przechowania jednego telewizora w magazynie przez jeden tydzień.
        /// </param>
        /// <param name="weeklyPlan">
        /// Parametr wyjściowy, przez który powinien zostać zwrócony szczegółowy plan sprzedaży.
        /// </param>
        /// <returns>
        /// Obiekt <see cref="PlanData"/> opisujący wyznaczony plan.
        /// W polu <see cref="PlanData.Quantity"/> powinna znaleźć się maksymalna liczba wyprodukowanych telewizorów,
        /// a w polu <see cref="PlanData.Value"/> - wyznaczony maksymalny zysk fabryki.
        /// </returns>
        public PlanData CreateSimplePlan(PlanData[] production, PlanData[] sales, PlanData storageInfo,
            out SimpleWeeklyPlan[] weeklyPlan)
        {
            int n = production.Length; // n to liczba tygodni (fabryk i klientów jest tyle samo)
    
            // 0 - S, od 1 do n - Fabryki, od n+1 do 2n - Klienci, 2n+1 - T
            int liczbaWierzcholkow = 2 * n + 2; 
            NetworkWithCosts<int, double> graf = new NetworkWithCosts<int, double>(liczbaWierzcholkow);
    
            int S = 0;
            int T = 2 * n + 1; // Nasze ostateczne ujście

            int licz = 1;
            foreach (var prod in production)
            {
                // 1. Z S do Fabryki
                graf.AddEdge(S, licz, prod.Quantity, prod.Value);
        
                // 2. Z Fabryki do Fabryki (Magazyn) - TYLKO jeśli to nie jest ostatni tydzień!
                if (licz < n)
                {
                    graf.AddEdge(licz, licz + 1, storageInfo.Quantity, storageInfo.Value); 
                }
                licz++;
            }

            licz = 1;
            foreach (var sale in sales)
            {
                int wierzcholekKlienta = n + licz; // Przesunięcie o 'n' miejsc
        
                // 3. Z Fabryki do Klienta
                graf.AddEdge(licz, wierzcholekKlienta, sale.Quantity, -sale.Value);
        
                // 4. Z Klienta do T (Możemy śmiało użyć sale.Quantity, bo i tak więcej nie kupi)
                graf.AddEdge(wierzcholekKlienta, T, sale.Quantity, 0);
        
                licz++;
            }

            var (flowValue, flowCost, f) = Flows.MinCostMaxFlow(graf, S, T);
    
            // Inicjujemy tablicę wynikową na dokładnie 'n' tygodni
            weeklyPlan = new SimpleWeeklyPlan[n];
    
            // Mała funkcja pomocnicza, żeby kod był czystszy (czyta przepływ, a jak nie ma rury, zwraca 0)
            int GetFlow(int u, int v) => f.HasEdge(u, v) ? f.GetEdgeWeight(u, v) : 0;

            for (int i = 0; i < n; i++)
            {
                // Odtwarzamy numery wierzchołków dla danego tygodnia (i = 0 to pierwszy tydzień)
                int wierzcholekFabryki = i + 1;
                int wierzcholekKlienta = n + i + 1;

                // 1. Ile wyprodukowano? (Czytamy wodę na krawędzi S -> Fabryka)
                int wyprodukowano = GetFlow(S, wierzcholekFabryki);

                // 2. Ile sprzedano? (Czytamy wodę na krawędzi Fabryka -> Klient)
                int sprzedano = GetFlow(wierzcholekFabryki, wierzcholekKlienta);

                // 3. Ile zmagazynowano na przyszły tydzień? 
                int zmagazynowano = 0;
                if (i < n - 1) // Ostatni tydzień nie ma rury w przyszłość!
                {
                    zmagazynowano = GetFlow(wierzcholekFabryki, wierzcholekFabryki + 1);
                }

                // Zapisujemy odczyty do planu na ten tydzień
                weeklyPlan[i] = new SimpleWeeklyPlan
                {
                    UnitsProduced = wyprodukowano,
                    UnitsSold = sprzedano,
                    UnitsStored = zmagazynowano
                };
            }

            // Algorytm MinCost starał się zminimalizować koszty. 
            // Ponieważ nasza cena sprzedaży była ujemna, by wyliczyć ZYSK, 
            // musimy po prostu odwrócić znak całkowitego kosztu!
            double totalProfit = -flowCost;

            // Zwracamy podsumowanie dla szefa fabryki
            return new PlanData
            {
                Value = totalProfit,
                Quantity = flowValue // flowValue to całkowity przepływ (czyli łączna liczba wyprodukowanych i sprzedanych TV)
            };
        }

        /// <summary>
        /// Część 2. zadania - zaplanowanie produkcji telewizorów dla wielu kontrahentów.
        /// </summary>
        /// <remarks>
        /// Do przeprowadzenia testów wyznaczających produkcję dającą maksymalny zysk wymagane jest jedynie zwrócenie obiektu <see cref="PlanData"/>.
        /// Testy weryfikujące plan wymagają przypisania tablicy z planem do parametru wyjściowego <see cref="weeklyPlan"/>.
        /// </remarks>
        /// <param name="production">
        /// Tablica obiektów zawierających informacje o produkcji fabryki w kolejnych tygodniach.
        /// Wartość pola <see cref="PlanData.Quantity"/> oznacza limit produkcji w danym tygodniu,
        /// a pola <see cref="PlanData.Value"/> - koszt produkcji jednej sztuki.
        /// </param>
        /// <param name="sales">
        /// Dwuwymiarowa tablica obiektów zawierających informacje o sprzedaży w kolejnych tygodniach.
        /// Pierwszy wymiar tablicy jest równy liczbie kontrahentów, zaś drugi - liczbie tygodni w planie.
        /// Wartości pola <see cref="PlanData.Quantity"/> oznaczają maksymalną sprzedaż w danym tygodniu,
        /// a pola <see cref="PlanData.Value"/> - cenę sprzedaży jednej sztuki.
        /// Każdy wiersz tablicy odpowiada jednemu kontrachentowi.
        /// </param>
        /// <param name="storageInfo">
        /// Obiekt zawierający informacje o magazynie.
        /// Wartość pola <see cref="PlanData.Quantity"/> oznacza pojemność magazynu,
        /// a pola <see cref="PlanData.Value"/> - koszt przechowania jednego telewizora w magazynie przez jeden tydzień.
        /// </param>
        /// <param name="weeklyPlan">
        /// Parametr wyjściowy, przez który powinien zostać zwrócony szczegółowy plan sprzedaży.
        /// </param>
        /// <returns>
        /// Obiekt <see cref="PlanData"/> opisujący wyznaczony plan.
        /// W polu <see cref="PlanData.Quantity"/> powinna znaleźć się optymalna liczba wyprodukowanych telewizorów,
        /// a w polu <see cref="PlanData.Value"/> - wyznaczony maksymalny zysk fabryki.
        /// </returns>
        public PlanData CreateComplexPlan(PlanData[] production, PlanData[,] sales, PlanData storageInfo,
            out WeeklyPlan[] weeklyPlan)
        {
            weeklyPlan = null;
            return new PlanData
            {
                Value = 0,
                Quantity = 0,
            };
        }
    }
}