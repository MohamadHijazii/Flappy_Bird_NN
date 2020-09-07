using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rand = UnityEngine.Random;
using TMPro;
public class BirdSpawner : MonoBehaviour
{
    public static BirdSpawner instance;

    // [-5,4] on x & [-2,3] on y

    public GameObject bird;     //sample to spawn
    public List<BirdController> birds;
    public List<NeuralNetwork> bestNetworks;
    public List<NeuralNetwork> current;
    public ObstaclesSpawner obstaclesSpawner;


    public int alive;
    public int populationPerGeneration;

    public double minAcceptedFitness, fitnessThreshold;
    public int generation;

    public TextMeshProUGUI gen;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        generation = 1;
        Spawn(populationPerGeneration);
        bestNetworks = new List<NeuralNetwork>();
        current = new List<NeuralNetwork>();
    }

    public void DecreaseBird(NeuralNetwork net,double fitness)
    {
        

        alive--;

        if (fitness >= fitnessThreshold)
        {
            //this is one of the best networks
            bestNetworks.Add(net);
        }
        else
        {
            if (fitness >= minAcceptedFitness || alive ==0 || alive == 1 || alive == 2)
            {
                current.Add(net);
            }
        }

        if(alive == 0)
        {
            GAController best = new GAController(bestNetworks);
            GAController cur = new GAController(current);
            // we need to start a new generation
            generation++;
            gen.text = $"Generation: {generation}";
            obstaclesSpawner.ClearObstacles();
            int should_spawn = populationPerGeneration;

            if(bestNetworks.Count >= 2)
            {
                // 80% of the population
                int p = (int) (0.8f * should_spawn);
                should_spawn -= p;
                for(int i = 0; i < p; i++)
                {
                    NeuralNetwork network = best.CrossOver();
                    Spawn(network);
                }
                Debug.Log("Spawned From The Best:" + p);
            }
            if(current.Count >= 2)
            {
                int p = (int)(0.2f * should_spawn);
                should_spawn -= p;
                for (int i = 0; i < p; i++)
                {
                    NeuralNetwork network = cur.CrossOver();
                    Spawn(network);
                }
                Debug.Log("Spawned From The Current :" + p);

            }

            Spawn(should_spawn);
            Debug.Log("Spawned Randomly:" + should_spawn);

            current.Clear();
        }
    }

    void Spawn(NeuralNetwork net)
    {
        alive++;
        Vector2 vec = new Vector2(Rand.Range(-5.0f, 4.0f), Rand.Range(-2.0f, 3.0f));
        GameObject b = Instantiate(bird, vec, Quaternion.identity);
        BirdController controller = b.GetComponent<BirdController>();
        controller.brain = net;
        birds.Add(controller);
    }
    


    void Spawn(int n)
    {
        alive += n;
        for(int i = 0; i < n; i++)
        {
            Vector2 vec = new Vector2(Rand.Range(-5.0f,4.0f), Rand.Range(-2.0f,3.0f));
            GameObject b = Instantiate(bird, vec, Quaternion.identity);
            birds.Add(b.GetComponent<BirdController>());
        }

    }
}
