﻿using System;

namespace ASD
{
    public class Lab14Stage1TestCase : TestCase
    {
        private string text;
        private string pattern;
        private int expectedResult;

        private int actualResult;
        private string actualString;

        public Lab14Stage1TestCase(
            string text,
            string pattern,
            int expectedResult,
            double timeLimit,
            string description
        ) : base(timeLimit, null, description)
        {
            this.text = text;
            this.pattern = pattern;
            this.expectedResult = expectedResult;  
        }

        protected override void PerformTestCase(object prototypeObject)
        {
            var solution = (Lab14)prototypeObject;

            actualResult = solution.Stage1((string)text.Clone(), (string)pattern.Clone(), out actualString);
        }

        protected override (Result resultCode, string message) VerifyTestCase(object settings)
        {
            if (actualResult != expectedResult)
                return (Result.WrongResult, $"Zwrócona długość słowa: {actualResult}, oczekiwano {expectedResult}");
            if (actualString == null)
                return (Result.WrongResult, $"Zwrócona wartość OK, zwrócono null jako słowo wynikowe");
            if (actualString.Length != actualResult)
                return (Result.WrongResult, $"Zwrócona wartość OK, długość słowa wynikowego ({actualString.Length}) nie jest równa zwróconej wartości ({actualResult})");
            if (!text.Contains(actualString))
                return (Result.WrongResult, $"Zwrócona wartość OK, słowo wynikowe nie jest podsłowem wejściowego");

            return (
                TimeLimit <= PerformanceTime ? Result.LowEfficiency : Result.Success,
                $"OK {PerformanceTime:0.00}s, [{Description}]"
                );
        }
    }

    public class Lab14Stage2TestCase : TestCase
    {
        private string word;
        private int expectedResult;

        private int actualResult;
        private string actualString;

        public Lab14Stage2TestCase(
            string word,
            int expectedResult,
            double timeLimit,
            string description
        ) : base(timeLimit, null, description)
        {
            this.word = word;
            this.expectedResult = expectedResult;
        }

        protected override void PerformTestCase(object prototypeObject)
        {
            var solution = (Lab14)prototypeObject;

            actualResult = solution.Stage2((string)word.Clone(), out actualString);
        }

        protected override (Result resultCode, string message) VerifyTestCase(object settings)
        {
            if (actualResult != expectedResult)
                return (Result.WrongResult, $"Zwrócona długość słowa: {actualResult}, oczekiwano {expectedResult}");
            if (actualString == null)
                return (Result.WrongResult, $"Zwrócona wartość OK, zwrócono null jako słowo wynikowe");
            if (actualString.Length != actualResult)
                return (Result.WrongResult, $"Zwrócona wartość OK, długość słowa wynikowego ({actualString.Length}) nie jest równa zwróconej wartości ({actualResult})");
            if (!word.Contains(actualString))
                return (Result.WrongResult, $"Zwrócona wartość OK, słowo wynikowe nie jest podsłowem wejściowego");

            return (
                TimeLimit <= PerformanceTime ? Result.LowEfficiency : Result.Success,
                $"OK {PerformanceTime:0.00}s, [{Description}]"
                );
        }
    }

    public class Lab14Tests : TestModule
    {
        private readonly TestSet Stage1 = new TestSet(new Lab14(), "Etap I");
        private readonly TestSet Stage2 = new TestSet(new Lab14(), "Etap II - poprawność");
        private readonly TestSet Stage2eff = new TestSet(new Lab14(), "Etap II - wydajność");

        public override void PrepareTestSets()
        {
            TestSets["Stage1"] = Stage1;
            TestSets["Stage2"] = Stage2;
            TestSets["Stage2eff"] = Stage2eff;

            PrepareStage1Tests();
            PrepareStage2Tests();
            PrepareStage2EfficiencyTests();
        }

        private void AddStage1Test(
            string text,
            string pattern,
            int expectedResult,
            double timeLimit,
            string description
        )
        {
            Stage1.TestCases.Add(
                new Lab14Stage1TestCase(text, pattern, expectedResult, timeLimit, description)
            );
        }

        private void AddStage2Test(
            string word,
            int expectedResult,
            double timeLimit,
            string description
        )
        {
            Stage2.TestCases.Add(
                new Lab14Stage2TestCase(word, expectedResult, timeLimit, description)
            );
        }

        private void AddStage2EfficiencyTest(
            string word,
            int expectedResult,
            double timeLimit,
            string description
        )
        {
            Stage2eff.TestCases.Add(
                new Lab14Stage2TestCase(word, expectedResult, timeLimit, description)
            );
        }

        // Funkcje pomocnicze do testow:

        string MakeRandomStage1TestFromPatternChars(string pattern, int[] prefixSuffixLengths, int length, int resultSizeHint, int seed)
        {
            Random random = new Random(seed);
            char[] result = new char[length];
            
            int patternIdx = random.Next(length - resultSizeHint);
            int currentIdx = patternIdx;

            while (currentIdx + pattern.Length <= length)
            {
                for (int i = currentIdx; i < currentIdx + pattern.Length; i++)
                {
                    result[i] = pattern[i - currentIdx];
                }
                currentIdx += pattern.Length;
                int offset = prefixSuffixLengths[random.Next(prefixSuffixLengths.Length)];
                if (currentIdx - offset + pattern.Length > length || currentIdx - patternIdx >= resultSizeHint)
                    break;
                currentIdx -= offset;
            }

            for (int i = 0; i < patternIdx; ++i)
                result[i] = pattern[random.Next(pattern.Length)];
            for (int i = currentIdx; i < length; ++i)
                result[i] = pattern[random.Next(pattern.Length)];

            return new string(result);
        }

        string MakeRandomStage2Test(string pattern, int[] prefixSuffixLengths, int lengthHint, int seed)
        {
            Random random = new Random(seed);
            char[] result = new char[lengthHint];

            int currentIdx = 0;
            while (currentIdx + pattern.Length <= lengthHint)
            {
                for (int i = currentIdx; i < currentIdx + pattern.Length; i++)
                {
                    result[i] = pattern[i - currentIdx];
                }
                currentIdx += pattern.Length;
                int offset = prefixSuffixLengths[random.Next(prefixSuffixLengths.Length)];
				if (currentIdx - offset + pattern.Length > lengthHint)
					break;
                currentIdx -= offset;
            }

            char[] newResult = new char[currentIdx];
            Array.Copy(result, newResult, currentIdx);
            return new string(newResult);
        }

        string MakeRandomString(char min, char max, int length, int seed)
        {
            Random random = new Random(seed);
            char[] result = new char[length];
            for (int i = 0; i < length; ++i)
                result[i] = (char)random.Next(min, max + 1);
            return new string(result);
        }
        
        // --- ETAP 1 -----------------------------------------------------------

        private void PrepareStage1Tests()
        {
            {
                string text = "abbababaababbaba";
                string pattern = "aba";
                int expectedResult = 8;

                AddStage1Test(text, pattern, expectedResult, 1, "Przykład z treści zadania");
            }

            {
                string text = "";
                string pattern = "aaaa";
                int expectedResult = 0;

                AddStage1Test(text, pattern, expectedResult, 1, "Puste słowo");
            }

            {
                string text = "asd";
                string pattern = "asdasd";
                int expectedResult = 0;

                AddStage1Test(text, pattern, expectedResult, 1, "Wzorzec dłuższy niż słowo wejściowe");
            }

            {
                string text = "abc";
                string pattern = "abc";
                int expectedResult = 3;

                AddStage1Test(text, pattern, expectedResult, 1, "Wzorzec taki sam, jak wejściowe słowo");
            }

            {
                string text = "aaaaaaaaaaaaaaaaaaaaa";
                string pattern = "a";
                int expectedResult = text.Length;

                AddStage1Test(text, pattern, expectedResult, 1, "Wszystkie znaki takie same");
            }

            {
                string text = "abcabcabcb";
                string pattern = "xyz";
                int expectedResult = 0;

                AddStage1Test(text, pattern, expectedResult, 1, "Nieistniejący wzorzec");
            }

            {
                string text = "yzabab";
                string pattern = "xyz";
                int expectedResult = 0;

                AddStage1Test(text, pattern, expectedResult, 1, "Niepełne wystąpienie na początku");
            }

            {
                string text = "ababxy";
                string pattern = "xyz";
                int expectedResult = 0;

                AddStage1Test(text, pattern, expectedResult, 1, "Niepełne wystąpienie na końcu");
            }

            {
                string text = "sopopsop";
                string pattern = "sop";
                int expectedResult = 3;

                AddStage1Test(text, pattern, expectedResult, 1, "SOP");
            }

            {
                string pattern = "xyzabxyzabxyz";
                int[] prefixSuffixLengths = new int[] { 0, 3, 8 };
                string text = MakeRandomStage1TestFromPatternChars(pattern, prefixSuffixLengths, 10000, 100, 6767);
                int expectedResult = 109;

                AddStage1Test(text, pattern, expectedResult, 1, "Mały test losowy");
            }

            {
                string pattern = "abcdefabcdefabcdefa";
                int[] prefixSuffixLengths = new int[] { 0, 1, 7, 13 };
                string text = MakeRandomStage1TestFromPatternChars(pattern, prefixSuffixLengths, 1000000, 1000, 44);
                int expectedResult = 1014;

                AddStage1Test(text, pattern, expectedResult, 1, "Średni test losowy");
            }

            {
                string pattern = "aaabbbaaabbbaaa";
                int[] prefixSuffixLengths = new int[] { 0, 3, 9 };
                string text = MakeRandomStage1TestFromPatternChars(pattern, prefixSuffixLengths, 100000000, 53028, 12345);
                int expectedResult = 53031;

                AddStage1Test(text, pattern, expectedResult, 10, "Duży test losowy");
            }
        }

        // --- ETAP 2 poprawność --------------------------------------

        private void PrepareStage2Tests()
        {
            {
                string word = "ababaaba";
                int expectedResult = 3;

                AddStage2Test(word, expectedResult, 1, "Pierwszy przykład z treści zadania");
            }

            {
                string word = "slowo";
                int expectedResult = 5;

                AddStage2Test(word, expectedResult, 1, "Drugi przykład z treści zadania");
            }

            {
                string word = "";
                int expectedResult = 0;

                AddStage2Test(word, expectedResult, 1, "Puste słowo");
            }

            {
                string word = "%";
                int expectedResult = 1;

                AddStage2Test(word, expectedResult, 1, "Jednoznakowe słowo");
            }

            {
                string word = "123123123123123123123";
                int expectedResult = 3;

                AddStage2Test(word, expectedResult, 1, "Potęga słowa 123");
            }

            {
                string word = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
                int expectedResult = 1;

                AddStage2Test(word, expectedResult, 1, "Wszystkie znaki takie same");
            }

            {
                string word = "abcabcabca";
                int expectedResult = 4;

                AddStage2Test(word, expectedResult, 1, "Niedokończone wystąpienie wzorca na końcu");
            }

            {
                string word = "bumbumbump";
                int expectedResult = word.Length;

                AddStage2Test(word, expectedResult, 1, "Prawie okresowe");
            }

            {
                string word = "aaaaabaaaaa";
                int expectedResult = word.Length;

                AddStage2Test(word, expectedResult, 1, "Intruz pośrodku");
            }

			{ 
                string word = "abaababa"; 
                int expectedResult = 3; 

                AddStage2Test(word, expectedResult, 1, "Adam: Nachodzące pokrycia bez okresowości"); 
            } 

            { 
                string word = "aabaabaaabaa"; 
                int expectedResult = 5; 

                AddStage2Test(word, expectedResult, 1, "Adam: Pokrycie z przerwą domykaną sufiksem"); 
            }

            {
                string pattern = "xyzabxyz";
                int[] prefixSuffixLengths = new int[] { 0, 3 };
                string word = MakeRandomStage2Test(pattern, prefixSuffixLengths, 10000, 3462);
                int expectedResult = pattern.Length;

                AddStage2Test(word, expectedResult, 10, "Mały test losowy");
            }

            {
                string word = MakeRandomString('a', 'g', 10000, 817348);
                int expectedResult = word.Length;

                AddStage2Test(word, expectedResult, 10, "Mały test kompletnie losowy");
            }
        }

        // --- ETAP 2 wydajność --------------------------------------------

        private void PrepareStage2EfficiencyTests()
        {
            {
                string pattern = "abaabbabaa";
                int[] prefixSuffixLengths = new int[] { 0, 4 };
                string word = MakeRandomStage2Test(pattern, prefixSuffixLengths, 1000000, 16643);
                int expectedResult = pattern.Length;

                AddStage2EfficiencyTest(word, expectedResult, 1, "Średni test losowy");
            }

            {
                string word = MakeRandomString('a', 'g', 1000000, 3321);
                int expectedResult = word.Length;

                AddStage2EfficiencyTest(word, expectedResult, 1, "Średni test kompletnie losowy");
            }

            {
                string pattern = "84428";
                int[] prefixSuffixLengths = new int[] { 0, 1 };
                string word = MakeRandomStage2Test(pattern, prefixSuffixLengths, 50000000, 826745);
                int expectedResult = pattern.Length;

                AddStage2EfficiencyTest(word, expectedResult, 20, "Duży test losowy");
            }

            {
                string word = MakeRandomString('a', 'z', 50000000, 12373);
                int expectedResult = word.Length;

                AddStage2EfficiencyTest(word, expectedResult, 20, "Duży test kompletnie losowy");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var tests = new Lab14Tests();
            tests.PrepareTestSets();

            foreach (var ts in tests.TestSets)
            {
                ts.Value.PerformTests(verbose: true, checkTimeLimit: false);
                Console.WriteLine();
            }
        }
    }
}