using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/**
 * NeuroEvolutionary Agent
 * Currently swapped out for MultiNetAgent, but will do more experimentation with it
 */
public abstract class NeuralNetAgent : Agent
{
    protected NeuralNet net;
    public string loadPath = "";
    
    protected override void GetActions()
    {
        double[] inputs = new double[29];
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
        inputs[inputs.Length - 4] = wallRiding > 0 ? 1 : 0;
        inputs[inputs.Length - 3] = grounded ? 1 : 0;
        inputs[inputs.Length - 2] = grounded && velocityY == 0 ? 1 : 0;
        inputs[inputs.Length - 1] = 1;

        double[] outputs = net.propagate(inputs);

        for (int i = 0; i < 3; i++)
            actions[i] = outputs[i] > 0;

        highlightActions();
    }
}