using System;
using System.Collections.Generic;
using System.Linq;

namespace ASD
{
    [Serializable]
    public struct Point
    {
        public double x;
        public double y;

        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Point p1, Point p2) { return p1.x == p2.x && p1.y == p2.y; }

        public static bool operator !=(Point p1, Point p2) { return !(p1 == p2); }

        public override string ToString()
        {
            return string.Format("({0},{1})", x, y);
        }
        public static double Distance(Point p1, Point p2)
        {
            double dx, dy;
            dx = p1.x - p2.x;
            dy = p1.y - p2.y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

    }

    public class Lab12 : MarshalByRefObject
    {

        private class ByY : IComparer<Point>
        {
            public int Compare(Point p1, Point p2)
            {
                int res = p1.y.CompareTo(p2.y);
                return res == 0 ? p1.x.CompareTo(p2.x) : res;
            }
        }


        /// <summary>
        /// Metoda zwraca dwa najbliższe punkty w dwuwymiarowej przestrzeni Euklidesowej
        /// </summary>
        /// <param name="points">Chmura punktów</param>
        /// <param name="minDistance">Odległość pomiędzy najbliższymi punktami</param>
        /// <returns>Para najbliższych punktów. Kolejność nie ma znaczenia</returns>
        /// <remarks>
        /// 1) Algorytm powinien mieć złożoność O(n^2), gdzie n to liczba punktów w chmurze
        /// </remarks>
        public Tuple<Point, Point> FindClosestPointsBrute(List<Point> points, out double minDistance)
        {
            minDistance = double.MaxValue;
            Tuple<Point, Point> bestPair = new Tuple<Point, Point>(points[0], points[1]);

            // Dwie pętle - klasyczne sprawdzenie wszystkich możliwych par
            for (int i = 0; i < points.Count; i++)
            {
                for (int j = i + 1; j < points.Count; j++)
                {
                    double dist = Point.Distance(points[i], points[j]);
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        bestPair = new Tuple<Point, Point>(points[i], points[j]);
                    }
                }
            }

            return bestPair;
        }

        /// <summary>
        /// Metoda zwraca dwa najbliższe punkty w dwuwymiarowej przestrzeni Euklidesowej
        /// </summary>
        /// <param name="points">Chmura punktów</param>
        /// <param name="minDistance">Odległość pomiędzy najbliższymi punktami</param>
        /// <returns>Para najbliższych punktów. Kolejność nie ma znaczenia</returns>
        /// <remarks>
        /// 1) Algorytm powinien mieć złożoność n*logn, gdzie n to liczba punktów w chmurze
        /// </remarks>
        public Tuple<Point, Point> FindClosestPoints(List<Point> points, out double minDistance)
        {
            // Zabezpieczenie na wypadek małej ilości punktów (zgodnie z PDF zawsze będą co najmniej dwa)
            if (points.Count <= 3)
            {
                return FindClosestPointsBrute(points, out minDistance);
            }

            // 1. Sortujemy całą chmurę punktów po osi X (od lewej do prawej). 
            // Dzięki temu nasza "miotła" będzie mogła iterować płynnie po punktach.
            points.Sort((p1, p2) =>
            {
                int cmp = p1.x.CompareTo(p2.x);
                return cmp == 0 ? p1.y.CompareTo(p2.y) : cmp;
            });

            // Inicjujemy dystans i najlepszą parę na podstawie dwóch pierwszych punktów
            minDistance = Point.Distance(points[0], points[1]);
            Tuple<Point, Point> bestPair = new Tuple<Point, Point>(points[0], points[1]);

            // Drzewo posortowane po Y (nasza struktura D z PDF-a)
            // Przechowuje punkty, które są na lewo od miotły w odległości nie większej niż aktualne minDistance
            SortedSet<Point> activePoints = new SortedSet<Point>(new ByY());
            
            // Wrzucamy początkowe punkty na stos miotły
            activePoints.Add(points[0]);
            activePoints.Add(points[1]);

            // Indeks lewej krawędzi naszego "okna" d. 
            // Mówi nam, które punkty są już za daleko i trzeba je usunąć z drzewa.
            int leftBoundaryIdx = 0;

            // 2. Właściwe ZAMIATANIE - startujemy od trzeciego punktu (indeks 2)
            for (int i = 2; i < points.Count; i++)
            {
                Point currentPoint = points[i];

                // Krok 1 z algorytmu: Usuwamy z drzewa punkty, które zostały za daleko w tyle.
                // Jeśli różnica X między aktualnym punktem a najstarszym w aktywnym zbiorze jest większa niż minDistance, wyrzucamy go.
                while (currentPoint.x - points[leftBoundaryIdx].x > minDistance)
                {
                    activePoints.Remove(points[leftBoundaryIdx]);
                    leftBoundaryIdx++;
                }

                // Krok 2: Szukamy kandydatów.
                // Tworzymy "sztuczne" punkty graniczne, żeby wyciągnąć z drzewa D tylko te punkty, 
                // których współrzędna Y znajduje się w przedziale [p.y - d, p.y + d]
                Point lowerBound = new Point(currentPoint.x, currentPoint.y - minDistance);
                Point upperBound = new Point(currentPoint.x, currentPoint.y + minDistance);

                // GetViewBetween wyciąga ze zbioru activePoints (w czasie O(log n)) wyłącznie te punkty, 
                // które spełniają powyższy warunek. Dzięki temu nie sprawdzamy tysięcy punktów, a jedynie kilka sztuk!
                var candidatesToCheck = activePoints.GetViewBetween(lowerBound, upperBound);

                // Krok 3: Sprawdzamy wyselekcjonowanych kandydatów i uaktualniamy d
                foreach (Point candidate in candidatesToCheck)
                {
                    double dist = Point.Distance(currentPoint, candidate);
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        bestPair = new Tuple<Point, Point>(currentPoint, candidate);
                    }
                }

                // Po sprawdzeniu dodajemy aktualny punkt do zbioru aktywnych, 
                // żeby był dostępny jako kandydat dla kolejnych wierzchołków
                activePoints.Add(currentPoint);
            }

            return bestPair;
        }
    }
    

}
