using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/**
 * NeuroEvolutionary Agent
 * Currently swapped out for MultiNetAgent, but will do more experimentation with it
 */
public class SimNEAgent : NeuralNetAgent
{
    public int GenerationID;
    public SimEvolution ev;
    public bool terminated = false;
    public int maxTime = 200;
    public int timeLeft;
    public float farthestX = -1000f;
    public TextMesh text;

    protected override void Awake()
    {
        text = GetComponentInChildren<TextMesh>();
        text.text = "" + maxTime;
        //text.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z-1);
    }

    protected void Start()
    {
        /*
        if (loadPath.Trim().Length > 0)
        {
            Debug.Log("LOADING " + loadPath);
            ev = new Evolution(loadPath);
            timeKeep.forceView = true;
            DetermineViewing();
        */
        anim = GetComponentInChildren<Animator>();
        groundCheck = transform.Find("groundCheck");

        beginningPosition = transform.position;
        timeKeep = GameObject.Find("Time Text").GetComponent<TimeKeep>();
        timeKeep.ChangeTimeScale(1f);
        timeKeep.ChangeTimeOut(20f);
        timeKeep.canView = false;
        BeginLevel();
    }

    public override void tick()
    {
        base.tick();
        if (transform.position.x > farthestX)
        {
            farthestX = transform.position.x;
            timeLeft = maxTime;
        }
        else
        {
            timeLeft--;
            text.text = "" + timeLeft;
            if (timeLeft < 1)
            {
                LevelEnd();
            }
        }
    }

    protected override void DetermineViewing()
    {
        timeLeft = maxTime;
        SetTerminated(false);
        farthestX = -1000f;
        net = ev.getAgentNet(GenerationID);
    }

    protected override void setLearningText()
    {
    }

    public virtual bool TimeOut()
    {
        score += transform.position.x;
        stopTick = true;
        SetTerminated(true);
        return ev.submitScore(GenerationID, score);
    }

    public override void LevelEnd()
    {
        score += transform.position.x;
        stopTick = true;
        SetTerminated(true);
        submitScore();
    }

    protected override void submitScore()
    {
        ev.submitScore(GenerationID, score);
    }

    protected virtual void SetTerminated(bool term)
    {
        //Debug.Log(GenerationID + "setting terminated to " + term);
        terminated = term;
        SpriteRenderer sprite = GetComponentInChildren<SpriteRenderer>();
        Color newColor = sprite.color;
        if (term)
            newColor = new Color(newColor.r, newColor.g, newColor.b, 0.5f);
        else
            newColor = new Color(newColor.r, newColor.g, newColor.b, 1f);
        sprite.color = newColor;
    }

    protected override void highlightActions()
    {
    }

    public override void LevelRestart()
    {
        timeKeep.BeginLevel();
        BeginLevel();
    }
}