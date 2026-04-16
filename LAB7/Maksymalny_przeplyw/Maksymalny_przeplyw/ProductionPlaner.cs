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
    // Pobieramy wymiary z argumentów
    int n = production.Length;      // liczba tygodni
    int m = sales.GetLength(0);     // liczba kontrahentów (klientów)

    // Liczymy wierzchołki:
    // 1 (Źródło S) + n (Fabryki) + n * m (Wszyscy klienci we wszystkich tygodniach) + 1 (Ujście T)
    int liczbaWierzcholkow = 1 + n + (n * m) + 1;
    NetworkWithCosts<int, double> graf = new NetworkWithCosts<int, double>(liczbaWierzcholkow);

    // Definiujemy nasze główne punkty
    int S = 0;
    int T = liczbaWierzcholkow - 1;

    // BUDOWANIE GRAFU
    for (int i = 0; i < n; i++)
    {
        // Fabryki mają numery od 1 do n. Tydzień i = Fabryka i + 1.
        int wierzcholekFabryki = i + 1;

        // 1. Rura z S do Fabryki (Produkcja)
        graf.AddEdge(S, wierzcholekFabryki, production[i].Quantity, production[i].Value);

        // 2. Rura z Fabryki do Fabryki+1 (Magazyn) - pomijamy w ostatnim tygodniu
        if (i < n - 1)
        {
            graf.AddEdge(wierzcholekFabryki, wierzcholekFabryki + 1, storageInfo.Quantity, storageInfo.Value);
        }

        // 3. Rury do wszystkich klientów w TYM tygodniu
        for (int j = 0; j < m; j++)
        {
            // Unikalny numer wierzchołka dla danego klienta 'j' w tygodniu 'i'
            // Przeskakujemy S (1) oraz wszystkie fabryki (n). Następnie dodajemy i*m (klienci z poprzednich tygodni) + j
            int wierzcholekKlienta = 1 + n + (i * m) + j;

            // UWAGA NA INDEKSY TABLICY SALES! W treści zadania jest [kontrahent, tydzień], czyli sales[j, i]
            double kosztSprzedazy = -sales[j, i].Value; // Minus bo to nasz zysk!
            int pojemnoscSprzedazy = sales[j, i].Quantity;

            // Fabryka -> Klient
            graf.AddEdge(wierzcholekFabryki, wierzcholekKlienta, pojemnoscSprzedazy, kosztSprzedazy);
            
            // Klient -> Ujście T
            graf.AddEdge(wierzcholekKlienta, T, pojemnoscSprzedazy, 0);
        }
    }

    // ODPALAMY ALGORYTM
    var (flowValue, flowCost, f) = Flows.MinCostMaxFlow(graf, S, T);

    // FUNKCJA POMOCNICZA DO ODCZYTYWANIA RUR
    int GetFlow(int u, int v) => f.HasEdge(u, v) ? (int)f.GetEdgeWeight(u, v) : 0;

    // ODCZYTYWANIE PLANU
    weeklyPlan = new WeeklyPlan[n];

    for (int i = 0; i < n; i++)
    {
        int wierzcholekFabryki = i + 1;

        int wyprodukowano = GetFlow(S, wierzcholekFabryki);
        int zmagazynowano = (i < n - 1) ? GetFlow(wierzcholekFabryki, wierzcholekFabryki + 1) : 0;

        // Odczytujemy sprzedaż dla każdego z klientów z osobna
        int[] sprzedanoKlientom = new int[m];
        for (int j = 0; j < m; j++)
        {
            int wierzcholekKlienta = 1 + n + (i * m) + j;
            sprzedanoKlientom[j] = GetFlow(wierzcholekFabryki, wierzcholekKlienta);
        }

        weeklyPlan[i] = new WeeklyPlan
        {
            UnitsProduced = wyprodukowano,
            UnitsStored = zmagazynowano,
            UnitsSold = sprzedanoKlientom // Wrzucamy całą tablicę sprzedaży
        };
    }

    // ZWRACAMY WYNIK (Pamiętaj o odwróceniu znaku kosztu!)
    return new PlanData
    {
        Quantity = flowValue,   // Łącznie wyprodukowane i sprzedane
        Value = -flowCost       // Nasz ostateczny zysk (odwrócony znak)
    };
}
    }
    public PlanData CreateComplexPlan(PlanData[] production, PlanData[,] sales, PlanData storageInfo, out WeeklyPlan[] weeklyPlan)
{
    int n = production.Length;      
    int m = sales.GetLength(0);     

    // POWIĘKSZAMY GRAF O WIERZCHOŁKI 'START'
    // 1 (S) + n (Start) + n (Fabryka) + n*m (Klienci) + 1 (T)
    int liczbaWierzcholkow = 2 + 2 * n + (n * m);
    NetworkWithCosts<int, double> graf = new NetworkWithCosts<int, double>(liczbaWierzcholkow);

    int S = 0;
    int T = liczbaWierzcholkow - 1;

    for (int i = 0; i < n; i++)
    {
        // Nowa numeracja: Starty są od 1 do n. Fabryki są od n+1 do 2n.
        int wierzcholekStart = i + 1;
        int wierzcholekFabryki = n + i + 1;

        int limitProdukcji = production[i].Quantity;

        // 1. S -> Start (Wypychamy potencjał do punktu decyzyjnego, koszt 0)
        graf.AddEdge(S, wierzcholekStart, limitProdukcji, 0);

        // 2A. Start -> Fabryka (DECYZJA: PRODUKUJEMY - płacimy za produkcję)
        graf.AddEdge(wierzcholekStart, wierzcholekFabryki, limitProdukcji, production[i].Value);

        // 2B. Start -> T (DECYZJA: WENTYL BEZPIECZEŃSTWA - nie opłaca się, nic nie robimy, koszt 0)
        graf.AddEdge(wierzcholekStart, T, limitProdukcji, 0);

        // 3. Fabryka -> Magazyn (przyszły tydzień)
        if (i < n - 1)
        {
            graf.AddEdge(wierzcholekFabryki, wierzcholekFabryki + 1, storageInfo.Quantity, storageInfo.Value);
        }

        // 4. Fabryka -> Klienci
        for (int j = 0; j < m; j++)
        {
            int wierzcholekKlienta = 2 * n + 1 + (i * m) + j;
            int pojemnoscSprzedazy = sales[j, i].Quantity;
            double kosztSprzedazy = -sales[j, i].Value; // Minus to nasz zysk

            graf.AddEdge(wierzcholekFabryki, wierzcholekKlienta, pojemnoscSprzedazy, kosztSprzedazy);
            graf.AddEdge(wierzcholekKlienta, T, pojemnoscSprzedazy, 0);
        }
    }

    // ODPALAMY ALGORYTM
    var (flowValue, flowCost, f) = Flows.MinCostMaxFlow(graf, S, T);
    int GetFlow(int u, int v) => f.HasEdge(u, v) ? (int)f.GetEdgeWeight(u, v) : 0;

    weeklyPlan = new WeeklyPlan[n];
    int totalProducedQuantity = 0;

    // ODCZYTYWANIE WYNIKÓW
    for (int i = 0; i < n; i++)
    {
        int wierzcholekStart = i + 1;
        int wierzcholekFabryki = n + i + 1;

        // ILE FAKTYCZNIE WYPRODUKOWANO? (Sprawdzamy prąd, który wybrał opcję 2A zamiast ucieczki do T)
        int wyprodukowano = GetFlow(wierzcholekStart, wierzcholekFabryki);
        totalProducedQuantity += wyprodukowano;

        int zmagazynowano = (i < n - 1) ? GetFlow(wierzcholekFabryki, wierzcholekFabryki + 1) : 0;

        int[] sprzedanoKlientom = new int[m];
        for (int j = 0; j < m; j++)
        {
            int wierzcholekKlienta = 2 * n + 1 + (i * m) + j;
            sprzedanoKlientom[j] = GetFlow(wierzcholekFabryki, wierzcholekKlienta);
        }

        weeklyPlan[i] = new WeeklyPlan
        {
            UnitsProduced = wyprodukowano,
            UnitsStored = zmagazynowano,
            UnitsSold = sprzedanoKlientom
        };
    }

    return new PlanData
    {
        Quantity = totalProducedQuantity, // Zwracamy faktycznie wyprodukowane telewizory
        Value = -flowCost // Maksymalny zysk
    };
}
}