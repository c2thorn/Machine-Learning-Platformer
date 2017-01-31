using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MultiNetAgent : NEAgent
{
    private MultiNetManager manager;


    private int viewingIndex = -1;
    private string coinName = "";
    private bool doNotTickOnce = false;

    protected override void Awake()
    {
        //manager = new MultiNetManager(this);
        //manager = new OptimizingMultiNetManager(this);
        manager = new LinearMultiNetManager(this);
        base.Awake();
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
        if (restart % timeKeep.viewNumber == 0 && restart > 0)
        {
            //Debug.Log("Viewing!");
            viewingIndex = 0;
            viewing = true;
            if (manager.bestCount() > 0)
                net = manager.getBestNet(0);
            else
                net = new NeuralNet();
        }
        else
        {
            viewing = false;
            net = new NeuralNet();
            if (manager.netIndex > 0)
            {
                setScenario();
            }
        }
    }

    protected override void FixedUpdate()
    {
        if (!doNotTickOnce && !stopTick)
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

        if (viewing)
        {
            line[0] = "Viewing Elite Nets";
            for (int i = 0; i < manager.bestCount(); i++)
            {
                if ((i == viewingIndex))
                {
                    line[1] += "<color=#ff0000>" + manager.getBestCoinName(i) + "</color>\t\t";
                    line[2] += "<color=#ff0000>" + manager.getBestFitness(i).ToString("F4") + "</color>\t\t";
                }
                else
                {
                    line[1] += manager.getBestCoinName(i) + "\t\t";
                    line[2] +="" + manager.getBestFitness(i).ToString("F4") + "\t\t";
                }
            }
        }
        else
        {
            for (int i = 0; i <= manager.count(); i++)
            {
                if ((i == manager.netIndex) )
                {
                    line[1] += "<color=#ff0000>" + manager.getCoinName(i) + "</color>\t\t";
                    line[2] += "<color=#ff0000>" + manager.getFitness(i).ToString("F4") + "</color>\t\t";
                    line[3] += "<color=#ff0000>" + manager.evaluations + "/" + manager.getMaxEvaluations() + "</color>\t\t";
                }
                else
                {
                    line[1] += manager.getCoinName(i) + "\t\t";
                    line[2] += "" + manager.getFitness(i).ToString("F4") + "\t\t";
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
        transform.position = manager.getPosition(manager.netIndex - 1);
        //Debug.Log("Setting up!" + manager.getPosition(manager.netIndex - 1).x + " " + manager.getPosition(manager.netIndex - 1).y);
        velocityX = manager.getVelocityX(manager.netIndex - 1);
        velocityY = manager.getVelocityY(manager.netIndex - 1);
        facingRight = manager.getFacingRight(manager.netIndex - 1);
        jump = manager.getJump(manager.netIndex - 1);
        grounded = manager.getGrounded(manager.netIndex - 1);
        manager.destroyCoins();
    }

    public override void grabCoin(string coinName)
    {
        if (viewing)
            base.grabCoin(coinName);
        this.coinName = coinName;
        if (!viewing)
        {
            //OutputScenario();
            LevelEnd();
        }
        else
        {
            if (viewingIndex < (manager.bestCount()-1))
            {
                manager.LogScenario(OutputCurrentScenario(), viewingIndex);
                net = manager.getBestNet(++viewingIndex);
                doNotTickOnce = true;
                setLearningText();
            }
        }
    }

    protected string OutputCurrentScenario()
    {
        return coinName+ ",  " + transform.position + ", " + velocityX + ", " + velocityY + ", " + facingRight + ", " + jump + ", " + grounded;
    }

    protected override void submitScore()
    {
        if (coinName.Length > 0)
        {
            ArrayList scenario = new ArrayList();
            scenario.Add(net);
            scenario.Add(coinName);
            scenario.Add(score);
            scenario.Add(transform.position);
            scenario.Add(velocityX);
            scenario.Add(velocityY);
            scenario.Add(facingRight);
            scenario.Add(jump);
            scenario.Add(grounded);

            stopTick = true;
            manager.submitNetScore(score, scenario);
        }
    }

    public override void grabWin()
    {
        base.grabWin();
        if (!viewing)
            coinName = "WinTrigger";
    }

    public override void LevelEnd()
    {
        if (viewing)
        {
            if (viewingIndex != (manager.bestCount() - 1))
                manager.OutputLog();
            manager.ClearLog();
        }
        base.LevelEnd();
    }
}