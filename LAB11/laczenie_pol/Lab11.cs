using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;

namespace ASD
{
    public class Lab11 : System.MarshalByRefObject
    {

        // iloczyn wektorowy
        private int Cross((double, double) o, (double, double) a, (double, double) b)
        {
            double value = (a.Item1 - o.Item1) * (b.Item2 - o.Item2) - (a.Item2 - o.Item2) * (b.Item1 - o.Item1);
            return Math.Abs(value) < 1e-10 ? 0 : value < 0 ? -1 : 1;
        }

        private double DistanceSquared((double, double) p1, (double, double) p2)
        {
            double dx = p1.Item1 - p2.Item1;
            double dy = p1.Item2 - p2.Item2;
            return dx * dx + dy * dy; 
        }
        
        // Etap 1
        // po prostu otoczka wypukła
        
        public (double, double)[] ConvexHull((double, double)[] points)
        {
            int indx = 0; 
            (double,double) lowest_point = points[0];
            for (int i = 0; i < points.Length; i++)
            {
                var point = points[i];
                if (point.Item2 < lowest_point.Item2 ||
                    (point.Item2 == lowest_point.Item2 && point.Item1 < lowest_point.Item1))
                {
                    lowest_point = point;
                    indx = i;
                }

            }

            var temp = points[0];
            points[0] = points[indx];
            points[indx] = temp;
            Array.Sort(points,1,points.Length-1,Comparer<(double,double)>.Create((p1, p2) =>
            {
                int wynik = Cross(points[0], p1, p2);
                if (wynik > 0) return -1; // p1 < p2 
                else if(wynik < 0) return 1; // p1 > p2 ( czyli trzeba zamienic) 
                else
                {
                    double dist1 = DistanceSquared(p1, points[0]);
                    double dist2 = DistanceSquared(p2, points[0]);
                    if (dist1 < dist2) return -1; // p1 jest blizej 
                    else return 1;
                }
            }));
            Stack<(double, double)> stack = new Stack<(double, double)>();
            stack.Push(lowest_point);
            indx = 1;
            for (int i = 1; i < points.Length; i++)
            {
                var nowy = points[i];
                while (stack.Count() >= 2) // dopoki sa tam jakies 2 punkty  
                {
                    var ostatni = stack.Pop();
                    var przedostatni = stack.Pop();
                    int iloczyn = Cross(przedostatni, ostatni, nowy);
                    if (iloczyn <= 0) // tutaj skrecamy w prawo ( lub jest na tej samej lini ) a to oznacza ze mamy taka sytuacje przedostani / ostatni \ nowy, czyli ostatni musimy zdjac 
                    {
                        stack.Push(przedostatni);
                        // i idziemy dalej sprawdzajac czy wczesniej gdzies nie skrecilismy w prawo
                    }
                    else // czyli tutaj normalnie skrecamy w lewo
                    {
                        stack.Push(przedostatni);
                        stack.Push(ostatni);
                        break;
                    }
                }
                stack.Push(nowy);
            }

            return stack.Reverse().ToArray();
        }

        // Część 2 zadania.
        //
        // Wejście:
        //  - dwie tablice punktów tworzące otoczki wypukłe,
        //  - punkty w każdej z tablic są uporządkowane w kolejności przeciwnej do ruchu wskazówek zegara,
        //  - wielokąty reprezentowane przez tablice nie przecinają się.
        //
        // Wyjście:
        //  - tablica punktów tworzących otoczkę wypukłą dla wszystkich punktów z obu tablic,
        //  - punkty powinny być zwrócone w kolejności przeciwnej do ruchu wskazówek zegara.
        //
        // Idea:
        //  - należy połączyć dwie otoczki wypukłe w jedną,
        //  - można użyć np. algorytmu Quickhull albo Chan's algorithm,
        //  - można też wykorzystać fakt, że wielokąty są rozłączne i poszukać stycznych,
        //  - połączenie otoczek polega na znalezieniu mostu, czyli dwóch punktów,
        //    po jednym z każdej otoczki, które tworzą odcinek, wokół którego można "owinąć"
        //    obie otoczki, aby utworzyć nową otoczkę wypukłą.
        public (double, double)[] ConvexHullOfTwo((double, double)[] poly1, (double, double)[] poly2)
        {
            // szukamy najbardziej po lewej i po prawej z otoczki 1 i 2
            int indx_min1 = 0;
            int indx_max1 = 0;
            for(int i=0 ; i < poly1.Length ; i++)
            {
                if(poly1[i].Item1 < poly1[indx_min1].Item1 || (poly1[i].Item1 == poly1[indx_min1].Item1 && poly1[i].Item2 < poly1[indx_min1].Item2))
                {
                    indx_min1 = i;
                }
                if(poly1[i].Item1 > poly1[indx_max1].Item1 || (poly1[i].Item1 == poly1[indx_max1].Item1 && poly1[i].Item2 > poly1[indx_max1].Item2))
                {
                    indx_max1 = i;
                }
            }
            
            int indx_min2 = 0;
            int indx_max2 = 0;
            for (int i = 0; i < poly2.Length; i++)
            {
                if (poly2[i].Item1 < poly2[indx_min2].Item1 || (poly2[i].Item1 == poly2[indx_min2].Item1 && poly2[i].Item2 < poly2[indx_min2].Item2))
                {
                    indx_min2 = i;
                }

                if (poly2[i].Item1 > poly2[indx_max2].Item1 || (poly2[i].Item1 == poly2[indx_max2].Item1 &&
                                                                poly2[i].Item2 > poly2[indx_max2].Item2))
                {
                    indx_max2 = i;
                }
            }
            // po co nam to bylo ? 
            // w dolnej otoczce x rosna ( w obu otoczkach 
            // w gornej otoczce x na pewno maleja, wiec teraz posortowanie tych dowch otoczek sprowadza sie jedynie do scaleniu dwoch posortowanych list 
            List<(double, double)> lower_sorted = new List<(double, double)>();

            int curr1 = indx_min1;
            int curr2 = indx_min2;

            // Te zmienne powiedzą nam, czy dany "suwak" dojechał już do prawego skraju
            bool done1 = false;
            bool done2 = false;

            while (!done1 || !done2)
            {
                // Jeśli z pierwszej otoczki już zebraliśmy cały dół, bierzemy resztę z drugiej
                if (done1)
                {
                    lower_sorted.Add(poly2[curr2]);
                    if (curr2 == indx_max2) done2 = true; 
                    else curr2 = (curr2 + 1) % poly2.Length; 
                }
                // Jeśli z drugiej już zebraliśmy, bierzemy z pierwszej
                else if (done2)
                {
                    lower_sorted.Add(poly1[curr1]);
                    if (curr1 == indx_max1) done1 = true;
                    else curr1 = (curr1 + 1) % poly1.Length;
                }
                // W obu wciąż są punkty do sprawdzenia, więc robimy starcie: kto ma mniejszy X?
                else
                {
                    if (poly1[curr1].Item1 < poly2[curr2].Item1 || 
                        (poly1[curr1].Item1 == poly2[curr2].Item1 && poly1[curr1].Item2 < poly2[curr2].Item2))
                    {
                        lower_sorted.Add(poly1[curr1]);
                        if (curr1 == indx_max1) done1 = true;
                        else curr1 = (curr1 + 1) % poly1.Length; // +1 % Length przesuwa nas bezpiecznie po obwodzie, bo czasem moze byc przypadek ze 
                    }
                    else
                    {
                        lower_sorted.Add(poly2[curr2]);
                        if (curr2 == indx_max2) done2 = true;
                        else curr2 = (curr2 + 1) % poly2.Length;
                    }
                }
            }


            List<(double, double)> higher_sorted = new List<(double, double)>();

            // ZACZYNAMY OD MINIMUM (z lewej), żeby X rosło!
            curr1 = indx_min1;
            curr2 = indx_min2;

            done1 = false;
            done2 = false;

            while (!done1 || !done2)
            {
                if (done1)
                {
                    higher_sorted.Add(poly2[curr2]);
                    if (curr2 == indx_max2) done2 = true; 
                    else curr2 = (curr2 - 1 + poly2.Length) % poly2.Length; // IDZIEMY DO TYŁU!
                }
                else if (done2)
                {
                    higher_sorted.Add(poly1[curr1]);
                    if (curr1 == indx_max1) done1 = true;
                    else curr1 = (curr1 - 1 + poly1.Length) % poly1.Length; // IDZIEMY DO TYŁU!
                }
                else
                {
                    // Sprawdzamy, kto ma mniejszy X (lub kto jest WYŻEJ w przypadku remisu)
                    if (poly1[curr1].Item1 < poly2[curr2].Item1 || 
                        (poly1[curr1].Item1 == poly2[curr2].Item1 && poly1[curr1].Item2 < poly2[curr2].Item2))
                    {
                        higher_sorted.Add(poly1[curr1]);
                        if (curr1 == indx_max1) done1 = true;
                        else curr1 = (curr1 - 1 + poly1.Length) % poly1.Length;
                    }
                    else
                    {
                        higher_sorted.Add(poly2[curr2]);
                        if (curr2 == indx_max2) done2 = true;
                        else curr2 = (curr2 - 1 + poly2.Length) % poly2.Length;
                    }
                }
            }

            // Zabezpieczenie na koniec
            if (!higher_sorted.Contains(poly1[indx_max1])) higher_sorted.Add(poly1[indx_max1]);
            if (!higher_sorted.Contains(poly2[indx_max2])) higher_sorted.Add(poly2[indx_max2]);

            // wyznaczamy otoczke dla dolu
            Stack<(double, double)> lowerStock = new Stack<(double, double)>();
            foreach (var p in lower_sorted)
            {
                while (lowerStock.Count() >= 2)
                {
                    var ostatni = lowerStock.Pop();
                    var przedostatni = lowerStock.Peek();
                    if(Cross(przedostatni, ostatni, p) <= 0) continue; // jesli idziemy w prawo lub prosto to continue i nie dodajemy tego ostaniego wierzcholka i sprawdzamy jeszcze poprzednie
                    lowerStock.Push(ostatni); // tu oznacza ze wszystko jest ok (skrecamy w lewo) 
                    break;
                }
                lowerStock.Push(p);
            }
            Stack<(double, double)> higherStock = new Stack<(double, double)>();
            foreach (var p in higher_sorted)
            { 
                while (higherStock.Count >= 2)
                {
                    var ostatni = higherStock.Pop();
                    var przedostatni = higherStock.Peek();
                    if(Cross(przedostatni,ostatni,p) >= 0) continue; // jesli wygina sie w prawo to continue
                    higherStock.Push(ostatni);
                    break;
                }
                higherStock.Push(p);
            }
            // stosy sa odwrotne -> na dole stosu maksymalny w lewo 
            var finalLower = lowerStock.Reverse().ToList(); // odwracamy bo stos jest od gory najbardziej na prawo 
            var finalHigher = higherStock.ToList();
            List<(double, double)> result = new List<(double, double)>();
            // Dodajemy całą dolną otoczkę
            result.AddRange(finalLower);

            for (int i = 0; i < finalHigher.Count; i++)
            {
                var pt = finalHigher[i];
    
                // Nie dodajemy punktu, jeśli jest taki sam jak ten na samym końcu (zapobiega duplikatom z prawej strony)
                if (result.Count > 0 && result[result.Count - 1] == pt) continue;
    
                // Nie dodajemy punktu, jeśli jest taki sam jak punkt startowy (zapobiega duplikatom z lewej strony)
                if (result.Count > 0 && result[0] == pt) continue;

                result.Add(pt);
            }

            // GOTOWE! Mamy jedną, wypukłą otoczkę w czasie liniowym O(n)
            return result.ToArray();
        }
    }
}