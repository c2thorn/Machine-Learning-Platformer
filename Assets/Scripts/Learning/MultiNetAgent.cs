using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/**
 * Agent that uses one neural net per coin/scenario.
 * Scenarios are stored and managed in Multinet Manager
 * 
 */ 
public class MultiNetAgent : NeuralNetAgent
{
    public MultiNetManager manager;
    private int viewingIndex = -1;
    private string coinName = "";
    private bool doNotTickOnce = false;

    protected override void Awake()
    {
        timeKeep = GameObject.Find("Time Text").GetComponent<TimeKeep>();
        SetManager();
        base.Awake();
    }

    protected virtual void SetManager()
    {
        manager = new MultiNetManager(this, timeKeep);
    }

    protected override void BeginLevel()
    {
        manager.BeginLevel();
        viewingIndex = -1;
        coinName = "";
        doNotTickOnce = false;

        InitialSettings();
        DetermineViewing();
        setLearningText();
    }

    protected override void DetermineViewing()
    {
        if (loadPath.Length > 0)
        {
            manager.LoadNets(loadPath);
            timeKeep.forceView = true;
            timeKeep.BeginLevel();
            timeKeep.forceView = true;
        }

        if (timeKeep.GetViewing())
        {
            viewingIndex = 0;
            if (manager.BestCount() > 0)
            {
                net = manager.GetBestNet(0);
            }
            else
                net = new NeuralNet();
        }
        else
        {
            net = new NeuralNet();
            if (manager.netIndex > 0)
            {
                setScenario();
            }
        }
    }

    protected override void FixedUpdate()
    {
        if (!doNotTickOnce && !stopTick && tickDone)
            tick();
        else
            doNotTickOnce = false;
    }

    protected override void setLearningText()
    {
        learningText.fontSize = 16;
        learningText.text = "";
        learningText2.text = "";
        learningText3.text = "";
        String[] line = { "", "", "", "" };

        if (timeKeep.GetViewing())
        {
            line[0] = "Viewing Elite Nets";
            for (int i = 0; i < manager.BestCount(); i++)
            {
                if ((i == viewingIndex))
                {
                    line[1] += "<color=#ff0000>" + manager.GetBestCoinName(i) + "</color>\t\t";
                    line[2] += "<color=#ff0000>" + (manager.GetBestFitness(i) > 0 ? "  "  : " ") +
                        manager.GetBestFitness(i).ToString("F2") + "</color>\t\t\t";
                }
                else
                {
                    line[1] += manager.GetBestCoinName(i) + "\t\t";
                    line[2] += "  " + manager.GetBestFitness(i).ToString("F2") + "\t\t\t";
                }
            }
        }
        else
        {
            for (int i = 0; i <= manager.Count(); i++)
            {
                if ((i == manager.netIndex))
                {
                    line[1] += "<color=#ff0000>" + manager.GetCoinName(i) + "</color>\t\t";
                    line[2] += "<color=#ff0000>" + (manager.GetBestFitness(i) > 0 ? "  " : " ") +
                        manager.GetBestFitness(i).ToString("F2") + "</color>\t\t\t";
                    line[3] += "<color=#ff0000>" + manager.evaluations + "/" + manager.GetMaxEvaluations() + "</color>\t\t";
                }
                else
                {
                    line[1] += manager.GetCoinName(i) + "\t\t";
                    line[2] += "  " + manager.GetFitness(i).ToString("F2") + "\t\t\t";
                    line[3] += "            \t\t";
                }
            }

        }

        for (int i = 0; i < 4; i++)
        {
            learningText.text += line[i] + "\n";
        }
    }

    public void setScenario()
    {
        Scenario scenario = manager.GetScenario(manager.netIndex - 1);
        transform.position = scenario.position;
        velocityX = scenario.velocityX;
        velocityY = scenario.velocityY;
        facingRight = scenario.facingRight;
        jump = scenario.jump;
        grounded = scenario.grounded;
        manager.DestroyCoins();
    }

    public override void grabCoin(string coinName)
    {
        if (timeKeep.GetViewing())
            base.grabCoin(coinName);
        this.coinName = coinName;
        if (!timeKeep.GetViewing())
        {
            stopTick = true;
            LevelEnd();
        }
        else
        {
            if (viewingIndex < (manager.BestCount() - 1))
            {
                //If we are in view mode, and we need to transition to the next neural net

                if (loadPath.Length == 0)
                    manager.LogScenario(OutputCurrentScenario(), viewingIndex);

                viewingIndex += 1;
                net = manager.GetBestNet(viewingIndex);
                doNotTickOnce = true;
                setLearningText();
            }
        }
    }

    //Log all scenario data for debug purposes
    protected string OutputCurrentScenario()
    {
        return coinName + ",  " + transform.position + ", " + velocityX + ", " + velocityY + ", " + facingRight + ", " + jump + ", " + grounded;
    }

    protected override void submitScore()
    {
        if (coinName.Length > 0)
        {
            Scenario scenario = new Scenario();
            scenario.net = net;
            scenario.coinName = coinName;
            scenario.fitness = score;
            scenario.position = transform.position;
            scenario.velocityX = velocityX;
            scenario.velocityY = velocityY;
            scenario.facingRight = facingRight;
            scenario.jump = jump;
            scenario.grounded = grounded;

            stopTick = true;
            manager.SubmitNetScore(score, scenario);
        }
    }

    public override void grabWin()
    {
        base.grabWin();
        if (!timeKeep.GetViewing())
            coinName = "WinTrigger";
    }

    public override void LevelEnd()
    {
        if (timeKeep.GetViewing())
        {
            //If the level ended with out getting to the end of the neural net list,
            //meaning the replay has failed...
            if (viewingIndex != (manager.BestCount() - 1))
            {
                if (loadPath.Length == 0)
                {
                    manager.OutputLog();
                    manager.WriteNets("" + manager.GetBestScore());
                }
                Debug.Log(timeKeep.getRestart());
            }
            if (loadPath.Length == 0)
                manager.ClearLog();
        }
        base.LevelEnd();
    }
}