using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiNetManager {

    public ArrayList netList = new ArrayList();
    public ArrayList coinList = new ArrayList();
    public ArrayList fitList = new ArrayList();
    public ArrayList scenarioList = new ArrayList();

    public MultiNetAgent agent;

    public int netIndex = 0;
    public int netsOptimized = 0;

    public int evaluations = 0;
    public int maxEvaluations = 500;

    public bool won = false;

    public void setAgent(MultiNetAgent agent)
    {
        this.agent = agent;
        if (won)
        {
            if (evaluations >= maxEvaluations)
            {
                netIndex++;
                evaluations = 0;
            }
            else
                evaluations++;
        }
    }

    public void destroyCoins()
    {
        for (int i = 0; i < netIndex; i++)
        {
            GameObject coin = GameObject.Find((string)coinList[i]);
            Object.Destroy(coin);
        }
    }

    public Vector3 getPreviousCoinPosition()
    {
        GameObject coin = GameObject.Find((string)coinList[netIndex - 1]);
        Vector3 returnPos = coin.transform.position;
        destroyCoins();
        return returnPos;
    }

    public void gotCoin(string coinName)
    {
        /*if (!coinList.Contains(coinName))
        {
            coinList.Add(coinName);
        }
        if (netIndex == netsOptimized)
        {

        }*/
    }

    public void replaceCoin(float score, NeuralNet net, string coinName, ArrayList scenario)
    {
        int index = coinList.IndexOf(coinName);
        fitList[index] = score;
        netList[index] = net;
        coinList[index] = coinName;
        scenarioList[index] = scenario;
        //netsOptimized++;
        netIndex++;
    }

    public void addNewCoin(float score, NeuralNet net, string coinName, ArrayList scenario)
    {
        fitList.Insert(netIndex, score);
        netList.Insert(netIndex, net);
        coinList.Insert(netIndex, coinName);
        scenarioList.Insert(netIndex, scenario);
        netsOptimized++;
        netIndex++;
    }
}
