
using System;

namespace ASD
{

    class ChangeMaking
    {

        /// <summary>
        /// Metoda wyznacza rozwiązanie problemu wydawania reszty przy pomocy minimalnej liczby monet
        /// bez ograniczeń na liczbę monet danego rodzaju
        /// </summary>
        /// <param name="amount">Kwota reszty do wydania</param>
        /// <param name="coins">Dostępne nominały monet</param>
        /// <param name="change">Liczby monet danego nominału użytych przy wydawaniu reszty</param>
        /// <returns>Minimalna liczba monet potrzebnych do wydania reszty</returns>
        /// <remarks>
        /// coins[i]  - nominał monety i-tego rodzaju
        /// change[i] - liczba monet i-tego rodzaju (nominału) użyta w rozwiązaniu
        /// Jeśli dostepnymi monetami nie da się wydać danej kwoty to change = null,
        /// a metoda również zwraca null
        ///
        /// Wskazówka/wymaganie:
        /// Dodatkowa uzyta pamięć powinna (musi) być proporcjonalna do wartości amount ( czyli rzędu o(amount) )
        /// </remarks>

        public int? NoLimitsDynamic(int amount, int[] coins, out int[] change)
        {
            
            const int inf = int.MaxValue / 2;  // ustawiamy sobie inf w polach
            
            int[] DP = new int[amount + 1]; // zablica na pamietanie reszt
            int[] usedCoinIndex = new int[amount + 1]; // tablica do pamietania ostatniego indeksu liczby jakiej uzylismy 
    
            Array.Fill(DP, inf);
            Array.Fill(usedCoinIndex, -1);
            DP[0] = 0; // ustawiamy zero w zerze
    
            for (int i = 1; i <= amount; i++)  // idziemy po wszytskich mozliwych sumach
            {
                for (int j = 0; j < coins.Length; j++) // idziemy po wszytskich monetach
                {
                    if (coins[j] <= i && DP[i - coins[j]] != inf) // jesli nasza moneta jest mniejsza od sumy i jesli nie skoczymy na nieskonczonosc
                    {
                        int currentCoinsCount = DP[i - coins[j]] + 1; // to oznacza ze nasza nowa sume mozemy wydac za pomoca DP[i-moneta] sposobow + 1
                        if (currentCoinsCount < DP[i]) // jesli tak jest to aktualizujemy
                        {
                            DP[i] = currentCoinsCount;
                            usedCoinIndex[i] = j; // zapisujemy ze dodalismy monete j
                        }
                    }
                }
            }
    
            // Jeśli nie udało się złożyć kwoty, zwracamy nulle
            if (DP[amount] == inf)
            {
                change = null; 
                return null;
            }
            // inicjalizujemy tablice change na dlugosc 
            change = new int[coins.Length]; 
            int currentAmount = amount;
    
            // Odtwarzanie rozwiązania
            while (currentAmount > 0)
            {
                int coinIdx = usedCoinIndex[currentAmount];
                change[coinIdx]++; // Zwiększamy ilość użytej monety danego rodzaju
                currentAmount -= coins[coinIdx]; // Zmniejszamy resztę
            }
    
            return DP[amount];
        }

        /// <summary>
        /// Metoda wyznacza rozwiązanie problemu wydawania reszty przy pomocy minimalnej liczby monet
        /// z uwzględnieniem ograniczeń na liczbę monet danego rodzaju
        /// </summary>
        /// <param name="amount">Kwota reszty do wydania</param>
        /// <param name="coins">Dostępne nominały monet</param>
        /// <param name="limits">Liczba dostępnych monet danego nomimału</param>
        /// <param name="change">Liczby monet danego nominału użytych przy wydawaniu reszty</param>
        /// <returns>Minimalna liczba monet potrzebnych do wydania reszty</returns>
        /// <remarks>
        /// coins[i]  - nominał monety i-tego rodzaju
        /// limits[i] - dostepna liczba monet i-tego rodzaju (nominału)
        /// change[i] - liczba monet i-tego rodzaju (nominału) użyta w rozwiązaniu
        /// Jeśli dostepnymi monetami nie da się wydać danej kwoty to change = null,
        /// a metoda również zwraca null
        ///
        /// Wskazówka/wymaganie:
        /// Dodatkowa uzyta pamięć powinna (musi) być proporcjonalna do wartości iloczynu amount*(liczba rodzajów monet)
        /// ( czyli rzędu o(amount*(liczba rodzajów monet)) )
        /// </remarks>
        public int? Dynamic(int amount, int[] coins, int[] limits, out int[] change) 
        {
            const int inf = int.MaxValue / 2;
            int[,] DP = new int[coins.Length, amount + 1]; 
            
            // prev musi być 2D! Zapiszemy tu ile sztuk (k) monety (i) użyliśmy dla kwoty (j)
            int[,] prev = new int[coins.Length, amount + 1]; 
            
            for (int i = 0; i < coins.Length; i++) // idziemy po koleinych i monetach
            {
                for (int j = 0; j <= amount; j++)
                {
                    DP[i, j] = inf;
                }
                DP[i, 0] = 0; 
                
                for (int j = 1; j <= amount; j++) 
                {
                    
                    // ZMIANA 3: Musi być <= limits[i], bo możemy użyć dokładnie tyle monet, ile wynosi limit
                    for (int k = 0; k <= limits[i]; k++)
                    {
                        int val = k * coins[i]; // Wartość k sztuk monety
                        
                        // Zabezpieczenie przed ujemnym indeksem j
                        if (val <= j) 
                        {
                            if (i == 0) // Pierwszy wiersz nie ma i-1, musimy to obsłużyć ręcznie!
                            {
                                if (val == j) // Pierwszą monetą możemy wydać tylko jej wielokrotności
                                {
                                    DP[i, j] = k;
                                    prev[i, j] = k;
                                }
                            }
                            else // Dla pozostałych monet (i > 0)
                            {
                                // ZMIANA 4: Używamy wartości val i zmiennej k!
                                if (DP[i - 1, j - val] != inf)
                                {
                                    if (DP[i - 1, j - val] + k < DP[i, j]) 
                                    {
                                        DP[i, j] = DP[i - 1, j - val] + k; 
                                        prev[i, j] = k; // Zapisujemy ile sztuk tej monety wzięliśmy 
                                    }
                                }
                            }
                        }
                    }
                }
            }
            
            if (DP[coins.Length - 1, amount] == inf) 
            {
                change = null;
                return null;
            }
            
            // ZMIANA 5: Poprawne odtwarzanie z tablicy 2D
            change = new int[coins.Length]; 
            int currAmount = amount;
            
            for (int i = coins.Length - 1; i >= 0; i--) // Idziemy od ostatniej monety do pierwszej
            {
                int usedSztuk = prev[i, currAmount]; // Odczytujemy zapisane k
                change[i] = usedSztuk;
                currAmount -= usedSztuk * coins[i];
            }
            
            return DP[coins.Length - 1, amount];
        }

    }

}
