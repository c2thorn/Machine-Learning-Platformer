using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Evolution Class Where Elite are untouched
 * */

public class Evolution {
    private NeuralNet[] population;
    public float[] fitness;
    private int elite;
    public int evaluationRepetitions = 1;
    private int evalIndex = 0;
    public int popIndex = 0;
    public int popSize = 90;

    private NeuralNet best;
    private float bestFit = 0f;

    public Evolution()
    {
        population = new NeuralNet[popSize];
        for (int i = 0; i < population.Length; i++)
        {
            population[i] = new NeuralNet("1073 Score");
            //population[i] = new NeuralNet();
        }

        fitness = new float[popSize];
        elite = popSize / 2;
    }

    public NeuralNet getCurrentNet()
    {
        return population[popIndex];
    }

    public void submitScore(float score)
    {
        if (score >= bestFit)
        {
            bestFit = score;
            best = population[popIndex].copy();
            population[popIndex].writeNet(score + " Score");
        }
        fitness[popIndex] += score;
        evalIndex++;
        if (evalIndex >= evaluationRepetitions)
        {
            fitness[popIndex] = fitness[popIndex] / evaluationRepetitions;
            evalIndex = 0;
            popIndex++;
            if (popIndex == elite)
                cullWeak();
           else if (popIndex >= population.Length)
            {
                //nextGeneration();
                nextGenerationEliteExclude();
            }
        }      
    }

    private void cullWeak()
    {
        for (int i = elite; i < population.Length; i++)
        {
            population[i] = population[i - elite].copy();
            population[i].mutate();
        }
    }

    public void nextGeneration()
    {
        shuffle();
        sortPopulationByFitness();
        for (int i = 0; i < population.Length; i++)
        {
            fitness[i] = 0;
        }
        popIndex = 0;
    }

    public void nextGenerationEliteExclude()
    {
        sortPopulationByFitness();
        for (int i = elite; i < population.Length; i++)
        {
            fitness[i] = 0;
        }
        popIndex = elite;
        cullWeak();
    }

    private void evaluate(int which)
    {
        fitness[which] = 0;
        fitness[which] = fitness[which] / evaluationRepetitions;
    }

    private void shuffle()
    {
        System.Random rand = new System.Random();
        for (int i = 0; i < population.Length; i++)
        {
            swap(i, (int)(rand.NextDouble() * population.Length));
        }
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
    }

    private void swap(int i, int j)
    {
        float cache = fitness[i];
        fitness[i] = fitness[j];
        fitness[j] = cache;
        NeuralNet gcache = population[i].copy();
        population[i] = population[j].copy();
        population[j] = gcache;
    }

    public NeuralNet getBestNet()
    {
        /*float best = 0f;
        int index = 0;
        for (int i = 0; i < fitness.Length; i++)
        {
            if (fitness[i] > best)
            {
                best = fitness[i];
                index = i;
            }
        }
        return population[index];
        */
        return best;
    }
}
