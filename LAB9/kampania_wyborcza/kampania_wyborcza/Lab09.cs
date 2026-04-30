using System;
using System.Collections.Generic;
using System.Linq;
using ASD.Graphs;

namespace ASD
{
    public class Lab08 : MarshalByRefObject
    {
        /// <summary>
        /// Znajduje cykl rozpoczynający się w stolicy, który dla wybranych miast,
        /// przez które przechodzi ma największą sumę liczby ludności w tych wybranych
        /// miastach oraz minimalny koszt.
        /// </summary>
        /// <param name="cities">
        /// Graf miast i połączeń między nimi.
        /// Waga krawędzi jest kosztem przejechania między dwoma miastami.
        /// Koszty transportu między miastami są nieujemne.
        /// </param>
        /// <param name="citiesPopulation">Liczba ludności miast</param>
        /// <param name="meetingCosts">
        /// Koszt spotkania w każdym z miast.
        /// Dla części pierwszej koszt spotkania dla każdego miasta wynosi 0.
        /// Dla części drugiej koszty są nieujemne.
        /// </param>
        /// <param name="budget">Budżet do wykorzystania przez kandydata.</param>
        /// <param name="capitalCity">Numer miasta będącego stolicą, z której startuje kandydat.</param>
        /// <param name="path">
        /// Tablica dwuelementowych krotek opisująca ciąg miast, które powinen odwiedzić kandydat.
        /// Pierwszy element krotki to numer miasta do odwiedzenia, a drugi element decyduje czy
        /// w danym mieście będzie organizowane spotkanie wyborcze.
        /// 
        /// Pierwszym miastem na tej liście zawsze będzie stolica (w której można, ale nie trzeba
        /// organizować spotkania).
        /// 
        /// Zakładamy, że po odwiedzeniu ostatniego miasta na liście kandydat wraca do stolicy
        /// (na co musi mu starczyć budżetu i połączenie między tymi miastami musi istnieć).
        /// 
        /// Jeżeli kandydat nie wyjeżdża ze stolicy (stolica jest jedynym miastem, które odwiedzi),
        /// to lista `path` powinna zawierać jedynie jeden element: stolicę (wraz z informacją
        /// czy będzie tam spotkanie czy nie). Nie są wtedy ponoszone żadne koszty podróży.
        /// 
        /// W pierwszym etapie drugi element krotki powinien być zawsze równy `true`.
        /// </param>
        /// <returns>
        /// Liczba mieszkańców, z którymi spotka się kandydat.
        /// </returns>

        static int[] _population;          // populacja w każdym mieście, _population[i] = liczba mieszkańców miasta i
        static double[] _meetingCosts;     // koszt organizacji spotkania, _meetingCosts[i] = koszt spotkania w mieście i
        static Graph<int> _cities;         // graf miast, wagi krawędzi = koszt podróży między miastami
        static int _capital;               // numer wierzchołka stolicy (miasto startowe)
        static bool[] _visited;            // _visited[i] = true jeśli miasto i już odwiedzone w aktualnej ścieżce

        static int _bestPopulation;        // najlepsza (największa) liczba mieszkańców znaleziona do tej pory
        static double _bestCost;           // koszt ścieżki dającej _bestPopulation (przy remisie wybieramy tańszą)
        static List<(int, bool)> _bestPath; // najlepsza ścieżka: (numer miasta, czy organizujemy spotkanie)

        static void Backtrack(int current, double budgetLeft, List<(int, bool)> currentPath,
            int currentPopulation, double currentCost, bool withMeetingCosts)
        {
            // aktualna ścieżka jest zawsze potencjalnym wynikiem
            // sprawdzamy czy po porstu z naszego miasta mozemy wrocic 
            bool moznaWrocic = (current == _capital) ||
                               (_cities.HasEdge(current, _capital) &&
                                _cities.GetEdgeWeight(current, _capital) <= budgetLeft);

            if (moznaWrocic) // jesli z naszego miasta mozemy wrocic to 
            {
                if (currentPopulation > _bestPopulation ||
                    (currentPopulation == _bestPopulation && currentCost < _bestCost))
                {
                    _bestPopulation = currentPopulation;
                    _bestCost = currentCost;
                    _bestPath = new List<(int, bool)>(currentPath);
                }
            }

            // próbuj każde sąsiednie miasto
            foreach (var neighbor in _cities.OutNeighbors(current))
            {
                // jesli jakis sasiad juz byl odwiedzony to pass 
                if (_visited[neighbor]) continue;

                double kosztPrzejazdu = _cities.GetEdgeWeight(current, neighbor);
                if (kosztPrzejazdu > budgetLeft) continue;

                _visited[neighbor] = true;

                if (!withMeetingCosts)
                {
                    // Część 1: zawsze organizujemy spotkanie, brak kosztów spotkań
                    currentPath.Add((neighbor, true));
                    Backtrack(neighbor, budgetLeft - kosztPrzejazdu, currentPath,
                        currentPopulation + _population[neighbor],
                        currentCost + kosztPrzejazdu, false);
                    currentPath.RemoveAt(currentPath.Count - 1);
                }
                else
                {
                    // Część 2: dla każdego miasta próbuj ze spotkaniem i bez

                    // opcja 1: jedź ale NIE organizuj spotkania
                    currentPath.Add((neighbor, false));
                    Backtrack(neighbor, budgetLeft - kosztPrzejazdu, currentPath,
                        currentPopulation, // nie dodajemy mieszkańców
                        currentCost + kosztPrzejazdu, true);
                    currentPath.RemoveAt(currentPath.Count - 1);

                    // opcja 2: jedź I organizuj spotkanie (jeśli stać)
                    double kosztSpotkania = _meetingCosts[neighbor];
                    if (kosztPrzejazdu + kosztSpotkania <= budgetLeft)
                    {
                        currentPath.Add((neighbor, true));
                        Backtrack(neighbor, budgetLeft - kosztPrzejazdu - kosztSpotkania, currentPath,
                            currentPopulation + _population[neighbor],
                            currentCost + kosztPrzejazdu + kosztSpotkania, true);
                        currentPath.RemoveAt(currentPath.Count - 1);
                    }
                }
                _visited[neighbor] = false;
            }
        }

        public int ComputeElectionCampaignPath(Graph<int> cities, int[] citiesPopulation, double[] meetingCosts,
            double budget, int capitalCity, out (int, bool)[] path)
        {
            _population = citiesPopulation;
            _meetingCosts = meetingCosts;
            _cities = cities;
            _capital = capitalCity;
            _visited = new bool[cities.VertexCount];

            _bestPopulation = 0;
            _bestCost = 0;
            _bestPath = new List<(int, bool)>();

            _visited[capitalCity] = true;

            bool withMeetingCosts = meetingCosts.Any(c => c > 0);

            if (!withMeetingCosts)
            {
                // Część 1: stolica zawsze ma spotkanie, koszt=0
                _bestPopulation = citiesPopulation[capitalCity];
                _bestPath = new List<(int, bool)> { (capitalCity, true) };

                var startPath = new List<(int, bool)> { (capitalCity, true) };
                Backtrack(capitalCity, budget, startPath, citiesPopulation[capitalCity], 0, false);
            }
            else
            {
                // Część 2: stolica - próbuj ze spotkaniem i bez
                double kosztSpotkaniaStoilcy = meetingCosts[capitalCity];

                // bez spotkania w stolicy
                _bestPopulation = 0;
                _bestPath = new List<(int, bool)> { (capitalCity, false) };

                var startPath = new List<(int, bool)> { (capitalCity, false) };
                Backtrack(capitalCity, budget, startPath, 0, 0, true);

                // ze spotkaniem w stolicy (jeśli stać)
                if (kosztSpotkaniaStoilcy <= budget)
                {
                    startPath = new List<(int, bool)> { (capitalCity, true) };
                    Backtrack(capitalCity, budget - kosztSpotkaniaStoilcy, startPath,
                        citiesPopulation[capitalCity], kosztSpotkaniaStoilcy, true);
                }
            }

            path = _bestPath.ToArray();
            return _bestPopulation;
        }
    }
}
