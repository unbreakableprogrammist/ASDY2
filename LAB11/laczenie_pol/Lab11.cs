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
                    (point.Item2 == lowest_point.Item2 && point.Item1 < point.Item1))
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
                else return 0;
            }));
            Stack<(double, double)> stack = new Stack<(double, double)>();
            stack.Push(lowest_point);
            indx = 1;
            List<(double, double)> otoczka = new List<(double, double)>();
            otoczka.Add(lowest_point);
            for (int i = 1; i < points.Length; i++)
            {
                var last = stack.Pop();
                var nowy = points[i];
                int iloczyn = Cross(points[0], last, nowy);
                if (iloczyn > 0)
                {
                    otoczka.Add(nowy);
                    stack.Push(nowy);
                }
                else
                {
                    stack.Push(last);
                }
            }

            return otoczka.ToArray();
        }

        // Etap 2
        // oblicza otoczkę dwóch wielokątów wypukłych
        public (double, double)[] ConvexHullOfTwo((double, double)[] poly1, (double, double)[] poly2)
        {
            return null;
        }

    }
}