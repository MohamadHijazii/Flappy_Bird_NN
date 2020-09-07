using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class NeuralNetwork
{
    public Matrix input;
    List<Matrix> hiddenLayers;
    public Matrix output;
    public readonly List<Matrix> weights;
    public readonly List<double> biases;

    public readonly int input_size;
    public readonly int output_size;
    public readonly int hidden_nb;
    public List<int> hidden_sizes;

    public double mutation;


    public Smart master;

    public NeuralNetwork(int input_size, int output_size, int hidden_nb)
    {
        this.input_size = input_size;
        this.output_size = output_size;
        this.hidden_nb = hidden_nb;
        input = new Matrix(input_size, 1);
        output = new Matrix(output_size, 1);
        weights = new List<Matrix>();
        weights.Capacity = hidden_nb + 1;
        biases = new List<double>();
        hiddenLayers = new List<Matrix>();
        hiddenLayers.Capacity = hidden_nb;
    }

    public void setHiddenLayersSizes(List<int> sizes)
    {
        hidden_sizes = sizes;
        //input to first hidden layer
        Matrix w;
        for (int i = 0; i < sizes.Count; i++)
        {
            Matrix h = new Matrix(sizes[i], 1);
            hiddenLayers.Add(h);
            if(i == 0)
            {
                w  = new Matrix(
                            hiddenLayers[0].
                            getLinesCount(),
                                    input.getLinesCount());
                        weights.Add(w);
            }
            else
            {
                w = new Matrix(hiddenLayers[i].getLinesCount(),
                    hiddenLayers[i-1].getLinesCount());
                weights.Add(w);
            }
        }
        //last hidden layer to output
        w = new Matrix(output.getLinesCount(),
                    hiddenLayers[hiddenLayers.Count-1].getLinesCount());
        weights.Add(w);
    }

    public void RandomizeWeights()
    {
        foreach(Matrix m in weights)
        {
            for(int i = 0; i < m.getLinesCount(); i++)
            {
                for(int j = 0; j < m.getColumnsCount(); j++)
                {
                    m.setAt(i, j, 
                        UnityEngine.Random.Range(0f,1.0f));
                }
            }
        }
        biases.Add(UnityEngine.Random.Range(0f, 1.0f));
    }

    public double[] activate()
    {
        hiddenLayers[0] = weights[0].mult(input);
        output = weights[1].mult(hiddenLayers[0]);
        return output.getColumnAt(0);
    }

    public static double sigmoid(double val)
    {
        double e = Math.Exp(-val);
        return 1 / (1+e);
    }

    public static double tanh(double val)
    {
        return 2*sigmoid(2*val) -1;
    }

    public static double [] sigmoid(double[] t)
    {
        double[] o = new double[t.Length];
        for(int i = 0; i < t.Length; i++)
        {
            o[i] = sigmoid(t[i]);
        }
        return o;
    }

    public static double [] tanh(double[] t)
    {
        double []o = new double[t.Length];
        for (int i = 0; i < t.Length; i++)
        {
            o[i] = tanh(t[i]);
        }
        return o;
    }

    public static Matrix sigmoid(Matrix m)
    {
        Matrix r = new Matrix(m.getLinesCount(), m.getColumnsCount());
        for(int i = 0; i < r.getLinesCount(); i++)
        {
            for(int j = 0; j < r.getColumnsCount(); j++)
            {
                r.setAt(i, j, sigmoid(m.getAt(i, j)));
            }
        }
        return r;
    }

    public static Matrix tanh(Matrix m)
    {
        Matrix r = new Matrix(m.getLinesCount(), m.getColumnsCount());
        for (int i = 0; i < r.getLinesCount(); i++)
        {
            for (int j = 0; j < r.getColumnsCount(); j++)
            {
                r.setAt(i, j, tanh(m.getAt(i, j)));
            }
        }
        return r;

    }


    public NeuralNetwork combain(NeuralNetwork other)
    {
        NeuralNetwork child = new NeuralNetwork(input_size, output_size, hidden_nb);
        List<int> sizes = new List<int>(hidden_sizes);
        child.setHiddenLayersSizes(sizes);
            int xx = 0;

        for (int k=0;k<child.weights.Count;k++)
        {
            Matrix m  = child.weights[k];
            Matrix p1 = weights[k];
            Matrix p2 = other.weights[k];
            for (int i = 0; i < m.getLinesCount(); i++)
            {
                for (int j = 0; j < m.getColumnsCount(); j++)
                {
                    xx = UnityEngine.Random.Range(0, 2);
                    m.setAt(i, j, xx == 0 ?p1.getAt(i,j):p2.getAt(i,j));
                }
            }
        }
        xx = UnityEngine.Random.Range(0, 2);

        double  b = (xx == 0 ? biases[0] : other.biases[0]);
        child.biases.Add(b);
        return child;
    }

    public double getWeightsHash()
    {
        double h = 0;
        for (int k = 0; k < weights.Count; k++)
        {
            Matrix m = weights[k];
            for (int i = 0; i < m.getLinesCount(); i++)
            {
                for (int j = 0; j < m.getColumnsCount(); j++)
                {
                    double w = m.getAt(i, j);
                    h += w * 17;
                }
            }
        }

        return h;
    }

    public double fitness() => master.getFitness();

    public NeuralNetwork Clone()
    {
        NeuralNetwork clone = new NeuralNetwork(input_size, output_size, hidden_nb);
        List<int> l = new List<int>();
        foreach(int i in hidden_sizes)
        {
            l.Add(i);
        }
        clone.setHiddenLayersSizes(l);
        for (int k = 0; k < weights.Count; k++)
        {
            Matrix m = clone.weights[k];
            for (int i = 0; i < weights[k].getLinesCount(); i++)
            {
                for (int j = 0; j < weights[k].getColumnsCount(); j++)
                {
                    double w = weights[k].getAt(i, j);
                    m.setAt(i, j,w);
                }
            }
        }
        for(int i = 0; i < biases.Count; i++)
        {
            clone.biases.Add(biases[i]);
        }
        return clone;
    }
}
