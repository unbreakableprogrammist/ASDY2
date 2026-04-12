#include <iostream>
using namespace std;

int ktora_spojna[1000];
vector<int> graph[1000];


void Spojne_DFS(int v, int ktora){
    ktora_spojna[v] = ktora;
    for(int i=0; i<graph[v].size(); i++){
        if(ktora_spojna[graph[v][i]] == -1){
            Spojne_DFS(graph[v][i], ktora);
        }
    }

}

int main() {
    for(int i=0; i<1000; i++){
        ktora_spojna[i] = -1;
    }
    int n;
    cin >> n;
    for(int i=0; i<n; i++){
        int a, b;
        cin >> a >> b;
        graph[a].push_back(b);
        graph[b].push_back(a);
    }
    int licz = 0;
    for(int i=0; i<1000; i++){
        if(ktora_spojna[i] == -1){
            Spojne_DFS(i, licz);
            licz++;
        }
    }
    return 0;
}