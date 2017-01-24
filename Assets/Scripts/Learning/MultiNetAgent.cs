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
                transform.position = manager.getPreviousCoinPosition();
            }
        }
    }

    public override void grabCoin(string coinName)
    {
        base.grabCoin(coinName);
        this.coinName = coinName;
        if (!viewing)
        {
            manager.gotCoin(coinName);
            LevelEnd();
        }
        else
        {
            if (viewingIndex < manager.netList.Count)
                net = (NeuralNet)manager.netList[viewingIndex++];
        }
    }

    protected override void submitScore()
    {
        if (manager.fitList.Count == 0 && coinName.Length > 0)
        {
            manager.addNewCoin(score, ev.getCurrentNet(), coinName);
            ev.cullPopulation();
        }
        else
        {
            ev.submitScore(score);
        }
    }
}