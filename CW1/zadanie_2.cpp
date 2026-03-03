#include<iostream>
#include<string>
#include<algorithm> // potrzebne dla max()
using namespace std;

#define MAX 105 
int dp[MAX][MAX]; // domyślnie wypełnione 0

int main(){
    string slowo1, slowo2;
    cin >> slowo1 >> slowo2;
    
    int n = slowo1.length();
    int m = slowo2.length();
    
    // Przesuwamy indeksowanie DP o 1. 
    // dp[i][j] oznacza wynik dla i pierwszych liter slowo1 i j pierwszych liter slowo2.
    for(int i = 1; i <= n; i++){
        for(int j = 1; j <= m; j++){
            // Uwaga: indeksy w stringu zostają od 0, stąd i-1 oraz j-1
            if(slowo1[i-1] == slowo2[j-1]){
                dp[i][j] = dp[i-1][j-1] + 1; // dodajemy 1 do wyniku bez obu tych liter
            } else {
                dp[i][j] = max(dp[i-1][j], dp[i][j-1]); // bierzemy max z góry lub z lewej
            }
        }
    }
    
    // Wynik znajduje się w prawym dolnym rogu tablicy
    cout << dp[n][m] << endl;
    return 0;
}