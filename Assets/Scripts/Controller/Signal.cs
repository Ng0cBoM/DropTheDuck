using EgdFoundation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateScore : Signal
{
    public int score;

    public UpdateScore(int score)
    {
        this.score = score;
    }
}

public class UpdateCoin : Signal
{
}

public class SpawnNewBlock : Signal
{
}

public class ContinuePlay : Signal
{
}