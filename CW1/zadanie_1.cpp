#include <iostream>
using namespace std;
#define N 17
#define rozm 10000
int S[rozm]; 

int dp[N+7]; // jest domyslnie wypelnione zerami
int main(){
    int n;
    cin>>n;
    for(int i=0;i<n;i++){
        cin>>S[i];
    }
    dp[0] = 1; // jest jeden sposób, aby uzyskać sumę 0 - nie wybierając żadnej liczby
    for(int i=0;i<n;i++){ // idziemy po wszystkich liczbach w S
        int x = S[i];
        for(int j = N;j>=x;j--){
            if(dp[j-x] == 1){ // jeśli istnieje sposób, aby uzyskać sumę j-x
                dp[j] = 1; // to istnieje sposób, aby uzyskać sumę j - wybierając x
            }
        }

    }
    cout<<dp[n-1]<<endl; // sprawdzamy, czy istnieje sposób, aby uzyskać sumę n-1
}