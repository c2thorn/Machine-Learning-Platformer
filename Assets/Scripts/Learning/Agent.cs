using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Agent : Player
{
    public GUIText learningText;
    public GUIText learningText2;
    public GUIText learningText3;
    
    protected float score = 0f;

    protected override void BeginLevel()
    {
        InitialSettings();
        DetermineViewing();
        setLearningText();
    }
    protected override void InitialSettings()
    {
        base.InitialSettings();
        score = 0f;
    }

    public override void LevelEnd()
    {
        float time = timeKeep.time > timeKeep.timeOut ? timeKeep.timeOut : timeKeep.time;
        if (!timeKeep.GetViewing())
        {
            score = scoreKeep.score - time + timeKeep.timeOut;
            submitScore();
        }
        LevelRestart();
    }

    protected abstract void DetermineViewing();

    //Get objects around teh agent
    protected virtual double probe(float x, float y, int i, int j)
    {
        float x2 = x + (2 * i);
        float y2 = y + (2 * j);
        GameObject obj = FindAt(new Vector2(x2, y2));
        if (obj == null)
            return 0;
        if (obj.tag == "Coin")
            return 2;
        if (obj.tag == "Finish")
            return 3;
        return 1;
    }

    // get all colliders that intersect pos
    protected virtual GameObject FindAt(Vector2 pos)
    {
        Collider2D col = Physics2D.OverlapCircle(pos, .5f);
        if (col == null)
            return null;
        return col.gameObject;
    }

    //Draw grid for editor purposes
    protected virtual void OnDrawGizmos()
    {
        float x = (float)GetNearestEven(gameObject.transform.position.x);
        float y = (float)GetNearestEven(gameObject.transform.position.y);
        for (int i = -2; i < 3; i++)
        {
            for (int j = -2; j < 3; j++)
            {
                float x2 = x + (2 * i);
                float y2 = y + (2 * j);
                Gizmos.DrawWireSphere(new Vector3(x2, y2, -1f), .5f);
            }
        }
    }

    //Find nearest even number
    protected virtual double GetNearestEven(double input)
    {
        double output = Math.Round(input / 2);
        if (output == 0 && input > 0) output += 1;
        output *= 2;

        return output;
    }

    protected abstract void setLearningText();

    protected abstract void submitScore();
}