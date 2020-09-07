using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Smart
{
    void think(NeuralNetwork net);

    double getFitness();
}
