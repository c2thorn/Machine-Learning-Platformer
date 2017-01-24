using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MultiNetAgent : NEAgent
{
    private static MultiNetManager manager = new MultiNetManager();

    private int viewingIndex = 0;
    private string coinName = "";

    protected override void Awake()
    {
        anim = GetComponent<Animator>();
        restart = timeKeep.getRestart();
        manager.setAgent(this);
        if (restart % timeKeep.viewNumber == 0 && restart > 0)
        {
            Debug.Log("Viewing!");
            viewingIndex = 0;
            viewing = true;
            if (manager.netList.Count > 0)
                net = (NeuralNet)manager.netList[0];
            else
                net = ev.getBestNet();
        }
        else
        {
            viewing = false;
            net = ev.getCurrentNet();
            if (manager.netIndex > 0)
            {
                setScenario();
            }
        }
    }

    public void setScenario()
    {
        //Debug.Log("not viewing!");
        ArrayList scenario = (ArrayList)manager.scenarioList[manager.netIndex - 1];
        transform.position = (Vector3)scenario[0];
        velocityX = (float)scenario[1];
        velocityY = (float)scenario[2];
        facingRight = (bool)scenario[3];
        jump = (bool)scenario[4];
        grounded = (bool)scenario[5];
        manager.destroyCoins();
    }

    public override void grabCoin(string coinName)
    {
        base.grabCoin(coinName);
        this.coinName = coinName;
        if (!viewing)
        {
            //manager.gotCoin(coinName);

            LevelEnd();
        }
        else
        {
            if (viewingIndex < (manager.netList.Count-1))
            {
                net = (NeuralNet)manager.netList[++viewingIndex];
                Debug.Log("Changing!! " + viewingIndex);
            }
        }
    }

    protected override void submitScore()
    {
        if (coinName.Length > 0)
        {
            ArrayList scenario = new ArrayList();
            scenario.Add(transform.position);
            scenario.Add(velocityX);
            scenario.Add(velocityY);
            scenario.Add(facingRight);
            scenario.Add(jump);
            scenario.Add(grounded);
            if (!manager.coinList.Contains(coinName))
            {
                manager.addNewCoin(score, ev.getCurrentNet(), coinName, scenario);
                ev.cullPopulation();
            }
            else if (coinName.Equals(manager.coinList[manager.netIndex - 1]))
            {
                if ((float)manager.fitList[manager.netIndex-1] < score)
                {
                    manager.replaceCoin(score, ev.getCurrentNet(), coinName, scenario);
                    ev.cullPopulation();
                }
            }
            else
            {
                ev.submitScore(score);
            }
        }
        else
        {
            ev.submitScore(score);
        }
    }
}