#include <iostream>
using namespace std;
#define MAX 100

int n; // liczba wierzchołków
int odleglosci[MAX];
vector<pair<int,int>> graf[MAX]; // tablica list sadziedztwa

void BellmanFord(int start, int dest){
    for(int i=0;i<n;i++){
        odleglosci[i] = INT_MAX;
    }
    odleglosci[start] = 0;
    for(int i=1;i<n;i++){
        for(int v = 0;v<n;v++){
            for(auto edge : graf[v]){
                int u = edge.first;
                int weight = edge.second;
                if(odleglosci[v] != INT_MAX && odleglosci[v] + weight < odleglosci[u]){ // jesli w jakims wierzcholku jest nie nieskonczonosc to probujemy atkualizowac odleglosci do sasiednich wierzcholkow
                    odleglosci[u] = odleglosci[v] + weight;
                }
            }
        }
    }
}

int main() {
    BellmanFord(0, n-1); // uruchamiamy Bellman-Ford z wierzchołka 0 do n-1
    return 0;
}