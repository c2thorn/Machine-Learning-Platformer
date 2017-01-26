using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MultiNetAgent : NEAgent
{
    private static MultiNetManager manager = new MultiNetManager();

    private int viewingIndex = 0;
    private string coinName = "";
    private bool doNotTickOnce = false;

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
        if (!doNotTickOnce)
            tick();
        else
            doNotTickOnce = false;

        if (Input.GetKey("v"))
            LevelEnd();
    }

    protected override void setLearningText()
    {
        if (viewing)
        {
            learningText.fontSize = 16;
            learningText.text = "";
            learningText2.text = "";
            learningText3.text = "";
            String[] line = { "Viewing Elite Nets", "", "", "" };

            for (int i = 0; i < manager.netsOptimized; i++)
            {
                if (i == viewingIndex)
                {
                    line[1] += "<color=#ff0000>" + manager.coinList[i] + "</color>\t\t";
                    line[2] += "<color=#ff0000>" + manager.fitList[i] + "</color>\t\t\t\t";
                }
                else
                {
                    line[1] += manager.coinList[i] + "\t\t";
                    line[2] += manager.fitList[i] + "\t\t\t\t";
                }
            }

            for (int i = 0; i < 4; i++)
            {
                learningText.text += line[i] + "\n";
            }
        }
        else
        {
            /*String text1 = "";
            String text2 = "";
            String text3 = "";
            int j = ev.popSize / 3;
            int k = j * 2;
            for (int i = 0; i < j; i++)
            {
                String line = "";
                if (i == ev.popIndex)
                    line += "<color=#ff0000>[ " + i + " " + ev.fitness[i] + " ]</color>\n";
                else
                    line += i + " " + ev.fitness[i] + "\n";

                text1 += line;
            }
            for (int i = j; i < k; i++)
            {
                String line = "";
                if (i == ev.popIndex)
                    line += "<color=#ff0000>[ " + i + " " + ev.fitness[i] + " ]</color>\n";
                else
                    line += i + " " + ev.fitness[i] + "\n";

                text2 += line;
            }
            for (int i = k; i < ev.popSize; i++)
            {
                String line = "";
                if (i == ev.popIndex)
                    line += "<color=#ff0000>[ " + i + " " + ev.fitness[i] + " ]</color>\n";
                else
                    line += i + " " + ev.fitness[i] + "\n";

                text3 += line;
            }
            learningText.text = text1;
            learningText2.text = text2;
            learningText3.text = text3;*/
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
        if (viewing)
            base.grabCoin(coinName);
        this.coinName = coinName;
        if (!viewing)
        {
            LevelEnd();
        }
        else
        {
            if (viewingIndex < (manager.netList.Count-1))
            {
                net = (NeuralNet)manager.netList[++viewingIndex];
                doNotTickOnce = true;
                setLearningText();
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
                manager.addNewCoin(score, net, coinName, scenario);
                //ev.cullPopulation();
            }
            else if (manager.netIndex < manager.coinList.Count) { 
                if (coinName.Equals(manager.coinList[manager.netIndex]))
                {
                    if ((float)manager.fitList[manager.netIndex] < score)
                    {
                        Debug.Log("Replacing! " + (float)manager.fitList[manager.netIndex] + " " + score);
                        manager.replaceCoin(score, net, coinName, scenario);
                        //ev.cullPopulation();
                    }
                    else
                    {
                        //ev.submitScore(score);
                    }
                }
            }
            else
            {
                //ev.submitScore(score);
            }
            if (coinName.Equals("WinTrigger"))
            {
                manager.netIndex = 0;
            }
        }
        else
        {
            //ev.submitScore(score);
        }
    }

    public override void grabWin()
    {
        base.grabWin();
        if (!viewing)
        {
            coinName = "WinTrigger";
            manager.won = true;
        }
    }
}