using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour,Smart
{
    public Rigidbody2D rb;
    public float impulseIntensity;

    public float asensor;

    public float startTime;

    NeuralNetwork brain;

    public bool to_jump;

    public TrianglesSpawner sp;

    private void Awake()
    {
        brain = new NeuralNetwork(1, 1, 1);
        brain.setHiddenLayersSizes(new List<int> {1});
        brain.RandomizeWeights();
        startTime = Time.time;
    }

    private void Update()
    {
        if (to_jump)
        {
            jump();
            to_jump = false;
        }
    }

    private void FixedUpdate()
    {
        InputSensors();
        think(brain);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("obstacle"))
        {
            Reset();
        }
    }

    void Reset()
    {
        sp.ClearObstacles();
        brain.RandomizeWeights();

    }

    void InputSensors()
    {
        Vector2 a = Vector2.right;

        Debug.DrawRay(transform.position, a, Color.yellow);

        Ray2D ray = new Ray2D(transform.position, a);
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x +2,transform.position.y)
            , a);
        if(hit.collider != null)
        {
            asensor = Mathf.Abs(hit.point.x - transform.position.x-2) / 10;
        }
    }

    public void jump()
    {
        rb.AddForce(Vector2.up * impulseIntensity, ForceMode2D.Impulse);
    }

    public void think(NeuralNetwork net)
    {
        net.master = this;
        net.input.setAt(0, 0, asensor);
        net.activate();
        Debug.Log(net.output.getAt(0, 0));
        to_jump = net.output.getAt(0, 0) > 0.55;
    }

    public double getFitness()
    {
        return Time.time - startTime;
    }
}
