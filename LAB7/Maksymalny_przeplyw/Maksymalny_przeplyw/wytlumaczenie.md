# Modelowanie Planu Produkcji (Min-Cost Max-Flow)

Ten dokument wyjaśnia podejście użyte do rozwiązania problemu optymalizacji produkcji i sprzedaży telewizorów na przestrzeni wielu tygodni, z wykorzystaniem algorytmu **Minimum Cost Maximum Flow** (Maksymalny Przepływ o Minimalnym Koszcie).

## 1. Koncepcja Algorytmu

Problem wymaga zmaksymalizowania liczby wyprodukowanych i sprzedanych telewizorów, a w drugiej kolejności (dla tej maksymalnej liczby) zmaksymalizowania zysku.

Algorytm `MinCostMaxFlow` idealnie wpisuje się w te wymagania:
1. Najpierw dąży do "przepchnięcia" jak największej ilości jednostek (telewizorów) od Źródła (S) do Ujścia (T).
2. Następnie dobiera takie ścieżki w grafie, aby suma wag (kosztów) na wykorzystanych krawędziach była jak najmniejsza.
   *(Ponieważ zysk ze sprzedaży traktujemy jako ujemny koszt, minimalizacja kosztów w grafie oznacza maksymalizację zysków w rzeczywistości).*

## 2. Architektura Grafu (Wierzchołki)

Aby algorytm "zrozumiał" upływ czasu i zasady magazynowania, musimy rozbić proces na poszczególne tygodnie. Zamiast jednego magazynu dla całej fabryki, magazyn to po prostu "krawędź w czasie" łącząca ten sam budynek na przestrzeni tygodni.

Dla planu obejmującego $n$ tygodni i $m$ klientów tworzymy następujące wierzchołki:
* **S (Źródło):** Punkt startowy generujący potencjalne telewizory.
* **T (Ujście):** Punkt końcowy, zamykający transakcje.
* **Fabryka[i]:** Hala produkcyjna w tygodniu `i`. Służy również jako punkt startowy magazynowania.
* **Klient[i][j]:** Konkretny kontrahent `j` chcący dokonać zakupu w tygodniu `i`.

## 3. Rury decyzyjne (Krawędzie)

Każda krawędź grafu (rura) definiuje akcję. Posiada dwa parametry: `[Pojemność, Koszt]`.

1. **Produkcja (S $\rightarrow$ Fabryka[i])**
    * *Pojemność:* Limit produkcji w danym tygodniu.
    * *Koszt:* Dodatni (koszt wytworzenia 1 sztuki).
    * *Logika:* Nowe telewizory "pojawiają się" na taśmie produkcyjnej.

2. **Magazynowanie / Podróż w czasie (Fabryka[i] $\rightarrow$ Fabryka[i+1])**
    * *Pojemność:* Maksymalna pojemność magazynu.
    * *Koszt:* Dodatni (koszt przechowania 1 sztuki przez tydzień).
    * *Logika:* Rury magazynowe prowadzą **tylko do przodu**. Dzięki temu zapobiegamy pętli czasu (nie da się sprzedać w tyg. 1 telewizora wyprodukowanego w tyg. 3). Zgodnie z Prawem Zachowania Przepływu, stare telewizory zajmują pojemność rury, blokując miejsce na półce.

3. **Sprzedaż (Fabryka[i] $\rightarrow$ Klient[i][j])**
    * *Pojemność:* Zapotrzebowanie danego klienta.
    * *Koszt:* **Ujemny** (-cena skupu).
    * *Logika:* Ujemna waga przyciąga algorytm, zmuszając go do wyboru tej drogi (bo minimalizuje to całkowity koszt grafu).

4. **Zakończenie transakcji (Klient[i][j] $\rightarrow$ T)**
    * *Pojemność:* Zapotrzebowanie klienta.
    * *Koszt:* 0.
    * *Logika:* Zamknięcie przepływu w sieci.

---

## 4. Przykładowa Wizualizacja Grafu (Dla 2 tygodni, 1 klienta)

```text
       [Produkcja 1]          [Sprzedaż 1] 
     +----------------> F_1 ----------------> K_1_1 
     |                   |                      |
    (S)         [Magazyn]|                      | [Odbiór]
     |                   v                      v
     +----------------> F_2 ----------------> K_2_1 -----> (T)
       [Produkcja 2]          [Sprzedaż 2]

5. Odczytywanie Planu
Po zakończeniu działania algorytmu wyciągamy wyniki z sieci rezydualnej (grafu przepływów):

Całkowita produkcja: Wartość maksymalnego przepływu.

Całkowity zysk: -(Ostateczny Koszt) (odwracamy znak z ujemnego).

Produkcja w tygodniu i: Prąd na krawędzi S→Fabryka[i].

Stan magazynu na kolejny tydzień: Prąd na krawędzi Fabryka[i]→Fabryka[i+1].

Sprzedaż dla klienta j: Prąd na krawędzi Fabryka[i]→Klient[i][j].