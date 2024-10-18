
using UnityEngine;

public struct ScoreDataEvent
{
    public float score;
    public float highScore;

    public ScoreDataEvent(float score, float highScore)
    {
        this.score = score;
        this.highScore = highScore;
    }
}

public struct ComboEvent
{
    public float count;
    public Vector2 pos;

    public ComboEvent(float count, Vector2 pos)
    {
        this.count = count;
        this.pos = pos;
    }
}


