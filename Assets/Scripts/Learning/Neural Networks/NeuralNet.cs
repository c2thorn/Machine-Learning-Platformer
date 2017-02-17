using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class NeuralNet {
    public static int numberOfInputs = 29;
    public static int numberOfHidden = 10;
    public static int numberOfOutputs = 3;

    public double[][] firstConnectionLayer = new double[numberOfInputs][];
    private double[][] secondConnectionLayer = new double[numberOfHidden][];
    private double[] hiddenNeurons = new double[numberOfHidden];
    private double[] outputs = new double[numberOfOutputs];
    private double[] inputs = new double[numberOfInputs];
    //private double[] targetOutputs;
    public double mutationMagnitude = .1;
    public static System.Random random = new System.Random();

    public static double mean = 0.0f;        // initialization mean
    public static double deviation = 1f;   // initialization deviation

    public double learningRate = 0.01;

    public NeuralNet(double[][] firstLayer, double[][] secondLayer)
    {
        firstConnectionLayer = firstLayer;
        secondConnectionLayer = secondLayer;
        inputs = new double[firstConnectionLayer.Length];
        hiddenNeurons = new double[numberOfHidden];
        outputs = new double[numberOfOutputs];
    }

    public NeuralNet()
    {
        firstConnectionLayer = new double[numberOfInputs][];
        for (int i = 0; i < numberOfInputs; i++)
        {
            firstConnectionLayer[i] = new double[numberOfHidden];
        }
        secondConnectionLayer = new double[numberOfHidden][];
        for (int i = 0; i < numberOfHidden; i++)
        {
            secondConnectionLayer[i] = new double[numberOfOutputs];
        }
        hiddenNeurons = new double[numberOfHidden];
        outputs = new double[numberOfOutputs];
        inputs = new double[numberOfInputs];
        initializeLayer(firstConnectionLayer);
        initializeLayer(secondConnectionLayer);
    }

    public NeuralNet(String fileName)
    {
        string[] lines = System.IO.File.ReadAllLines(@"NeuralNets\" + fileName + ".txt");
        for (int i = 0; i < numberOfInputs; i++)
        {
            firstConnectionLayer[i] = new double[numberOfHidden];
        }
        secondConnectionLayer = new double[numberOfHidden][];
        for (int i = 0; i < numberOfHidden; i++)
        {
            secondConnectionLayer[i] = new double[numberOfOutputs];
        }

        for (int i = 0; i < numberOfInputs; i++)
        {
            String[] vals = lines[i].Split(' ');
            for (int j = 0; j < numberOfHidden; j++)
                firstConnectionLayer[i][j] = Double.Parse(vals[j]);
        }
        for (int i = 0; i < numberOfHidden; i++)
        {
            String[] vals = lines[i + numberOfInputs].Split(' ');
            for (int j = 0; j < numberOfOutputs; j++)
                secondConnectionLayer[i][j] = Double.Parse(vals[j]);
        }
        inputs = new double[firstConnectionLayer.Length];
        hiddenNeurons = new double[numberOfHidden];
        outputs = new double[numberOfOutputs];
    }

    protected void initializeLayer(double[][] layer)
    {
        for (int i = 0; i < layer.Length; i++)
        {
            for (int j = 0; j < layer[i].Length; j++)
            {
                layer[i][j] = mean + deviation*randGaussian();
            }
        }
    }

    private double randGaussian()
    {
        double u1 = random.NextDouble(); //these are uniform(0,1) random doubles
        double u2 = random.NextDouble();
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                     Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
        return randStdNormal;
    }

    private double[][] copy(double[][] original)
    {
        double[][] copy = new double[original.Length][];
        for (int i = 0; i < original.Length; i++)
        {
            copy[i] = new double[original[i].Length];
            Array.Copy(original[i], 0, copy[i], 0, original[i].Length);
        }
        return copy;
    }

    public void mutate()
    {
        mutate(firstConnectionLayer);
        mutate(secondConnectionLayer);
    }

    private void mutate(double[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] += randGaussian() * mutationMagnitude;
        }
    }

    private void mutate(double[][] array)
    {
        for (int i = 0; i < array.Length;i++)
        {
            mutate(array[i]);
        }
    }

    private void clear(double[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = 0;
        }
    }

    private void propagateOneStep(double[] fromLayer, double[] toLayer, double[][] connections)
    {
        clear(toLayer);
        for (int from = 0; from < fromLayer.Length; from++)
        {
            for (int to = 0; to < toLayer.Length; to++)
            {
                toLayer[to] += fromLayer[from] * connections[from][to];
                //Debug.Log("From : " + from + " to: " + to + " :: " +toLayer[to] + "+=" +  fromLayer[from] + "*"+  connections[from][to]);
            }
        }
    }
    
    public double[] propagate(double[] inputIn)
    {
        
        if (inputs != inputIn)
        {
            Array.Copy(inputIn, 0, inputs, 0, inputIn.Length);
        }
        if (inputIn.Length < inputs.Length)
            Debug.Log("MLP: NOTE: only " + inputIn.Length + " inputs out of " + inputs.Length + " are used in the network");
        propagateOneStep(inputs, hiddenNeurons, firstConnectionLayer);
        tanh(hiddenNeurons);
        propagateOneStep(hiddenNeurons, outputs, secondConnectionLayer);
        tanh(outputs);

        /*System.Random random = new System.Random();
        for (int i = 0; i < outputs.Length;i++)
        {
            outputs[i] = random.Next(2);
        }*/

        //Debug.Log("Outputs " + outputs[0] + " " + outputs[1] + " " + outputs[2]);
        

        return outputs;

    }

    private void tanh(double[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = Math.Tanh(array[i]);
            // for the sigmoid
            // array[i] = array[i];
            //array[i] = sig(array[i]);//array[i];//
        }
    }
    
    public NeuralNet copy()
    {
        NeuralNet second = new NeuralNet(copy(firstConnectionLayer), copy(secondConnectionLayer));
        return second;
    }

    public void writeNet(String name)
    {
        string[] lines = new String[numberOfInputs + numberOfHidden];
        for (int i = 0; i < numberOfInputs; i++)
        {
            for (int j = 0; j < numberOfHidden; j++)
                lines[i] += firstConnectionLayer[i][j]+" ";
        }
        for (int i = 0; i < numberOfHidden; i++)
        {
            for (int j = 0; j < numberOfOutputs; j++)
                lines[i+numberOfInputs] += secondConnectionLayer[i][j] + " ";
        }
        // WriteAllLines creates a file, writes a collection of strings to the file,
        // and then closes the file.  You do NOT need to call Flush() or Close().
        System.IO.File.WriteAllLines(@"NeuralNets\" + name + ".txt", lines);
    }
}
