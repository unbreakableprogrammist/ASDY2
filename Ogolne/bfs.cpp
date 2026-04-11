#include <iostream>
using namespace std;
#define MAX 100

bool odwiedzone[MAX];
vector<int> graf[MAX]; // tablica list sadziedztwa

void bfs(int start){
    queue<int>q;
    odwiedzone[start] = true;
    q.push(start);
    while(!q.empty()){
        int v = q.front();
        q.pop();
        for(auto u:graf[v]){
            if(!odwiedzone[u]){
                odwiedzone[u] = true;
                q.push(u);
            }
        }
    }
}
int main() {
    bfs(0); // uruchamiamy BFS z wierzchołka 0
    // uwaga jesli graf niespojny to trzeba to napisac tak :
    int n = 20; // liczba wierzchołków
    for(int i=0;i<n;i++){
        if(!odwiedzone[i]){
            bfs(i);
        }
    }
    return 0;
}
