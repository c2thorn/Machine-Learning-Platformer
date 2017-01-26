using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MonoBehaviour {
    public ScoreKeep scoreKeep;


    public virtual void grabCoin(string coinName)
    {
        scoreKeep.score += 50;
    }

    public virtual void grabWin()
    {
        scoreKeep.score += 500;
    }

    public abstract void LevelEnd();
}
