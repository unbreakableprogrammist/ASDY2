#include <iostream>
using namespace std;

bool visited[100];
vector<int> graph[100];

void Stack_DFS(int v){
    stack<int> s;
    s.push(v);
    while(!s.empty()){
        int u = s.top();
        s.pop();
        if(!visited[u]){
            visited[u] = true;
            cout << u << " ";
            for(int i = 0; i < graph[u].size(); i++){
                int w = graph[u][i];
                if(!visited[w]){
                    s.push(w);
                }
            }
        }
    }
}

int main() {
    cout << "Hello, World!" << endl;
    return 0;
}
