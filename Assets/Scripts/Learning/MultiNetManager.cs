using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiNetManager {
    /**
     * Scenario ArrayList
     * 0 Neural Net
     * 1 Coin
     * 2 Fitness
     * 3 Position
     * 4 VelocityX
     * 5 VelocityY
     * 6 FacingRight
     * 7 Jump
     * 8 Grounded
     */
    protected ArrayList scenarioList = new ArrayList();
    protected ArrayList bestList = new ArrayList();
    protected MultiNetAgent agent;

    public int netIndex = 0;
    public int evaluations = 0;

    protected int maxEvaluations = 1000;

    public MultiNetManager(MultiNetAgent agent)
    {
        this.agent = agent; 
    }

    public virtual void BeginLevel()
    {
        if (evaluations >= maxEvaluations)
        {
            Debug.Log("Max evaluations on net index: " + netIndex + ". Restarting from bestList.");
            scenarioList = deepCopy(bestList);
            netIndex = 0;
            evaluations = 0;
        }
        else
            evaluations++;
    }

    public void destroyCoins()
    {
        for (int i = 0; i < netIndex; i++)
        {
            GameObject coin = GameObject.Find(getCoinName(i));
            coin.SetActive(false);
        }
    }

    /**
    * If there are no entries, add the new scenario.
    * Otherwise, replace the current entry with the new scenario and delete the rest.
    */
    public virtual void updateList(ArrayList scenario)
    {
        ArrayList newList = new ArrayList();
        for (int i = 0; i < netIndex; i++)
        {
            newList.Add(scenarioCopy((ArrayList)scenarioList[i]));
        }

        newList.Add(scenario);

        if (((string)scenario[1]).Equals("WinTrigger"))
        {
            if (compareListScore(bestList, newList))
            {
                bestList = deepCopy(newList);
            }
            netIndex = 0;
            scenarioList = deepCopy(bestList);
        }
        else
        {
            scenarioList = deepCopy(newList);
            netIndex++;
        }
        evaluations = 0;
    }
    
    public bool containsCoin(string coinName)
    {
        bool found = false;
        foreach(ArrayList entry in scenarioList)
        {
            if (((string)entry[1]).Equals(coinName))
                found = true;
        }
        return found;
    }

    public NeuralNet getNet(int index)
    {
        return (NeuralNet)((ArrayList)scenarioList[index])[0];
    }

    public string getCoinName(int index)
    {
        if (index == scenarioList.Count)
            return "None";
        return (string)((ArrayList)scenarioList[index])[1];
    }

    public float getFitness(int index)
    {
        if (index == scenarioList.Count)
            return -1;
        return (float)((ArrayList)scenarioList[index])[2];
    }

    public Vector3 getPosition(int index)
    {
        return (Vector3)((ArrayList)scenarioList[index])[3];
    }

    public float getVelocityX(int index)
    {
        return (float)((ArrayList)scenarioList[index])[4];
    }

    public float getVelocityY(int index)
    {
        return (float)((ArrayList)scenarioList[index])[5];
    }

    public bool getFacingRight(int index)
    {
        return (bool)((ArrayList)scenarioList[index])[6];
    }

    public bool getJump(int index)
    {
        return (bool)((ArrayList)scenarioList[index])[7];
    }

    public bool getGrounded(int index)
    {
        return (bool)((ArrayList)scenarioList[index])[8];
    }

    public float count()
    {
        return scenarioList.Count;
    }

    public NeuralNet getBestNet(int index)
    {
        return bestList.Count > 0 ? (NeuralNet)((ArrayList)bestList[index])[0] : (NeuralNet)((ArrayList)scenarioList[index])[0];
    }

    public string getBestCoinName(int index)
    {
        if (index == bestCount())
            return "None";
        return bestList.Count > 0 ? (string)((ArrayList)bestList[index])[1] : (string)((ArrayList)scenarioList[index])[1];
    }

    public float getBestFitness(int index)
    {
        if (index == bestCount())
            return -1;
        return bestList.Count > 0 ? (float)((ArrayList)bestList[index])[2] : (float)((ArrayList)scenarioList[index])[2];
    }

    public float bestCount()
    {
        return bestList.Count > 0 ? bestList.Count : scenarioList.Count;
    }

    public virtual float getMaxEvaluations()
    {
        return maxEvaluations;
    }

    protected ArrayList scenarioCopy(ArrayList entry)
    {
        
        ArrayList scenario = new ArrayList();
        scenario.Add(((NeuralNet)entry[0]).copy());
        scenario.Add(((string)entry[1]).Clone());
        scenario.Add(((float)entry[2]));
        scenario.Add(((Vector3)entry[3]));
        scenario.Add(((float)entry[4]));
        scenario.Add(((float)entry[5]));
        scenario.Add(((bool)entry[6]));
        scenario.Add(((bool)entry[7]));
        scenario.Add(((bool)entry[8]));

        return scenario;
    }

    protected ArrayList deepCopy(ArrayList original)
    {
        ArrayList copy = new ArrayList();

        for (int i = 0; i < original.Count; i++)
        {
            copy.Add(scenarioCopy((ArrayList)original[i]));
        }

        return copy;
    }

    public virtual void submitNetScore(float score, ArrayList scenario)
    {
        if (score > getFitness(netIndex))
        {
            updateList(scenario);
        }
    }

    protected bool compareListScore(ArrayList incumbent, ArrayList challenger)
    {
        float iScore = 0f;
        float cScore = 0f;

        foreach (ArrayList entry in incumbent)
        {
            iScore += (float)entry[2];
        }

        foreach (ArrayList entry in challenger)
        {
            cScore += (float)entry[2];
        }
        if (cScore > iScore)
        {
            Debug.Log("New score " + cScore + " has beaten previous score " + iScore);
            return true;
        }
        else
        {
            Debug.Log("Old score " + iScore + " remains higher than new score " + cScore);
            return false;
        }
    }
}
