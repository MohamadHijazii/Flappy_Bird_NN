using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Rand = UnityEngine.Random;
using MathNet.Numerics.LinearAlgebra;

public class GeneticManager : MonoBehaviour
{
    public bool pause;

    public static GeneticManager instance;

    // [-5,4] on x & [-2,3] on y

    public GameObject bird;     //sample to spawn
    
    public ObstaclesSpawner obstaclesSpawner;

    public NNet[] population;

    public int alive;
    public int populationPerGeneration;
    public int LAYERS, NEURONS;

    public int generation = 1;

    public int naturallySelected;

    public TextMeshProUGUI gen;

    public int bestAgentSelection, worstAgentSelection;
    public List<int> genePool;

    private float mutationRate = .05f;

    public int numberToCrossover;

    [Range(0,1)]
    public float startAddingAt;   //percentage of best population

    public static int index { get; private set; }

    private void Awake()
    {
        instance = this;
        index = 0;
        //alive = populationPerGeneration;
        gen.text = generation + "";
        Spawn(populationPerGeneration);
        naturallySelected = 0;
        genePool = new List<int>();
    }




    public void decreaseBird(NNet net,float fitness,int id)
    {

        alive--;
        population[id].fitness = fitness;
        if(alive == 0)
        {
            generation++;
            gen.text = "Generation: "+generation;
            //Start a new generation
            naturallySelected = 0;
            genePool.Clear();
            NNet []newPopulation = PickBestPopulation();
            //cross over
            Crossover(newPopulation);
            //mutate
            Mutate(newPopulation);
            //fill the rest with random values 
            FillPopulationWithRandomValues(newPopulation, naturallySelected);
            population = newPopulation;
            obstaclesSpawner.ClearObstacles();
            foreach(NNet n in population)
            {
                Spawn(n);
            }
            index = 0;
        }

    }

    private void Crossover(NNet[] newPopulation)
    {
        for (int i = 0; i < numberToCrossover; i += 2)
        {
            int AIndex = i;
            int BIndex = i + 1;

            if (genePool.Count >= 1)
            {
                for (int l = 0; l < 100; l++)
                {
                    AIndex = genePool[Random.Range(0, genePool.Count)];
                    BIndex = genePool[Random.Range(0, genePool.Count)];

                    if (AIndex != BIndex)
                        break;
                }
            }

            NNet Child1 = new NNet();
            NNet Child2 = new NNet();

            Child1.Initialise(LAYERS, NEURONS);
            Child2.Initialise(LAYERS, NEURONS);

            Child1.fitness = 0;
            Child2.fitness = 0;


            for (int w = 0; w < Child1.weights.Count; w++)
            {

                if (Random.Range(0.0f, 1.0f) < 0.5f)
                {
                    Child1.weights[w] = population[AIndex].weights[w];
                    Child2.weights[w] = population[BIndex].weights[w];
                }
                else
                {
                    Child2.weights[w] = population[AIndex].weights[w];
                    Child1.weights[w] = population[BIndex].weights[w];
                }

            }


            for (int w = 0; w < Child1.biases.Count; w++)
            {

                if (Random.Range(0.0f, 1.0f) < 0.5f)
                {
                    Child1.biases[w] = population[AIndex].biases[w];
                    Child2.biases[w] = population[BIndex].biases[w];
                }
                else
                {
                    Child2.biases[w] = population[AIndex].biases[w];
                    Child1.biases[w] = population[BIndex].biases[w];
                }

            }

            newPopulation[naturallySelected] = Child1;
            naturallySelected++;

            newPopulation[naturallySelected] = Child2;
            naturallySelected++;

        }
    }

    private void Mutate(NNet[] newPopulation)
    {

        for (int i = 0; i < naturallySelected; i++)
        {

            for (int c = 0; c < newPopulation[i].weights.Count; c++)
            {

                if (Random.Range(0.0f, 1.0f) < mutationRate)
                {
                    newPopulation[i].weights[c] = MutateMatrix(newPopulation[i].weights[c]);
                }

            }

        }

    }

    Matrix<float> MutateMatrix(Matrix<float> A)
    {

        int randomPoints = Random.Range(1, (A.RowCount * A.ColumnCount) / 7);

        Matrix<float> C = A;

        for (int i = 0; i < randomPoints; i++)
        {
            int randomColumn = Random.Range(0, C.ColumnCount);
            int randomRow = Random.Range(0, C.RowCount);

            C[randomRow, randomColumn] = Mathf.Clamp(C[randomRow, randomColumn] + Random.Range(-1f, 1f), -1f, 1f);
        }

        return C;

    }

    void Spawn(NNet net)
    {
        alive++;
        Vector2 vec = new Vector2(Rand.Range(-5.0f, 4.0f), Rand.Range(-2.0f, 3.0f));
        GameObject b = Instantiate(bird, vec, Quaternion.identity);
        Bird controller = b.GetComponent<Bird>();
        controller.setNetwork(net);
        controller.index = index++;
        //birds.Add(controller);
    }



    void Spawn(int n)
    {
        alive += n;
        population = new NNet[populationPerGeneration];
        FillPopulationWithRandomValues(population, 0);
        for (int i = 0; i < n; i++)
        {
            Vector2 vec = new Vector2(Rand.Range(-5.0f, 4.0f), Rand.Range(-2.0f, 3.0f));
            GameObject b = Instantiate(bird, vec, Quaternion.identity);
            Bird cont = b.GetComponent<Bird>();
            cont.brain = population[i];
            //birds.Add(b.GetComponent<BirdController>());
        }
    }

    private NNet[] PickBestPopulation()
    {

        NNet[] newPopulation = new NNet[populationPerGeneration];

        for (int i = 0; i < bestAgentSelection; i++)
        {
            newPopulation[naturallySelected] = population[i].InitialiseCopy(LAYERS, NEURONS);
            newPopulation[naturallySelected].fitness = 0;
            naturallySelected++;

            int f = Mathf.RoundToInt(population[i].fitness * 10);

            for (int c = 0; c < f; c++)
            {
                genePool.Add(i);
            }

        }

        for (int i = 0; i < worstAgentSelection; i++)
        {
            int last = population.Length - 1;
            last -= i;

            int f = Mathf.RoundToInt(population[last].fitness * 10);

            for (int c = 0; c < f; c++)
            {
                genePool.Add(last);
            }

        }

        return newPopulation;

    }

    private void FillPopulationWithRandomValues(NNet[] newPopulation, int startingIndex)
    {
        while (startingIndex < populationPerGeneration)
        {
            newPopulation[startingIndex] = new NNet();
            newPopulation[startingIndex].Initialise(1,NEURONS);
            startingIndex++;
        }
    }

    private void SortPopulation()
    {
        for (int i = 0; i < population.Length; i++)
        {
            for (int j = i; j < population.Length; j++)
            {
                if (population[i].fitness < population[j].fitness)
                {
                    NNet temp = population[i];
                    population[i] = population[j];
                    population[j] = temp;
                }
            }
        }

    }
}
