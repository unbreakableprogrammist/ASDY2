using ASD.Graphs.Testing;
using System;

namespace ASD
{
    public abstract class Lab12TestCase : TestCase
    {
        protected const double Eps = 1e-6;

        protected readonly double expectedValue;
        protected double result;

        protected Lab12TestCase(
            double expectedValue,
            double timeLimit,
            string description
        ) : base(timeLimit, null, description)
        {
            this.expectedValue = expectedValue;
        }

        protected (Result resultCode, string message) CheckReturnedValue()
        {
            if (Math.Abs(result - expectedValue) > Eps)
                return (
                    Result.WrongResult,
                    $"Zwrócono D = {result:0.######}, oczekiwano {expectedValue:0.######} [{Description}]"
                );

            return (
                TimeLimit < PerformanceTime ? Result.LowEfficiency : Result.Success,
                $"OK {PerformanceTime:0.00}s [{Description}]"
            );
        }

        protected static double[] CloneSample(double[] sample)
        {
            return (double[])sample.Clone();
        }

        protected static double[][] CloneSamples(double[][] samples)
        {
            double[][] result = new double[samples.Length][];

            for (int i = 0; i < samples.Length; i++)
                result[i] = CloneSample(samples[i]);

            return result;
        }
    }

    public class Stage1TestCase : Lab12TestCase
    {
        private readonly double[] sample1;
        private readonly double[] sample2;

        public Stage1TestCase(
            double[] sample1,
            double[] sample2,
            double expectedValue,
            double timeLimit,
            string description
        ) : base(expectedValue, timeLimit, description)
        {
            this.sample1 = CloneSample(sample1);
            this.sample2 = CloneSample(sample2);
        }

        protected override void PerformTestCase(object prototypeObject)
        {
            var solution = (Lab12)prototypeObject;

            result = solution.Stage1(CloneSample(sample1), CloneSample(sample2));
        }

        protected override (Result resultCode, string message) VerifyTestCase(object settings)
        {
            return CheckReturnedValue();
        }
    }

    public class Stage2TestCase : Lab12TestCase
    {
        private readonly double[][] samples;

        public Stage2TestCase(
            double[][] samples,
            double expectedValue,
            double timeLimit,
            string description
        ) : base(expectedValue, timeLimit, description)
        {
            this.samples = CloneSamples(samples);
        }

        protected override void PerformTestCase(object prototypeObject)
        {
            var solution = (Lab12)prototypeObject;

            result = solution.Stage2(CloneSamples(samples));
        }

        protected override (Result resultCode, string message) VerifyTestCase(object settings)
        {
            return CheckReturnedValue();
        }
    }

    public class Stage3TestCase : Lab12TestCase
    {
        private readonly double[][] samples;

        public Stage3TestCase(
            double[][] samples,
            double expectedValue,
            double timeLimit,
            string description
        ) : base(expectedValue, timeLimit, description)
        {
            this.samples = CloneSamples(samples);
        }

        protected override void PerformTestCase(object prototypeObject)
        {
            var solution = (Lab12)prototypeObject;

            result = solution.Stage3(CloneSamples(samples));
        }

        protected override (Result resultCode, string message) VerifyTestCase(object settings)
        {
            return CheckReturnedValue();
        }
    }

    public class Lab12Tests : TestModule
    {
        private readonly TestSet Stage1 = new TestSet(new Lab12(), "1 - dwie próbki");
        private readonly TestSet Stage2 = new TestSet(new Lab12(), "2 - K próbek, O(nK)");
        private readonly TestSet Stage3 = new TestSet(new Lab12(), "3 - K próbek, O(n log K)");

        public override void PrepareTestSets()
        {
            TestSets["Stage1"] = Stage1;
            TestSets["Stage2"] = Stage2;
            TestSets["Stage3"] = Stage3;

            PrepareStage1Tests();
            PrepareStage2And3Tests();
            PrepareStage3Tests();
        }

        private void AddStage1Test(
            double[] sample1,
            double[] sample2,
            double expectedValue,
            double timeLimit,
            string description
        )
        {
            Stage1.TestCases.Add(
                new Stage1TestCase(sample1, sample2, expectedValue, timeLimit, description)
            );
        }

        private void AddStage2Test(
            double[][] samples,
            double expectedValue,
            double timeLimit,
            string description
        )
        {
            Stage2.TestCases.Add(
                new Stage2TestCase(samples, expectedValue, timeLimit, description)
            );
        }

        private void AddStage3Test(
            double[][] samples,
            double expectedValue,
            double timeLimit,
            string description
        )
        {
            Stage3.TestCases.Add(
                new Stage3TestCase(samples, expectedValue, timeLimit, description)
            );
        }

        private void AddStage2And3Test(
            double[][] samples,
            double expectedValue,
            double timeLimit,
            string description
        )
        {
            AddStage2Test(samples, expectedValue, timeLimit, description);
            AddStage3Test(samples, expectedValue, timeLimit, description);
        }
		
		// Funkcje pomocnicze do testow:
		
		private static double[] MakeRangeSample(int start, int length)
		{
		    double[] sample = new double[length];

		    for (int i = 0; i < length; i++)
		        sample[i] = start + i;

		    return sample;
		}

		private static double[] MakeConstantSample(double value, int length)
		{
		    double[] sample = new double[length];

		    for (int i = 0; i < length; i++)
		        sample[i] = value;

		    return sample;
		}

		private static double[][] MakeBlockSamples(int K, int n)
		{
		    double[][] samples = new double[K][];

		    for (int i = 0; i < K; i++)
		    {
		        samples[i] = new double[n];

		        for (int j = 0; j < n; j++)
		            samples[i][j] = i * n + j;
		    }

		    return samples;
		}

		private static double[][] MakeInterleavedSamples(int K, int n)
		{
		    double[][] samples = new double[K][];

		    for (int i = 0; i < K; i++)
		    {
		        samples[i] = new double[n];

		        for (int j = 0; j < n; j++)
		            samples[i][j] = j * K + i;
		    }

		    return samples;
		}

		private static double[][] MakeIdenticalSamples(int K, int n)
		{
		    double[][] samples = new double[K][];

		    for (int i = 0; i < K; i++)
		        samples[i] = MakeRangeSample(0, n);

		    return samples;
		}
		
		private static double[][] MakeShiftedSamples(int K, int n, int shift)
		{
		    double[][] samples = new double[K][];

		    for (int i = 0; i < K; i++)
		    {
		        samples[i] = new double[n];

		        for (int j = 0; j < n; j++)
		            samples[i][j] = j * shift + i;
		    }

		    return samples;
		}

		private static double[][] MakeSparseInterleavedSamples(int K, int n)
		{
		    double[][] samples = new double[K][];

		    for (int i = 0; i < K; i++)
		    {
		        samples[i] = new double[n];

		        for (int j = 0; j < n; j++)
		            samples[i][j] = j * K + i;
		    }

		    return samples;
		}

		private static double[][] MakeDifferentLengthInterleavedSamples(int K, int minN, int maxN)
		{
		    double[][] samples = new double[K][];

		    for (int i = 0; i < K; i++)
		    {
		        int n = minN + (i % (maxN - minN + 1));
		        samples[i] = new double[n];

		        for (int j = 0; j < n; j++)
		            samples[i][j] = j * K + i;
		    }

		    return samples;
		}
		
		private static double[] MakeRandomSample(int n, int minValue, int maxValue, int seed)
		{
		    Random random = new Random(seed);
		    double[] sample = new double[n];

		    for (int i = 0; i < n; i++)
		        sample[i] = random.Next(minValue, maxValue + 1);

		    Array.Sort(sample);
		    return sample;
		}

		private static double[][] MakeRandomSamples(int K, int minN, int maxN, int minValue, int maxValue, int seed)
		{
		    Random random = new Random(seed);
		    double[][] samples = new double[K][];

		    for (int i = 0; i < K; i++)
		    {
		        int n = random.Next(minN, maxN + 1);
		        samples[i] = new double[n];

		        for (int j = 0; j < n; j++)
		            samples[i][j] = random.Next(minValue, maxValue + 1);

		        Array.Sort(samples[i]);
		    }

		    return samples;
		}
		
		private static double[][] MakeRandomAlmostUniqueSamples(
		    int K,
		    int minN,
		    int maxN,
		    int seed
		)
		{
		    Random random = new Random(seed);
		    double[][] samples = new double[K][];

		    for (int i = 0; i < K; i++)
		    {
		        int n = random.Next(minN, maxN + 1);
		        samples[i] = new double[n];

		        for (int j = 0; j < n; j++)
		        {
		            // Część j * K + i zapewnia uporządkowane, prawie niekolidujące wartości
		            // między próbkami. Losowy składnik dodaje trochę losowości, ale nie zmienia
		            // kolejności wewnątrz próbki.
		            samples[i][j] = (double)j * K + i + random.NextDouble() * 0.1;
		        }
		    }

		    return samples;
		}

		// --- ETAP 1 -----------------------------------------------------------

		private void PrepareStage1Tests()
		{
		    {
		        double[] sample1 = { 1, 2, 4 };
		        double[] sample2 = { 1, 3, 3, 5 };

		        AddStage1Test(
		            sample1,
		            sample2,
		            5.0 / 12.0,
		            1,
		            "Przykład z treści zadania"
		        );
		    }

		    {
		        double[] sample1 = { 1, 2, 3 };
		        double[] sample2 = { 1, 2, 3 };

		        AddStage1Test(
		            sample1,
		            sample2,
		            0.0,
		            1,
		            "Identyczne próbki"
		        );
		    }

		    {
		        double[] sample1 = { 1, 2, 3 };
		        double[] sample2 = { 4, 5, 6 };

		        AddStage1Test(
		            sample1,
		            sample2,
		            1.0,
		            1,
		            "Rozłączne próbki"
		        );
		    }

		    {
		        double[] sample1 = { 1, 1, 1 };
		        double[] sample2 = { 1, 1, 2 };

		        AddStage1Test(
		            sample1,
		            sample2,
		            1.0 / 3.0,
		            1,
		            "Powtórzenia w próbkach"
		        );
		    }

		    {
		        double[] sample1 = { -3, -1, 2 };
		        double[] sample2 = { -2, 0, 4 };

		        AddStage1Test(
		            sample1,
		            sample2,
		            1.0 / 3.0,
		            1,
		            "Wartości ujemne"
		        );
		    }

		    {
		        double[] sample1 = { 1 };
		        double[] sample2 = { 1, 2, 3, 4 };

		        AddStage1Test(
		            sample1,
		            sample2,
		            3.0 / 4.0,
		            1,
		            "Próbki różnych długości"
		        );
		    }
			
			{
			    double[] sample1 = { 1, 1, 1, 1 };
			    double[] sample2 = { 0, 2, 3, 4 };
			    AddStage1Test(
			        sample1,
			        sample2,
			        3.0 / 4.0,
			        1,
			        "Duży skok przez powtórzenia"
			    );
			}
			
			{
			    double[] sample1 = { 1, 2, 3, 100 };
			    double[] sample2 = { 1, 2, 3, 4 };
			    AddStage1Test(
			        sample1,
			        sample2,
			        1.0 / 4.0,
			        1,
			        "Maksimum przed ostatnim elementem"
			    );
			}
			
			{
			    double[] sample1 = { -5, -4, -3 };
			    double[] sample2 = { -2 };
			    AddStage1Test(
			        sample1,
			        sample2,
			        1.0,
			        1,
			        "Wartości ujemne i różne długości"
			    );
			}
			
			{
			    int n = 5000000;

			    double[] sample1 = MakeRangeSample(0, n);
			    double[] sample2 = MakeRangeSample(n, n);

			    AddStage1Test(
			        sample1,
			        sample2,
			        1.0,
			        2,
			        "Wydajność: dwie duże rozłączne próbki"
			    );
			}

			{
			    int n = 5000000;

			    double[] sample1 = new double[n];
			    double[] sample2 = new double[n];

			    for (int i = 0; i < n; i++)
			    {
			        sample1[i] = 2 * i;
			        sample2[i] = 2 * i + 1;
			    }

			    AddStage1Test(
			        sample1,
			        sample2,
			        1.0 / n,
			        2,
			        "Wydajność: dwie duże przeplatane próbki"
			    );
			}

			{
			    int n = 9000000;

			    double[] sample1 = MakeConstantSample(0.0, n);
			    double[] sample2 = new double[n];

			    for (int i = 0; i < n / 2; i++)
			        sample2[i] = 0.0;

			    for (int i = n / 2; i < n; i++)
			        sample2[i] = 1.0;

			    AddStage1Test(
			        sample1,
			        sample2,
			        0.5,
			        2,
			        "Wydajność: duże próbki z powtórzeniami"
			    );
			}
			
			// --- ETAP 1: testy losowe ----------------------------------------------------

			{
			    double[] sample1 = MakeRandomSample(1000000, -10, 10, 101);
			    double[] sample2 = MakeRandomSample(1500000, -10, 10, 102);

			    AddStage1Test(
			        sample1,
			        sample2,
			        0.001067,
			        1,
			        "Losowy mały"
			    );
			}

			{
			    double[] sample1 = MakeRandomSample(10000000, -100, 100, 201);
			    double[] sample2 = MakeRandomSample(18000000, -100, 100, 202);

			    AddStage1Test(
			        sample1,
			        sample2,
			        0.000285,
			        5,
			        "Losowy średni"
			    );
			}

			{
			    double[] sample1 = MakeRandomSample(24000000, -50, 40, 301);
			    double[] sample2 = MakeRandomSample(17000000, -40, 50, 302);

			    AddStage1Test(
			        sample1,
			        sample2,
			        0.110084,
			        10,
			        "Losowy duży"
			    );
			}
		}

		// --- ETAPY 2 i 3: testy wspólne --------------------------------------

		private void PrepareStage2And3Tests()
		{
		    {
		        double[][] samples =
		        {
		            new double[] { 1, 2, 4 },
		            new double[] { 1, 3, 3, 5 },
		            new double[] { 4, 5, 5 }
		        };

		        AddStage2And3Test(
		            samples,
		            3.0 / 4.0,
		            1,
		            "Przykład z treści zadania"
		        );
		    }

		    {
		        double[][] samples =
		        {
		            new double[] { 1, 2, 3 },
		            new double[] { 1, 2, 3 },
		            new double[] { 1, 2, 3 }
		        };

		        AddStage2And3Test(
		            samples,
		            0.0,
		            1,
		            "Identyczne próbki"
		        );
		    }

		    {
		        double[][] samples =
		        {
		            new double[] { 1, 2 },
		            new double[] { 3, 4 },
		            new double[] { 5, 6 }
		        };

		        AddStage2And3Test(
		            samples,
		            1.0,
		            1,
		            "Rozłączne próbki"
		        );
		    }

		    {
		        double[][] samples =
		        {
		            new double[] { 1, 1, 1 },
		            new double[] { 1, 2, 2 },
		            new double[] { 2, 2, 2 }
		        };

		        AddStage2And3Test(
		            samples,
		            1.0,
		            1,
		            "Powtórzenia w wielu próbkach"
		        );
		    }

		    {
		        double[][] samples =
		        {
		            new double[] { -3, -1, 2 },
		            new double[] { -2, 0, 4 },
		            new double[] { -4, 5 }
		        };

		        AddStage2And3Test(
		            samples,
		            1.0 / 2.0,
		            1,
		            "Wartości ujemne"
		        );
		    }

		    {
		        double[][] samples =
		        {
		            new double[] { 1 },
		            new double[] { 1, 2, 3, 4 },
		            new double[] { 2, 3 }
		        };

		        AddStage2And3Test(
		            samples,
		            1.0,
		            1,
		            "Próbki różnych długości"
		        );
		    }

		    {
		        double[][] samples =
		        {
		            new double[] { 0 },
		            new double[] { 0, 0 },
		            new double[] { 0, 0, 0 },
		            new double[] { 0, 0, 0, 0 }
		        };

		        AddStage2And3Test(
		            samples,
		            0.0,
		            1,
		            "Wszystkie wartości jednakowe"
		        );
		    }

			{
			    double[][] samples =
			    {
			        new double[] { 1, 1, 1, 1 },
			        new double[] { 1, 2, 3, 4 },
			        new double[] { 2, 2, 2, 2 }
			    };

			    AddStage2And3Test(
			        samples,
			        1.0,
			        1,
			        "Duży skok przez powtórzenia"
			    );
			}

			{
			    double[][] samples =
			    {
			        new double[] { 1, 4 },
			        new double[] { 2, 5 },
			        new double[] { 3, 6 }
			    };

			    AddStage2And3Test(
			        samples,
			        1.0 / 2.0,
			        1,
			        "Każda próbka zmienia się osobno"
			    );
			}

			{
			    double[][] samples =
			    {
			        new double[] { 1, 3 },
			        new double[] { 1, 4 },
			        new double[] { 1, 5 }
			    };

			    AddStage2And3Test(
			        samples,
			        1.0 / 2.0,
			        1,
			        "Kilka próbek ma ten sam aktualny x"
			    );
			}

			{
			    double[][] samples =
			    {
			        new double[] { 1 },
			        new double[] { 2 },
			        new double[] { 3 },
			        new double[] { 4 }
			    };

			    AddStage2And3Test(
			        samples,
			        1.0,
			        1,
			        "Wiele jednoelementowych próbek"
			    );
			}
			
			{
			    int K = 100;
			    int n = 1000;

			    double[][] samples = MakeIdenticalSamples(K, n);

			    AddStage2And3Test(
			        samples,
			        0.0,
			        10,
			        "Wydajność: wiele identycznych próbek"
			    );
			}

			{
			    int K = 70;
			    int n = 1000;

			    double[][] samples = MakeBlockSamples(K, n);

			    AddStage2And3Test(
			        samples,
			        1.0,
			        10,
			        "Wydajność: próbki w rozłącznych blokach"
			    );
			}

			{
			    int K = 50;
			    int n = 1600;

			    double[][] samples = MakeInterleavedSamples(K, n);

			    AddStage2And3Test(
			        samples,
			        1.0 / n,
			        10,
			        "Wydajność: mocno przeplatane próbki"
			    );
			}
			
			// --- ETAPY 2 i 3: testy losowe ----------------------------------------------

			{
			    double[][] samples = MakeRandomSamples(
			        K: 40,
			        minN: 200,
			        maxN: 500,
			        minValue: -100,
			        maxValue: 100,
			        seed: 401
			    );

			    AddStage2And3Test(
			        samples,
			        0.166467,
			        1,
			        "Losowy mały"
			    );
			}

			{
			    double[][] samples = MakeRandomSamples(
			        K: 150,
			        minN: 800,
			        maxN: 1200,
			        minValue: -50000,
			        maxValue: 50000,
			        seed: 501
			    );

			    AddStage2And3Test(
			        samples,
			        0.092656,
			        5,
			        "Losowy średni"
			    );
			}

			{
			    double[][] samples = MakeRandomSamples(
			        K: 200,
			        minN: 1000,
			        maxN: 2000,
			        minValue: -50000,
			        maxValue: 100000,
			        seed: 601
			    );

			    AddStage2And3Test(
			        samples,
			        0.086237,
			        10,
			        "Losowy duży"
			    );
			}
		}

		// --- ETAP 3: testy wydajnościowe --------------------------------------------

		private void PrepareStage3Tests()
		{
		    {
		        int K = 1150;
		        int n = 600;

		        double[][] samples = MakeSparseInterleavedSamples(K, n);

		        AddStage3Test(
		            samples,
		            1.0 / n,
		            15,
		            "Wydajność Etapu 3: wiele mocno przeplatanych próbek"
		        );
		    }

		    {
		        int K = 1700;
		        int n = 350;

		        double[][] samples = MakeSparseInterleavedSamples(K, n);

		        AddStage3Test(
		            samples,
		            1.0 / n,
		            15,
		            "Wydajność Etapu 3: bardzo wiele krótkich próbek"
		        );
		    }

		    {
		        int K = 600;
		        int n = 1500;

		        double[][] samples = MakeShiftedSamples(K, n, K + 7);

		        AddStage3Test(
		            samples,
		            1.0 / n,
		            15,
		            "Wydajność Etapu 3: przesunięte próbki bez synchronizacji"
		        );
			}

		    {
		        int K = 1500;
		        int minN = 250;
		        int maxN = 600;

		        double[][] samples = MakeDifferentLengthInterleavedSamples(K, minN, maxN);

		        AddStage3Test(
		            samples,
		            ((double)(maxN - minN + 1)) / (double)(maxN),
		            15,
		            "Wydajność Etapu 3: próbki różnych długości"
		        );
		    }
			
			// --- ETAP 3: testy wydajnościowe losowe --------------------------------------

			{
			    int K = 800;
			    int minN = 500;
			    int maxN = 800;

			    double[][] samples = MakeRandomAlmostUniqueSamples(
			        K,
			        minN,
			        maxN,
			        seed: 701
			    );

			    AddStage3Test(
			        samples,
			        0.37625,
			        15,
			        "Wydajność Etapu 3: losowe prawie unikalne wartości"
			    );
			}

			{
			    int K = 1800;
			    int minN = 250;
			    int maxN = 400;

			    double[][] samples = MakeRandomAlmostUniqueSamples(
			        K,
			        minN,
			        maxN,
			        seed: 702
			    );

			    AddStage3Test(
			        samples,
			        0.3775,
			        15,
			        "Wydajność Etapu 3: wiele losowych krótkich próbek"
			    );
			}

			{
			    int K = 400;
			    int minN = 1500;
			    int maxN = 2200;

			    double[][] samples = MakeRandomAlmostUniqueSamples(
			        K,
			        minN,
			        maxN,
			        seed: 703
			    );

			    AddStage3Test(
			        samples,
			        0.316629,
			        15,
			        "Wydajność Etapu 3: losowe długie próbki"
			    );
			}
		}

        // --- punktacja --------------------------------------------------------

        public override double ScoreResult()
        {
            double score = 0.0;

            if (Stage1.PassedCount == Stage1.TestCases.Count)
                score += 0.5;

            if (Stage2.PassedCount == Stage2.TestCases.Count)
                score += 0.5;

            if (Stage3.PassedCount == Stage3.TestCases.Count)
                score += 1.5;

            return score;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var tests = new Lab12Tests();
            tests.PrepareTestSets();

            foreach (var ts in tests.TestSets)
            {
                ts.Value.PerformTests(verbose: true, checkTimeLimit: false);
                Console.WriteLine();
            }
        }
    }
}