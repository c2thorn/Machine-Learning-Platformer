using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MultiNetManager {
    protected List<Scenario> scenarioList = new List<Scenario>();
    protected List<Scenario> bestList = new List<Scenario>();
    protected MultiNetAgent agent;

    protected ArrayList log = new ArrayList();

    public int netIndex = 0;
    public int evaluations = 0;

    protected int maxEvaluations = 1000;
    protected TimeKeep timeKeep;

    public MultiNetManager(MultiNetAgent agent, TimeKeep timeKeep)
    {
        this.agent = agent;
        this.timeKeep = timeKeep;
    }

    public virtual void BeginLevel()
    {
        if (evaluations >= maxEvaluations)
        {
            Debug.Log("Max evaluations on net index: " + netIndex + ". Restarting from bestList.");
            scenarioList = DeepCopy(bestList);
            netIndex = 0;
            evaluations = 0;
        }
        else
            evaluations++;
    }

    public void DestroyCoins()
    {
        for (int i = 0; i < netIndex; i++)
        {
            GameObject coin = GameObject.Find(GetCoinName(i));
            coin.SetActive(false);
        }
    }

    /**
    * If there are no entries, add the new scenario.
    * Otherwise, replace the current entry with the new scenario and delete the rest.
    */
    public virtual void UpdateList(Scenario scenario)
    {
        List<Scenario> newList = new List<Scenario>();
        for (int i = 0; i < netIndex; i++)
        {
            newList.Add(scenarioList[i].Copy());
        }

        newList.Add(scenario);

        if (scenario.coinName.Equals("WinTrigger"))
        {
            if (CompareListScore(bestList, newList))
            {
                bestList = DeepCopy(newList);
            }
            netIndex = 0;
            scenarioList = DeepCopy(bestList);
        }
        else
        {
            scenarioList = DeepCopy(newList);
            netIndex++;
        }
        evaluations = 0;
    }

    public Scenario GetScenario(int index)
    {
        return index == scenarioList.Count ? new Scenario() : scenarioList[index];
    }

    public string GetCoinName(int index)
    {
        if (index == Count())
            return "None";
        return scenarioList[index].coinName;
    }

    public float GetFitness(int index)
    {
        if (index == Count())
            return -1;
        return scenarioList[index].fitness;
    }

    public float Count()
    {
        return scenarioList.Count;
    }

    public NeuralNet GetBestNet(int index)
    {
        return bestList.Count > 0 ? bestList[index].net : scenarioList[index].net;
    }
    
    public string GetBestCoinName(int index)
    {
        if (index == BestCount())
            return "None";
        return bestList.Count > 0 ? bestList[index].coinName : scenarioList[index].coinName;
    }

    public float GetBestFitness(int index)
    {
        if (index == BestCount())
            return -1;
        return bestList.Count > 0 ? bestList[index].fitness : scenarioList[index].fitness;
    }

    public float BestCount()
    {
        return bestList.Count > 0 ? bestList.Count : scenarioList.Count;
    }

    public virtual float GetMaxEvaluations()
    {
        return maxEvaluations;
    }

    public void LogScenario(string input, int index)
    {
        Scenario scenario = bestList.Count > 0 ? bestList[index] : scenarioList[index];

        string input2 = scenario.ToString();

        log.Add(input);
        log.Add(input2);
    }

    public void OutputLog()
    {
        foreach (string entry in log)
            Debug.Log(entry);
    }

    public void ClearLog()
    {
        log = new ArrayList();
    }

    protected List<Scenario> DeepCopy(List<Scenario> original)
    {
        List<Scenario> copy = new List<Scenario>();

        for (int i = 0; i < original.Count; i++)
        {
            copy.Add(original[i].Copy());
        }

        return copy;
    }

    public virtual void SubmitNetScore(float score, Scenario scenario)
    {
        if (score > GetFitness(netIndex))
        {
            UpdateList(scenario);
        }
    }

    protected bool CompareListScore(List<Scenario> incumbent, List<Scenario> challenger)
    {
        float iScore = 0f;
        float cScore = 0f;

        foreach (Scenario entry in incumbent)
            iScore += entry.fitness;

        foreach (Scenario entry in challenger)
            cScore += entry.fitness;

        if (cScore > iScore)
        {
            Debug.Log("New score " + cScore + " has beaten previous score " + iScore + ". @ " + timeKeep.getRestart());
            return true;
        }
        else
        {
            Debug.Log("Old score " + iScore + " remains higher than new score " + cScore + ". @ " + timeKeep.getRestart());
            return false;
        }
    }

    public float GetBestScore()
    {
        float bestScore = 0f;

        foreach (Scenario entry in bestList)
        {
            bestScore += entry.fitness;
        }
        return bestScore;
    }

    public void WriteNets(string directory)
    {
        System.IO.Directory.CreateDirectory(@"NeuralNets\" + directory+"\\");
        for (int i = 0; i < bestList.Count; i++)
        {
            GetBestNet(i).writeNet(directory + "\\" + i);
        }
    }

    public void LoadNets(string loadPath)
    {
        int fCount = Directory.GetFiles(@"NeuralNets\" + loadPath + "\\", "*", SearchOption.TopDirectoryOnly).Length;
        bestList = new List<Scenario>();
        for (int i = 0; i < fCount; i++) {
            Scenario scenario = new Scenario();
            scenario.net = new NeuralNet(loadPath + "\\" + i);
            scenario.coinName = "LOADED";
            scenario.fitness = 11f;
            bestList.Add(scenario);
        }
        //Debug.Log("LOADING " + bestCount());
    }
}
