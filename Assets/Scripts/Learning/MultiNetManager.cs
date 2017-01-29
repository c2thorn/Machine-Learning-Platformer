using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiNetManager {

    /**
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
    private ArrayList scenarioList = new ArrayList();
    private ArrayList bestList = new ArrayList();

    public MultiNetAgent agent;

    public int netIndex = 0;

    public int evaluations = 0;
    public int maxDiscoveringEvaluations = 1500;
    public int maxOptimizingEvaluations = 500;

    public bool optimizing = false;

    private int lastOptimalIndex = -1;

    private int optimizingLoopCounter = 0;
    private int maxOptimizingAttempts = 5;

    public void setAgent(MultiNetAgent agent)
    {
        this.agent = agent;
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
                //scenarioList = (ArrayList)bestList.Clone();
                netIndex = 0;
                evaluations = 0;
                lastOptimalIndex = -1;
                optimizingLoopCounter = 0;
            }
            else
                evaluations++;
        }
    }

    public void destroyCoins()
    {
        for (int i = 0; i < netIndex; i++)
        {
            GameObject coin = GameObject.Find(getCoinName(i));
            Object.Destroy(coin);
        }
    }

    /**
    * If there are no entries, add the new scenario.
    * Otherwise, replace the current entry with the new scenario and delete the rest.
    */
    public void updateList(ArrayList scenario)
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

            if (compareScore(bestList, newList))
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
                netIndex = lastOptimalIndex+1;
                Debug.Log("Retrying from: " + lastOptimalIndex + ". Attempt: " + optimizingLoopCounter);
                //netIndex = lastOptimalIndex;
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
        //Debug.Log(index + " " + (bestList.Count > 0) + " " + (bestList.Count > 0 ? ((NeuralNet)((ArrayList)bestList[index])[0]).firstConnectionLayer[0][0] : ((NeuralNet)((ArrayList)scenarioList[index])[0]).firstConnectionLayer[0][0]));
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

    public float getMaxEvaluations()
    {
        return optimizing ? maxOptimizingEvaluations : maxDiscoveringEvaluations;
    }

    private ArrayList scenarioCopy(ArrayList entry)
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

    private ArrayList deepCopy(ArrayList original)
    {
        ArrayList copy = new ArrayList();

        for (int i = 0; i < original.Count; i++)
        {
            copy.Add(scenarioCopy((ArrayList)original[i]));
        }

        return copy;
    }

    private bool compareScore(ArrayList incumbent, ArrayList challenger)
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
