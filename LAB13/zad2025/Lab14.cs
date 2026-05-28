using System;
using System.Collections.Generic;

namespace ASD
{
    /// <summary>
    /// Klasa drzewa prefiksowego z możliwością wyszukiwania słów w zadanej odległości edycyjnej
    /// </summary>
    public class Lab14_Trie : System.MarshalByRefObject
    {
        // klasy TrieNode NIE WOLNO ZMIENIAĆ!
        private class TrieNode
        {
            public SortedDictionary<char, TrieNode> childs = new SortedDictionary<char, TrieNode>();
            public bool IsWord = false;
            // WordCount przechowuje liczbę słów w poddrzewie, co pozwala na operacje w czasie O(1)
            public int WordCount = 0;
        }

        private TrieNode root;

        public Lab14_Trie()
        {
            root = new TrieNode();
        }

        /// <summary>
        /// Zwraca liczbę przechowywanych słów
        /// Ma działać w czasie stałym - O(1)
        /// </summary>
        public int Count 
        { 
            get { return root.WordCount; } 
        }

        /// <summary>
        /// Zwraca liczbę przechowywanych słów o zadanym prefiksie
        /// Ma działać w czasie O(len(startWith))
        /// </summary>
        public int CountPrefix(string startWith)
        {
            TrieNode current = root;
            foreach (char c in startWith)
            {
                if (!current.childs.TryGetValue(c, out current)) 
                {
                    return 0; // Jeśli ścieżka się urywa, prefiks nie istnieje
                }
            }
            return current.WordCount;
        }

        /// <summary>
        /// Dodaje słowo do słownika
        /// Ma działać w czasie O(len(newWord))
        /// </summary>
        public bool AddWord(string newWord)
        {
            // Sprawdzamy najpierw czy słowo już istnieje, aby niepotrzebnie nie zwiększać WordCount
            if (Contains(newWord)) return false;

            TrieNode current = root;
            current.WordCount++; // Inkrementacja dla korzenia

            foreach (char c in newWord)
            {
                if (!current.childs.ContainsKey(c))
                {
                    current.childs[c] = new TrieNode();
                }
                current = current.childs[c];
                current.WordCount++; // Inkrementacja w każdym węźle ścieżki
            }
            
            current.IsWord = true;
            return true;
        }

        /// <summary>
        /// Sprawdza czy podane słowo jest przechowywane w słowniku
        /// Ma działać w czasie O(len(word))
        /// </summary>
        public bool Contains(string word)
        {
            TrieNode current = root;
            foreach (char c in word)
            {
                if (!current.childs.TryGetValue(c, out current)) return false;
            }
            return current.IsWord;
        }

        /// <summary>
        /// Usuwa podane słowo ze słownika
        /// Ma działać w czasie O(len(word))
        /// </summary>
        public bool Remove(string word)
        {
            // Przerywamy, jeśli słowa nie ma w drzewie
            if (!Contains(word)) return false;

            TrieNode current = root;
            current.WordCount--;

            for (int i = 0; i < word.Length; i++)
            {
                char c = word[i];
                TrieNode nextNode = current.childs[c];
                nextNode.WordCount--;

                // Optymalizacja: gdy poddrzewo staje się puste, usuwamy gałąź i przerywamy pętlę
                if (nextNode.WordCount == 0)
                {
                    current.childs.Remove(c);
                    return true; 
                }
                current = nextNode;
            }
            
            // Jeśli dotarliśmy do końca bez usuwania węzłów (bo mają inne odgałęzienia)
            current.IsWord = false;
            return true;
        }

        /// <summary>
        /// Zwraca wszystkie słowa o podanym prefiksie. 
        /// Ma działać w czasie O(liczba węzłów w drzewie)
        /// </summary>
        public List<string> AllWords(string startWith = "")
        {
            List<string> result = new List<string>();
            TrieNode current = root;
            
            // Znalezienie węzła startowego dla zadanego prefiksu
            foreach (char c in startWith)
            {
                if (!current.childs.TryGetValue(c, out current)) return result;
            }
            
            // Rekurencyjne zebranie słów (DFS)
            CollectWords(current, startWith, result);
            return result;
        }

        private void CollectWords(TrieNode node, string currentString, List<string> result)
        {
            if (node.IsWord) result.Add(currentString);
            
            // SortedDictionary gwarantuje porządek alfabetyczny podczas iteracji
            foreach (var child in node.childs) 
            {
                CollectWords(child.Value, currentString + child.Key, result);
            }
        }

        /// <summary>
        /// Wyszukuje w słowniku wszystkie słowa w podanej odległości edycyjnej od zadanego słowa
        /// Złożoność pesymistyczna: O(len(word) * (liczba węzłów w drzewie))
        /// </summary>
        public List<(string, int)> Search(string word, int distance = 1)
        {
            List<(string, int)> results = new List<(string, int)>();
            
            // Tworzymy początkowy wiersz algorytmu Levenshteina dla pustego prefiksu
            int[] currentRow = new int[word.Length + 1];
            for (int i = 0; i <= word.Length; i++) currentRow[i] = i;

            // Obsługa przypadku bazowego (gdy puste słowo jest w słowniku)
            if (root.IsWord && word.Length <= distance)
            {
                results.Add(("", word.Length));
            }

            // Rozpoczynamy DFS z przycinaniem (pruning)
            foreach (var kvp in root.childs)
            {
                SearchRecursive(kvp.Value, kvp.Key, word, currentRow, results, distance, kvp.Key.ToString());
            }

            return results;
        }

        private void SearchRecursive(TrieNode node, char letter, string word, int[] previousRow, List<(string, int)> results, int maxDistance, string currentPrefix)
        {
            int columns = word.Length + 1;
            int[] currentRow = new int[columns];
            
            // Koszt dodania pierwszej litery do pustego słowa wzorca
            currentRow[0] = previousRow[0] + 1;

            int minDistance = currentRow[0];

            // Wypełnianie wiersza odległości Levenshteina
            for (int c = 1; c < columns; c++)
            {
                int insertCost = currentRow[c - 1] + 1;
                int deleteCost = previousRow[c] + 1;
                int replaceCost = previousRow[c - 1] + (word[c - 1] == letter ? 0 : 1);

                currentRow[c] = Math.Min(Math.Min(insertCost, deleteCost), replaceCost);
                if (currentRow[c] < minDistance) minDistance = currentRow[c];
            }

            // Pruning (przycinanie): jeśli wszystkie koszty w wierszu przekraczają maksymalny dopuszczalny dystans, odcinamy tę gałąź
            if (minDistance > maxDistance) return;

            // Jeżeli w danym węźle kończy się słowo i dystans całkowity spełnia warunek, dodaj do wyników
            if (node.IsWord && currentRow[columns - 1] <= maxDistance)
            {
                results.Add((currentPrefix, currentRow[columns - 1]));
            }

            // Dalsza rekursja (wyniki automatycznie zachowają alfabetyczność, z racji używania SortedDictionary)
            foreach (var kvp in node.childs)
            {
                SearchRecursive(kvp.Value, kvp.Key, word, currentRow, results, maxDistance, currentPrefix + kvp.Key);
            }
        }
    }
}