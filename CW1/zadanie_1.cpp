#include <iostream>
using namespace std;
#define N 1000
#define rozm 10
int S[rozm]; // załóżmy, że S jest wypelnione juz liczbami 

int dp[N+7]; // jest domyslnie wypelnione zerami
int main(){
    dp[0] = 1; // jest jeden sposób, aby uzyskać sumę 0 - nie wybierając żadnej liczby
    for(int i=0;i<rozm;i++){ // idziemy po wszystkich liczbach w S
        int x = S[i];
        for(int j = N;j>=x;j--){
            if(dp[j-x] == 1){ // jeśli istnieje sposób, aby uzyskać sumę j-x
                dp[j] = 1; // to istnieje sposób, aby uzyskać sumę j - wybierając x
            }
        }

    }
    cout<<dp[N];
}