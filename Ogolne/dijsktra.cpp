#include<bits/stdc++.h>
using namespace std;

#define MAX 100

void Dijsktra(int v, vector<int> &odleglosci, vector<pair<int,int>> graf){
    odleglosci[v] = 0; // odleglosc do samego siebie jest 0
    priority_queue<pair<int,int>, greater<pair<int,int>>> kolejka;
    kolejka.push({0, v}); // {odleglosc, wierzcholek}
    while(!kolejka.empty()){
        int aktualny = kolejka.top().second;
        kolejka.pop();
        for(auto edge : graf[aktualny]){
            int sasiad = edge.first;
            int waga = edge.second;
            if(odleglosci[aktualny] + waga < odleglosci[sasiad]){ // nie musimy sprawdzac tablicy odwiedzonych bo jesli da sie zaktualizowac odleglosc to oznacza ze nie byl odwiedzony 
                odleglosci[sasiad] = odleglosci[aktualny] + waga;
                kolejka.push({odleglosci[sasiad], sasiad});
            }
        }
    }
}



int main(){
    vector<int> odleglosci(MAX, INT_MAX); // wypelniamy nieskonczonosciami
    for(int i=0;i<MAX;i++){
        odleglosci[i] = INT_MAX;
    }
    Dijsktra(0, odleglosci, graf); // uruchamiamy Dijsktra z wierzchołka 0
    return 0;
}