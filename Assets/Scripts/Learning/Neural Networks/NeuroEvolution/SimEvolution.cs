using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimEvolution {
    private NeuralNet[] population;
    private SimNEAgent[] agentPopulation;
    public float[] fitness;
    private int elite;
    public int popSize = 20;
    public double mutationRate = 0.5;
    public System.Random random = new System.Random();


    public string fileName = "";


    public SimEvolution(GameObject[] robos ,string loadPath)
    {
        popSize = robos.Length;
        agentPopulation = new SimNEAgent[popSize];
        population = new NeuralNet[popSize];
        for (int i = 0; i < population.Length; i++)
        {
            population[i] = new NeuralNet();
            agentPopulation[i] = robos[i].GetComponent<SimNEAgent>();
            robos[i].transform.position = new Vector3(robos[i].transform.position.x, robos[i].transform.position.y,i);
            agentPopulation[i].GenerationID = i;
            agentPopulation[i].ev = this;
        }
        SetColors();
        fitness = new float[popSize];
        for (int i = 0; i < popSize; i++)
            fitness[i] = -1;
        elite = 3;
    }
    

    public NeuralNet getAgentNet(int id)
    {
        return population[id];
    }

    public bool submitScore(int id, float score)
    {
        fitness[id] = score;
        if (CheckGenerationDone())
        {
            NextGeneration();
            return true;
        }
        return false;
    }
    
    private bool CheckGenerationDone()
    {
        bool done = true;
        for(int i = 0; i < popSize; i++)
        {
            if (fitness[i] == -1)
                done = false;
        }
        return done;
    }

    public void NextGeneration()
    {
        sortPopulationByFitness();
        for (int i = 0; i < population.Length; i++)
        {
            fitness[i] = -1;
        }
        MutatePop();
        SetZPositions();
        RestartAgents();
    }

    private void sortPopulationByFitness()
    {
        for (int i = 0; i < population.Length; i++)
        {
            for (int j = i + 1; j < population.Length; j++)
            {
                if (fitness[i] < fitness[j])
                {
                    swap(i, j);
                }
            }
        }
        //for (int i = 0; i < popSize; i++)
            //Debug.Log(fitness[i]);
    }

    private void swap(int i, int j)
    {
        float cache = fitness[i];
        fitness[i] = fitness[j];
        fitness[j] = cache;

        NeuralNet gcache = population[i].copy();
        population[i] = population[j].copy();
        population[j] = gcache;

        //Debug.Log("(" + i + "," + agentPopulation[i].GenerationID + ") swap with (" + j + "," + agentPopulation[j].GenerationID + ")");
        int idCache = agentPopulation[i].GenerationID;
        agentPopulation[i].GenerationID = agentPopulation[j].GenerationID;
        agentPopulation[j].GenerationID = idCache;

        SimNEAgent acache = agentPopulation[i];
        agentPopulation[i] = agentPopulation[j];
        agentPopulation[j] = acache;
    }



    private void MutatePop()
    {
        for (int i = 0; i < popSize; i++)
        {
            double mutationChance = random.NextDouble();
            if (i < elite)
            {
                //if (mutationChance < mutationRate)
                //    WeakMutate(i);
            }
            else
            {
                int copyIndex = i % elite;
                population[i] = population[copyIndex].copy();

                if (mutationChance < mutationRate)
                    WeakMutate(i);
                else
                    StrongMutate(i);
            }


        }
    }

    private void WeakMutate(int id)
    {
        population[id].mutationMagnitude = 0.3;
        population[id].mutate();
    }

    private void StrongMutate(int id)
    {
        population[id].mutationMagnitude = 1;
        population[id].mutate();
    }

    private void SetZPositions()
    {
        for (int i = 0; i< popSize; i++)
        {
            GameObject robo = agentPopulation[i].gameObject;
            robo.transform.position = new Vector3(robo.transform.position.x, robo.transform.position.y, i);
        }
    }

    private void SetColors()
    {
        for (int i = 0; i < popSize; i++)
        {
            GameObject robo = agentPopulation[i].gameObject;
            SpriteRenderer sprite = robo.GetComponentInChildren<SpriteRenderer>();
            float h = (float)i / (float)popSize;
            float s = 90f + (float)random.NextDouble() * 10f;
            float l = 90f + (float)random.NextDouble() * 10f;
            s /= 100;
            l /= 100;
            sprite.color = Color.HSVToRGB(h, s, l);
        }
    }

    private void RestartAgents()
    {
        for (int i = 0; i < popSize; i++)
        {
            agentPopulation[i].LevelRestart();
        }
    }
}
