#include<iostream>
using namespace std;


int main()
{
    int n,s;
    int dl[n];
    cin >> n >> s;
    for(int i = 0; i < n; i++)
    {
        cin >> dl[i];
    }
    vector<int> dp(n,INT_MAX);
    dp[0] = 0;
    for(int
         i=1; i<n; i++)
    {
        int zsumowana_dl = dl[i];
        for(int j=0; j<i; j++)
        {
            if(zsumowana_dl <= s)
            {
                int koszt = s - zsumowana_dl;
                dp[i] = min(dp[i], dp[i-j] + koszt*koszt);
            }
            zsumowana_dl += dl[i-j];
        }
    }
    cout << dp[n-1] << endl;
    return 0;
}