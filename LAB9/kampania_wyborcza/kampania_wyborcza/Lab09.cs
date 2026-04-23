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
        public int ComputeElectionCampaignPath(Graph<int> cities, int[] citiesPopulation,
            double[] meetingCosts, double budget, int capitalCity, out (int, bool)[] path)
        {
            path = null;
            return 0;
        }
    }
}
