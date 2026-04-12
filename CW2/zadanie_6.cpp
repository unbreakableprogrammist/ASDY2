#include <iostream>
#include <vector>
using namespace std;

bool visted[1000];
vector<int> graph[1000];
vector<int> post_order;

void DFS_post_order(int v, int ktora){
    visted[v] = true;
    for(int i=0; i<graph[v].size(); i++){
        if(!visted[graph[v][i]]){
            DFS_post_order(graph[v][i], ktora);
        }
    }
    post_order.push_back(v);
}

int main() {
    for(int i=0; i<1000; i++){
        visted[i] = false;
    }
    int n;
    cin >> n;
    for(int i=0; i<n; i++){
        int a, b;
        cin >> a >> b;
        graph[a].push_back(b);
        graph[b].push_back(a);
    }
    for(int i=0; i<1000; i++){
        if(!visted[i]){
            DFS_post_order(i, 0);
        }
    }
    post_order.reverse(post_order.begin(), post_order.end());
     for(int i=0; i<post_order.size(); i++){
        cout << post_order[i] << " ";
    }
    return 0;
}