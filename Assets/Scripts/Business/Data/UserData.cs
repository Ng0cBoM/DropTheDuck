using System.Collections.Generic;
using System;

[Serializable]
public class UserData
{
    public string Name;
    public int level;
    public int HighestScore = 0;
    public int Coin = 0;
    public List<int> ListCharacterId = new List<int>();
}