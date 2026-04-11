#include <iostream>
using namespace std;

#define MAX 100
vector<vector<int>> macierz_odl(MAX, vector<int>(MAX)); // tablica list sadziedztwa

void FloydWarshall(int n){
    for(int k=0;k<n;k++){
        for(int i=0;i<n;i++){
            for(int j=0;j<n;j++){
                if(macierz_odl[i][j] > macierz_odl[i][k] + macierz_odl[k][j]){
                    macierz_odl[i][j] = macierz_odl[i][k] + macierz_odl[k][j];
                }
            }
        }
    }
}

int main() {
    for(int i=0;i<MAX;i++){
        for(int j=0;j<MAX;j++){
            if(i==j) macierz_odl[i][j] = 0;
            else macierz_odl[i][j] = 1e9; // nieskonczonosc
        }
    }
    FloydWarshall(MAX);
    return 0;
}