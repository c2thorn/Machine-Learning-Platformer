using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MonoBehaviour {
    public ScoreKeep scoreKeep;


    public virtual void grabCoin()
    {
        scoreKeep.score += 50;
    }

    public abstract void LevelEnd();
}
