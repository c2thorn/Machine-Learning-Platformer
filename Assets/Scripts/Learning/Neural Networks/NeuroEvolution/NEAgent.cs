using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/**
 * NeuroEvolutionary Agent
 * Currently swapped out for MultiNetAgent, but will do more experimentation with it
 */ 
public class NEAgent : NeuralNetAgent
{
    public Evolution ev = new Evolution("");

    protected void Start()
    {
        if (loadPath.Trim().Length > 0)
        {
            Debug.Log("LOADING " + loadPath);
            ev = new Evolution(loadPath);
            timeKeep.forceView = true;
            DetermineViewing();
        }
    }

    protected override void DetermineViewing()
    {
        if (timeKeep.GetViewing())
        {
            net = ev.getBestNet();
        }
        else
        {
            net = ev.getCurrentNet();
        }
    }

    protected override void setLearningText()
    {
        if (timeKeep.GetViewing())
        {
            learningText.text = "Viewing Elite";
            learningText.fontSize = 16;
            learningText2.text = "";
            learningText3.text = "";
        }
        else
        {
            String text1 = "";
            String text2 = "";
            String text3 = "";
            learningText.fontSize = 10;
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
            learningText3.text = text3;
        }
    }

    protected override void submitScore()
    {
        ev.submitScore(score);
    }
}