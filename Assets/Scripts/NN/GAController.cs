using System.Collections;
using System.Collections.Generic;
using Rand = UnityEngine.Random;
using UnityEngine;

public class GAController
{
    public List<NeuralNetwork> networks;

    public double probability_to_roulet_selection;

    public GAController(List<NeuralNetwork> networks)
    {
        this.networks = networks;
    }

    public (NeuralNetwork,NeuralNetwork) TournementParentSelection()
    {
        int father = Rand.Range(0, networks.Count);
        int mother = 0;
        do
        {
            mother = Rand.Range(0, networks.Count);
        } while (mother == father);

        return (networks[father],networks[mother]);
    }

    public (NeuralNetwork,NeuralNetwork) RouletParentSelection()
    {
        NeuralNetwork father = null, mother = null;

        //for this function all fitness values should be positive
        double[] fitnesses = new double[networks.Count];
        double sum = 0;
        double[] proba = new double[networks.Count];

        for(int i = 0; i < fitnesses.Length; i++)
        {
            fitnesses[i] = networks[i].fitness();
            sum += fitnesses[i];
        }
        //calculate the cumulative sum
        proba[0] = fitnesses[0] / sum;
        for (int i = 1; i < fitnesses.Length; i++)
        {
            proba[i] = fitnesses[i] / sum + proba[i-1];
        }

        double r = Rand.Range(0f, 1f);

        for (int i = 0; i < fitnesses.Length; i++)
        {
            if (r > proba[i])
            {
                father = networks[i];
                break;
            }
        }

        do
        {
            r = Rand.Range(0f, 1f);

            for (int i = 0; i < fitnesses.Length; i++)
            {
                if (r > proba[i])
                {
                    mother = networks[i];
                    break;
                }
            }
        } while (father == mother);

        return (father, mother);
    }

    public NeuralNetwork CrossOver()
    {
        double r = Rand.Range(0f, 1f);  //selection methods
        NeuralNetwork father, mother;

        (father, mother) = r < probability_to_roulet_selection ?
                RouletParentSelection() : TournementParentSelection();

        NeuralNetwork child = new NeuralNetwork
            (father.input_size, father.output_size, father.hidden_nb);
        List<int> l = new List<int>();
        for(int i = 0; i < father.hidden_sizes.Count; i++)
        {
            l.Add(father.hidden_sizes[i]);
        }
        child.setHiddenLayersSizes(l);
        // here we have only one hidden layer
        //so we have 2 weights and one bias
        int w0 = Rand.Range(0,
            father.weights[0].getLinesCount()* father.weights[0].getColumnsCount());
        int w1 = Rand.Range(0,
            father.weights[1].getLinesCount()* father.weights[1].getColumnsCount());

        //weight 0
        for (int i = 0; i < father.weights[0].getLinesCount(); i++)
        {
            for(int j = 0; j < father.weights[0].getColumnsCount(); j++)
            {
                if(w0-- != 0)
                {
                    child.weights[0].setAt(i, j, 
                        father.weights[0].getAt(i,j));
                }
                else
                {
                    child.weights[0].setAt(i, j,
                        mother.weights[0].getAt(i, j));
                }
            }
        }

        //weight 1
        for (int i = 0; i < father.weights[1].getLinesCount(); i++)
        {
            for (int j = 0; j < father.weights[1].getColumnsCount(); j++)
            {
                if (w1-- != 0)
                {
                    child.weights[1].setAt(i, j,
                        father.weights[1].getAt(i, j));
                }
                else
                {
                    child.weights[1].setAt(i, j,
                        mother.weights[1].getAt(i, j));
                }
            }
        }

        int bb = UnityEngine.Random.Range(0, 2);
        double b = (bb == 0 ? father.biases[0] : mother.biases[0]);
        child.biases.Add(b);


        return child;
    }
}
