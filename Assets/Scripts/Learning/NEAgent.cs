using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NEAgent : Agent
{
    public static Evolution ev = new Evolution();
    protected NeuralNet net;
    public string loadPath = "";

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

    protected override void GetActions()
    {
        double[] inputs = new double[28];
        int which = 0;
        float x = (float)GetNearestEven(gameObject.transform.position.x);
        float y = (float)GetNearestEven(gameObject.transform.position.y);
        for (int i = -2; i < 3; i++)
        {
            for (int j = -2; j < 3; j++)
            {
                inputs[which++] = probe(x, y, i, j);
            }
        }
        inputs[inputs.Length - 3] = grounded ? 1 : 0;
        inputs[inputs.Length - 2] = grounded && velocityY == 0 ? 1 : 0;
        inputs[inputs.Length - 1] = 1;

        double[] outputs = net.propagate(inputs);

        for (int i = 0; i < 3; i++)
            actions[i] = outputs[i] > 0;

        highlightActions();
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