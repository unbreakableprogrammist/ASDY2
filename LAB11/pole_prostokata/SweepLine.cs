using System;
using System.Collections.Generic;
using System.Linq;

namespace ASD
{

    class SweepLine
    {

        /// <summary>
        /// Struktura pomocnicza opisująca zdarzenie
        /// </summary>
        /// <remarks>
        /// Można jej użyć, przerobić, albo w ogóle nie używać i zrobić po swojemu
        /// </remarks>
        struct SweepEvent
        {
            /// <summary>
            /// Współrzędna zdarzenia
            /// </summary>
            public double Coord;

            /// <summary>
            /// Czy zdarzenie oznacza początek odcinka/prostokąta
            /// </summary>
            public bool IsStartingPoint;

            /// <summary>
            /// Indeks odcinka/prodtokąta w odpowiedniej tablicy
            /// </summary>
            public int Idx;

            public SweepEvent(double c, bool sp, int i=-1 ) { Coord=c; IsStartingPoint=sp; Idx=i; }
        }

        /// <summary>
        /// Funkcja obliczająca długość teoriomnogościowej sumy pionowych odcinków
        /// </summary>
        /// <returns>Długość teoriomnogościowej sumy pionowych odcinków</returns>
        /// <param name="segments">Tablica z odcinkami, których teoriomnogościowej sumy długość należy policzyć</param>
        /// Każdy odcinek opisany jest przez dwa punkty: początkowy i końcowy
        /// </param>
        public double VerticalSegmentsUnionLength(Geometry.Segment[] segments)
        {
            List<SweepEvent> events = new List<SweepEvent>();
            for (int i = 0; i < segments.Length; i++)
            {
                // Zabezpieczamy się, upewniając się co jest dołem, a co górą
                double minY = Math.Min(segments[i].ps.y, segments[i].pe.y);
                double maxY = Math.Max(segments[i].ps.y, segments[i].pe.y);

                events.Add(new SweepEvent(minY, true, i));  // Zawsze otwieramy na dole
                events.Add(new SweepEvent(maxY, false, i)); // Zawsze zamykamy na górze
            }

            events.Sort((a, b) =>
            {
                int cmp = a.Coord
                    .CompareTo(b.Coord); // zwraca 1 w przypadku a > b, -1 w przypadku a < b, 0 w przypadku a == b
                if (cmp == 0)
                {
                    // Jeśli zdarzenia są na tej samej wysokości, najpierw dajemy "otwarcia" (true).
                    // Dzięki temu jeśli odcinek B zaczyna się idealnie tam, gdzie kończy się A, 
                    // nie przerwiemy na ułamek sekundy naszej "serii" (nie zrobimy dziury w polu).
                    return b.IsStartingPoint.CompareTo(a.IsStartingPoint);//
                }
                // zwracamy cmp, sort zostawia dane jesli zwrocimy -1, zamienia gdy zwrocimy 1
                return cmp;
            });
            
            double totalLength = 0;
            int activeEvents = 0;
            double currentSeriesStart = 0;
            foreach (var ev in events)
            {
                // Jeśli licznik to 0, znaczy to, że właśnie wchodzimy w nową serię odcinków
                if (activeEvents == 0)
                {
                    currentSeriesStart = ev.Coord;
                }

                // Zwiększamy licznik jeśli odcinek się zaczyna, zmniejszamy jeśli się kończy
                if (ev.IsStartingPoint) activeEvents++;
                else activeEvents--;

                // Jeśli licznik spadł do 0, seria nakładających się odcinków właśnie się zakończyła
                if (activeEvents == 0)
                {
                    totalLength += (ev.Coord - currentSeriesStart);
                }
            }

            return totalLength;
            return -1;
        }

        /// <summary>
        /// Funkcja obliczająca pole teoriomnogościowej sumy prostokątów
        /// </summary>
        /// <returns>Pole teoriomnogościowej sumy prostokątów</returns>
        /// <param name="rectangles">Tablica z prostokątami, których teoriomnogościowej sumy pole należy policzyć</param>
        /// Każdy prostokąt opisany jest przez cztery wartości: minimalna współrzędna X, minimalna współrzędna Y, 
        /// maksymalna współrzędna X, maksymalna współrzędna Y.
        /// </param>
        public double RectanglesUnionArea(Geometry.Rectangle[] rectangles)
        {
            return -1;
        }

    }

}