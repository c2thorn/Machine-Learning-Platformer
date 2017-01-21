using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiNetManager {

    public ArrayList netList = new ArrayList();
    public int netIndex = 0;
    public int netsOptimized = 0;
    public ArrayList coinList = new ArrayList(); 

    public NeuralNet getCurrentNet()
    {
        return (NeuralNet)netList[netIndex];
    }
}
