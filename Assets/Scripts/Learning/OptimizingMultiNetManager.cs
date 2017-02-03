using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptimizingMultiNetManager : MultiNetManager
{
    private int maxDiscoveringEvaluations = 1500;
    private int maxOptimizingEvaluations = 500;

    private bool optimizing = false;
    private int lastOptimalIndex = -1;
    private int optimizingLoopCounter = 0;
    private int maxOptimizingAttempts = 5;

    public OptimizingMultiNetManager(MultiNetAgent agent, TimeKeep timeKeep) : base(agent, timeKeep)
    {
        this.agent = agent;
        this.timeKeep = timeKeep;
    }

    public override void BeginLevel()
    {
        if (optimizing)
        {
            if (evaluations >= maxOptimizingEvaluations)
            {
                if (netIndex >= (bestList.Count - 1))
                {
                    Debug.Log("Optimization complete with max optimizing evaluations at " + maxOptimizingEvaluations + ". Restarting with netIndex = 0.");
                    netIndex = 0;
                }
                else
                {
                    Debug.Log("Max Optimizing Evals on net index: " + netIndex + " with score " + getFitness(netIndex) + ". Continuing to next.");
                    netIndex++;
                }

                evaluations = 0;

            }
            else
                evaluations++;
        }
        else
        {
            if (evaluations >= maxDiscoveringEvaluations)
            {
                Debug.Log("Max Discovering Evals on net index: " + netIndex + ". Restarting from bestList.");
                scenarioList = deepCopy(bestList);
                netIndex = 0;
                evaluations = 0;
                lastOptimalIndex = -1;
                optimizingLoopCounter = 0;

                //The case where this wouldn't be true is when the agent has failed to discover a new coin on its first run.
                if (bestList.Count > 0)
                    optimizing = true;
            }
            else
                evaluations++;
        }
    }

    /**
    * If there are no entries, add the new scenario.
    * Otherwise, replace the current entry with the new scenario and delete the rest.
    */
    public override void updateList(ArrayList scenario)
    {
        ArrayList newList = new ArrayList();
        for (int i = 0; i < netIndex; i++)
        {
            newList.Add(scenarioCopy((ArrayList)scenarioList[i]));
        }

        newList.Add(scenario);

        if (optimizing)
            lastOptimalIndex = netIndex;

        if (((string)scenario[1]).Equals("WinTrigger"))
        {

            if (compareListScore(bestList, newList))
            {
                bestList = deepCopy(newList);
                lastOptimalIndex = -1;
            }
            if (lastOptimalIndex >= 0 && optimizingLoopCounter < maxOptimizingAttempts)
            {
                ArrayList optimalList = new ArrayList();

                for (int i = 0; i <= lastOptimalIndex; i++)
                    optimalList.Add(scenarioCopy((ArrayList)scenarioList[i]));
                scenarioList = deepCopy(optimalList);
                netIndex = lastOptimalIndex + 1;
                Debug.Log("Retrying from: " + lastOptimalIndex + ". Attempt: " + optimizingLoopCounter);
                optimizingLoopCounter++;
                optimizing = false;
            }
            else
            {
                netIndex = 0;
                scenarioList = deepCopy(bestList);
                optimizingLoopCounter = 0;
                optimizing = true;
            }

        }
        else
        {
            scenarioList = deepCopy(newList);
            optimizing = false;
            netIndex++;
        }
        evaluations = 0;
    }

    public override float getMaxEvaluations()
    {
        return optimizing ? maxOptimizingEvaluations : maxDiscoveringEvaluations;
    }
}
