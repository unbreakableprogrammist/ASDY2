using System;
using System.Collections.Generic;
using System.Linq;

namespace ASD
{
    class Program
    {
        /// <summary>
        /// Ustawienie wartości na true spowoduje pominięcie generowania i wykonywania dużych testów
        /// (istotnie przyspieszy to wykonanie małych testów)
        /// </summary>
        static bool skiBigTests = false;

        class ClosestPointsTestCase : TestCase
        {
            private double Eps = 0.00000001;

            private Tuple<Point, Point> resultP;
            private double resultD;
            private Tuple<Point, Point> properResultP;
            private double properResultD;
            private bool brute;

            private List<Point> points;

            public ClosestPointsTestCase(double timeLimit, List<Point> points, Tuple<Point, Point> properResultP, double properResultD, bool brute = false) : base(timeLimit, null, "Brak opisu")
            {
                this.properResultP = properResultP;
                this.properResultD = properResultD;
                this.points = points;
                this.brute = brute;
            }

            protected override void PerformTestCase(object prototypeObject)
            {
                resultP = brute ? ((Lab12)prototypeObject).FindClosestPointsBrute(points, out resultD) : ((Lab12)prototypeObject).FindClosestPoints(points, out resultD);
            }

            protected override (Result resultCode, string message) VerifyTestCase(object settings)
            {
                string message;
                Result resultCode;
                if (Math.Abs(resultD - properResultD) > Eps)
                {
                    resultCode = Result.WrongResult;
                    message = string.Format("Incorrect distance: {0} (expected: {1})", resultD, properResultD);
                    return (resultCode, message);
                }

                if ((properResultP.Item1 == resultP.Item1 && properResultP.Item2 == resultP.Item2)
                    || (properResultP.Item1 == resultP.Item2 && properResultP.Item2 == resultP.Item1))
                {
                    if (base.PerformanceTime <= base.TimeLimit)
                    {
                        resultCode = Result.Success;
                        message = "OK";
                    }
                    else
                    {
                        resultCode = Result.LowEfficiency;
                        message = $"Wynik ok, ale zbyt wolno: {base.TimeLimit} vs {base.PerformanceTime}";
                    }
                }
                else
                {
                    resultCode = Result.WrongResult;
                    message = string.Format("Incorrect points: {0} and {1} (expected: {2} and {3})", resultP.Item1, resultP.Item2, properResultP.Item1, properResultP.Item2);
                }
                return (resultCode, message);
            }
        }

        private class PointsWithResult : MarshalByRefObject
        {
            public List<Point> points;
            public double minDistance;
            public Tuple<Point, Point> pair;

            public PointsWithResult(List<Point> points, Tuple<Point, Point> pair, double minDistance)
            {
                this.points = points;
                this.minDistance = minDistance;
                this.pair = pair;
            }
        }

        private static PointsWithResult CreateBasicTest1()
        {
            var points = new[] { new Point(-4, 1), new Point(-3, 5), new Point(-2, 4), new Point(-1, 0) };
            return new PointsWithResult(points.ToList(), new Tuple<Point, Point>(new Point(-3, 5), new Point(-2, 4)), Point.Distance(new Point(-3, 5), new Point(-2, 4)));
        }

        private static PointsWithResult CreateBasicTest2()
        {
            // ta sama x-owa wspolrzedna
            var points = new[] { new Point(10, 10), new Point(10, 11), new Point(10, 12.5f), new Point(10, 13), new Point(10, 14), new Point(10, 15) };
            return new PointsWithResult(points.ToList(), new Tuple<Point, Point>(new Point(10, 12.5f), new Point(10, 13)), Point.Distance(new Point(10, 12.5f), new Point(10, 13)));
        }

        private static PointsWithResult CreateBasicTest3()
        {
            //ta sama y-owa wspolrzedna
            var points = new[] { new Point(1, 10), new Point(2, 10), new Point(3.5f, 10), new Point(4, 10), new Point(5, 10), new Point(6, 10) };
            return new PointsWithResult(points.ToList(), new Tuple<Point, Point>(new Point(3.5f, 10), new Point(4, 10)), Point.Distance(new Point(3.5f, 10), new Point(4, 10)));
        }

        private static PointsWithResult CreateBasicTest4()
        {
            // 3 punkty
            var points = new[] { new Point(10, 10), new Point(11, 11), new Point(15, -17) };
            return new PointsWithResult(points.ToList(), new Tuple<Point, Point>(new Point(10, 10), new Point(11, 11)), Point.Distance(new Point(10, 10), new Point(11, 11)));
        }

        private static PointsWithResult CreateBasicTest5()
        {
            // 2 punkty
            var points = new[] { new Point(10, 10), new Point(100, 100) };
            return new PointsWithResult(points.ToList(), new Tuple<Point, Point>(new Point(10, 10), new Point(100, 100)), Point.Distance(new Point(10, 10), new Point(100, 100)));
        }

        private static PointsWithResult CreateBasicTest6()
        {
            List<Point> points = new List<Point>();
            for (int i = 10; i >= 2; i--)
            {
                points.Add(new Point(-i, i));
                points.Add(new Point(-i, -i));
                points.Add(new Point(-i, i - 1));
                points.Add(new Point(-i, -i + 1));
            }
            points.Add(new Point(-2, 1.1));
            return new PointsWithResult(points, new Tuple<Point, Point>(new Point(-2, 1), new Point(-2, 1.1)), Point.Distance(new Point(-2, 1), new Point(-2, 1.1)));
        }

        private static PointsWithResult CreatePerformanceTest1()
        {
            int chmuraCount = 10000;
            float eps = 0.01f;
            Random random = new Random(1024);
            var points = Enumerable.Repeat(0, chmuraCount).Select(i => new Point(random.Next(-300, 300), random.Next(-300, 300))).Distinct().ToList();
            var p1 = points[random.Next(0, points.Count)];
            var p2 = new Point(p1.x - 7 * eps, p1.y + 2 * eps);
            points.Add(p2);
            return new PointsWithResult(points, new Tuple<Point, Point>(p1, p2), Point.Distance(p1, p2));
        }

        private static PointsWithResult CreatePerformanceTest2()
        {
            int chmuraCount = 100000;
            float eps = 0.01f;
            Random random = new Random(1024);
            var points = Enumerable.Repeat(0, chmuraCount).Select(i => new Point(random.Next(-40, 40), random.Next(-40, 40))).Distinct().ToList();
            var p1 = points[random.Next(0, points.Count)];
            var p2 = new Point(p1.x - 7 * eps, p1.y + 2 * eps);
            points.Add(p2);
            return new PointsWithResult(points, new Tuple<Point, Point>(p1, p2), Point.Distance(p1, p2));
        }

        private static PointsWithResult CreatePerformanceTest3()
        {
            int chmuraCount = 1000000;
            float eps = 0.01f;
            Random random = new Random(1024);
            var points = Enumerable.Repeat(0, chmuraCount).Select(i => new Point(random.Next(100, 300), random.Next(50, 300))).Distinct().ToList();
            var p1 = points[random.Next(0, points.Count)];
            var p2 = new Point(p1.x - 2 * eps, p1.y + 7 * eps);
            points.Add(p2);
            return new PointsWithResult(points, new Tuple<Point, Point>(p1, p2), Point.Distance(p1, p2));
        }

        private static PointsWithResult CreatePerformanceTest4()
        {
            int chmuraCount = 100000;
            float eps = 0.01f;
            Random random = new Random(1024);
            var points = Enumerable.Repeat(0, chmuraCount).Select(i => new Point(random.Next(20, 300), random.Next(20, 300))).Distinct().ToList();
            var p1 = points[random.Next(0, points.Count)];
            var p2 = new Point(p1.x - 9 * eps, p1.y + 9 * eps);
            points.Add(p2);
            return new PointsWithResult(points, new Tuple<Point, Point>(p1, p2), Point.Distance(p1, p2));
        }

        private static PointsWithResult CreatePerformanceTest5()
        {
            int chmuraCount = 100000;
            float eps = 0.01f;
            Random random = new Random(1024);
            var points = Enumerable.Repeat(0, chmuraCount).Select(i => new Point(200 * random.NextDouble(), 200 * random.NextDouble())).Distinct().ToList();
            var minX = points.Min(p => p.x);
            var minY = points.Min(p => p.y);
            var p1 = points[random.Next(0, points.Count)];
            var p2 = new Point(p1.x - eps * minX, p1.y + eps * minY);
            points.Add(p2);
            return new PointsWithResult(points, new Tuple<Point, Point>(p1, p2), Point.Distance(p1, p2));
        }

        private static PointsWithResult CreatePerformanceTest6()
        {
            int chmuraCount = 100000;
            float eps = 0.01f;
            Random random = new Random(512);
            var points = Enumerable.Repeat(0, chmuraCount).Select(i => new Point(200 * random.NextDouble(), 200 * random.NextDouble())).Distinct().ToList();
            var minX = points.Min(p => p.x);
            var minY = points.Min(p => p.y);
            var p1 = points[random.Next(0, points.Count)];
            var p2 = new Point(p1.x - eps * minX, p1.y + eps * minY);
            points.Add(p2);
            return new PointsWithResult(points, new Tuple<Point, Point>(p1, p2), Point.Distance(p1, p2));
        }

        public class Lab12Tests : TestModule
        {
            TestSet simpleTestsBrute = new TestSet(prototypeObject: new Lab12(), description: "Brute force algorithm tests", settings: true);
            TestSet simpleTests = new TestSet(prototypeObject: new Lab12(), description: "Sweep line algorithm tests", settings: true);
            TestSet performanceTests = new TestSet(prototypeObject: new Lab12(), description: "Sweep line algorithm performance tests", settings: true);

            public override void PrepareTestSets()
            {
                TestSets["BF"] = simpleTestsBrute;
                TestSets["SL"] = simpleTests;
                TestSets["PT"] = performanceTests;
                PointsWithResult pwr = CreateBasicTest1();
                simpleTestsBrute.TestCases.Add(new ClosestPointsTestCase(6, pwr.points, pwr.pair, pwr.minDistance, true));
                pwr = CreateBasicTest2();
                simpleTestsBrute.TestCases.Add(new ClosestPointsTestCase(6, pwr.points, pwr.pair, pwr.minDistance, true));
                pwr = CreateBasicTest3();
                simpleTestsBrute.TestCases.Add(new ClosestPointsTestCase(6, pwr.points, pwr.pair, pwr.minDistance, true));
                pwr = CreateBasicTest4();
                simpleTestsBrute.TestCases.Add(new ClosestPointsTestCase(6, pwr.points, pwr.pair, pwr.minDistance, true));
                pwr = CreateBasicTest5();
                simpleTestsBrute.TestCases.Add(new ClosestPointsTestCase(6, pwr.points, pwr.pair, pwr.minDistance, true));
                pwr = CreateBasicTest6();
                simpleTestsBrute.TestCases.Add(new ClosestPointsTestCase(6, pwr.points, pwr.pair, pwr.minDistance, true));


                pwr = CreateBasicTest1();
                simpleTests.TestCases.Add(new ClosestPointsTestCase(6, pwr.points, pwr.pair, pwr.minDistance));
                pwr = CreateBasicTest2();
                simpleTests.TestCases.Add(new ClosestPointsTestCase(6, pwr.points, pwr.pair, pwr.minDistance));
                pwr = CreateBasicTest3();
                simpleTests.TestCases.Add(new ClosestPointsTestCase(6, pwr.points, pwr.pair, pwr.minDistance));
                pwr = CreateBasicTest4();
                simpleTests.TestCases.Add(new ClosestPointsTestCase(6, pwr.points, pwr.pair, pwr.minDistance));
                pwr = CreateBasicTest5();
                simpleTests.TestCases.Add(new ClosestPointsTestCase(6, pwr.points, pwr.pair, pwr.minDistance));
                pwr = CreateBasicTest6();
                simpleTests.TestCases.Add(new ClosestPointsTestCase(6, pwr.points, pwr.pair, pwr.minDistance));

                if (!skiBigTests)
                {
                    pwr = CreatePerformanceTest1();
                    performanceTests.TestCases.Add(new ClosestPointsTestCase(1, pwr.points, pwr.pair, pwr.minDistance));
                    pwr = CreatePerformanceTest2();
                    performanceTests.TestCases.Add(new ClosestPointsTestCase(1, pwr.points, pwr.pair, pwr.minDistance));
                    pwr = CreatePerformanceTest3();
                    performanceTests.TestCases.Add(new ClosestPointsTestCase(3, pwr.points, pwr.pair, pwr.minDistance));
                    pwr = CreatePerformanceTest4();
                    performanceTests.TestCases.Add(new ClosestPointsTestCase(3, pwr.points, pwr.pair, pwr.minDistance));
                    pwr = CreatePerformanceTest5();
                    performanceTests.TestCases.Add(new ClosestPointsTestCase(6, pwr.points, pwr.pair, pwr.minDistance));
                    pwr = CreatePerformanceTest6();
                    performanceTests.TestCases.Add(new ClosestPointsTestCase(6, pwr.points, pwr.pair, pwr.minDistance));
                }
            }
        }

        static void Main(string[] args)
        {
            var tests = new Lab12Tests();
            tests.PrepareTestSets();


            foreach (var ts in tests.TestSets)
            {
                ts.Value.PerformTests(verbose: true, checkTimeLimit: false);
            }

        }
    }
}
