#include <iostream>
using namespace std;
#define MAX 100

bool odwiedzone[MAX];
vector<int> graf[MAX]; // tablica list sadziedztwa 

void dfs(int v){
    odwiedzone[v] = true;
    for(int i=0;i<graf[v].size();i++){ 
        int sasiad = graf[v][i];
        if(!odwiedzone[sasiad]){
            dfs(sasiad);
        }
    }
    // ladniejsza implementacja w nowym cpp 
    for(int u : graf[v]){
        if(!odwiedzone[u]){
            dfs(u);
        }
    }
}

int main() {
    dfs(0); // uruchamiamy DFS z wierzchołka 0
    // uwaga jesli graf niespojny to trzeba to napisac tak : 
    int n = 20; // liczba wierzchołków
    for(int i=0;i<n;i++){
        if(!odwiedzone[i]){
            dfs(i);
        }
    }
    return 0;
}