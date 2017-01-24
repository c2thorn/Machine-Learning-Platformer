using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiNetManager {

    public ArrayList netList = new ArrayList();
    public ArrayList coinList = new ArrayList();
    public ArrayList fitList = new ArrayList();

    public MultiNetAgent agent;

    public int netIndex = 0;
    public int netsOptimized = 0;

    public void setAgent(MultiNetAgent agent)
    {
        this.agent = agent;
    }

    public Vector3 getPreviousCoinPosition()
    {
        GameObject coin = GameObject.Find((string)coinList[netIndex - 1]);
        Vector3 returnPos = coin.transform.position;
        Object.Destroy(coin);
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
    

    public void addNewCoin(float score, NeuralNet net, string coinName)
    {
        fitList.Add(score);
        netList.Add(net);
        coinList.Add(coinName);
        netsOptimized++;
        netIndex++;
    }
}
