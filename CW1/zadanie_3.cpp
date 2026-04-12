#include<iostream>
using namespace std;


int main()
{
    int n,s;
    cin >> n >> s;
    int dl[n+1];
    for(int i = 1; i <= n; i++)
    {
        cin >> dl[i];
    }
    vector<int> dp(n+1,INT_MAX);
    dp[0] = 0;
    for(int i=1; i<=n; i++)
    {
        int zsumowana_dl = 0;
        for(int j=0; j<i; j++)
        {
            zsumowana_dl += dl[i-j];
            if(zsumowana_dl <= s)
            {
                int koszt = s - zsumowana_dl;
                dp[i] = min(dp[i], dp[i-j] + koszt*koszt);
            }   
        }
    }
    cout << dp[n] << endl;
    return 0;
}