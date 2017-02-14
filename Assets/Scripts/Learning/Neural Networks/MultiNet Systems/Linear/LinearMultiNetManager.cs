using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearMultiNetManager : MultiNetManager
{
    private bool won = false;
    private bool foundSomething = false;

    public LinearMultiNetManager(MultiNetAgent agent, TimeKeep timeKeep) : base(agent, timeKeep)
    {
        this.agent = agent;
        this.timeKeep = timeKeep;
        maxEvaluations = 500;
    }

    public override void BeginLevel()
    {
        IncrementEvaluations();
    }
    
    /**
    * If there are no entries, add the new scenario.
    * Otherwise, replace the current entry with the new scenario and delete the rest.
    */
    public override void UpdateList(Scenario scenario)
    {
        List<Scenario> newList = new List<Scenario>();
        for (int i = 0; i < netIndex; i++)
        {
            newList.Add(scenarioList[i].Copy());
        }

        newList.Add(scenario);
        scenarioList = newList;

        if (scenario.coinName.Equals("WinTrigger"))
            won = true;
        else
            won = false;

        if (bestList.Count == 0)
            evaluations = maxEvaluations;
    }

    public override void SubmitNetScore(float score, Scenario scenario)
    {
        base.SubmitNetScore(score, scenario);
        foundSomething = true;
    }

    private void IncrementEvaluations()
    {
        if (evaluations >= maxEvaluations)
        {
            if (foundSomething)
            {
                if (won)
                {
                    if (CompareListScore(bestList, scenarioList))
                    {
                        bestList = DeepCopy(scenarioList);
                        WriteNets("" + GetBestScore());
                    }
                    else
                        maxEvaluations = maxEvaluations + 500;
                    timeKeep.forceView = true;
                    scenarioList = new List<Scenario>();
                    netIndex = 0;
                    won = false;
                }
                else
                {
                    //Debug.Log("Continuing to net index: " + netIndex + ".");
                    netIndex++;
                }
            }
            else
            {
                Debug.Log("Max Evals on net index: " + netIndex + ". Restarting completely.");
                scenarioList = new List<Scenario>();
                netIndex = 0;
                won = false;
                maxEvaluations = maxEvaluations + 500;
            }
            foundSomething = false;
            evaluations = 0;
        }
        else
        {
            evaluations++;
        }
    }
}
