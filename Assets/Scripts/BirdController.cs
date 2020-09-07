using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdController : MonoBehaviour , Smart
{
    public Rigidbody2D rb;

    public float impulseIntesity;
    bool alive = true;
    public float start_position;
    public float parcoured_distance;
    public int number_of_blocks_parcourd = 0;
    public NeuralNetwork brain;

    float asensor, bsensor, csensor, dsensor, esensor;
    float upsensor, downsensor;

    bool to_jump;

    public Transform eye;

    float to_allow_juming;

    private void Start()
    {
        start_position = transform.position.x;
        if(brain == null)
        {
            brain = new NeuralNetwork(7, 1, 1);
            brain.setHiddenLayersSizes(new List<int> {3});
            brain.RandomizeWeights();
            //startFrom();
        }
        to_allow_juming = Time.time + 0.4f;
    }

    public void startFrom()
    {
        Matrix m1 = new Matrix(3, 7);
        Matrix m2 = new Matrix(1, 3);
        double []l1 = new double[] { -0.4,-0.3,0,0,0,0.3,0};
        double []l2 = new double[] { 0,0,-1,0,0,0,0};
        double []l3 = new double[] { 0,0,0,0.3,0.4,0,0.3};
        double []l4 = new double[] {0.3,0.4,-0.3 };
        m1.addLine(l1);
        m1.addLine(l2);
        m1.addLine(l3);
        m2.addLine(l4);
        brain.weights[0] = m1;
        brain.weights[1] = m2;
        brain.biases.Add(0);
    }

    private void Update()
    {
        if (to_jump && Time.time > to_allow_juming)
        {
            jump();
            to_allow_juming = Time.time + 0.35f;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump();
            to_allow_juming = Time.time + 100;
        }
    }

    private void FixedUpdate()
    {
        InputSensors();
        think(brain);
        
    }

    public void jump()
    {
        rb.AddForce(Vector2.up * impulseIntesity, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("middle"))
        {
            number_of_blocks_parcourd++;
            return;
        }
        if (collision.CompareTag("Player"))
            return;

        Die();
        if (collision.CompareTag("ground"))
        {
            Destroy(gameObject, 0.5f);
        }
    }

    void Die()
    {
        if (alive)
        {
            alive = false;
            BirdSpawner.instance.DecreaseBird(brain.Clone(),getFitness());
            transform.eulerAngles += new Vector3(0,0,38);
            impulseIntesity = 0;
            parcoured_distance = transform.position.x - start_position;
        }
    }

    public void InputSensors()
    {
        Vector2 a = (2*Vector2.up + Vector2.right);
        Vector2 b = (Vector2.up + 2*Vector2.right);
        Vector2 c = (Vector2.right);
        Vector2 d = (-2*Vector2.up + Vector2.right);
        Vector2 e = (-Vector2.up + 2*Vector2.right);

        Debug.DrawRay(eye.position,a,Color.red);
        Debug.DrawRay(eye.position,b,Color.red);
        Debug.DrawRay(eye.position,c,Color.red);
        Debug.DrawRay(eye.position,d,Color.red);
        Debug.DrawRay(eye.position,e,Color.red);
        Debug.DrawRay(eye.position, Vector2.up, Color.yellow);
        Debug.DrawRay(eye.position, Vector2.down, Color.yellow);

        RaycastHit2D hit;

        hit = Physics2D.Raycast(eye.transform.position,a);
        if(hit.collider != null && hit.collider.CompareTag("obstacle")){

            asensor = Mathf.Abs(hit.point.x - eye.transform.position.x)/10;
            //Debug.Log("A: " + asensor);
        }

        hit = Physics2D.Raycast(eye.transform.position, b);
        if (hit.collider != null && hit.collider.CompareTag("obstacle"))
        {
            bsensor = Mathf.Abs(hit.point.x - eye.transform.position.x)/10;
            //Debug.Log("B: " + bsensor);
        }

        hit = Physics2D.Raycast(eye.transform.position, c);
        if (hit.collider != null && hit.collider.CompareTag("obstacle"))
        {
            csensor = Mathf.Abs(hit.point.x - eye.transform.position.x)/10;
            //Debug.Log("C: "+csensor);
        }
        hit = Physics2D.Raycast(eye.transform.position, d);
        if (hit.collider != null && hit.collider.CompareTag("obstacle"))
        {

            dsensor = Mathf.Abs(hit.point.x - eye.transform.position.x)/10;
            //Debug.Log("D: " + dsensor);
        }

        hit = Physics2D.Raycast(eye.transform.position, e);
        if (hit.collider != null && hit.collider.CompareTag("obstacle"))
        {

            esensor = Mathf.Abs(hit.point.x - eye.transform.position.x)/10;
            //Debug.Log("E: " + esensor);
        }

        hit = Physics2D.Raycast(eye.position, Vector2.up);
        if (hit.collider != null && (hit.collider.CompareTag("ground") || hit.collider.CompareTag("obstacle")))
        {

            upsensor = Mathf.Abs(hit.point.y - eye.transform.position.y)/10;
            //Debug.Log("Up: " + upsensor);
        }

        hit = Physics2D.Raycast(eye.position, Vector2.down);
        if (hit.collider != null && (hit.collider.CompareTag("ground") || hit.collider.CompareTag("obstacle")))
        {

            downsensor = Mathf.Abs(hit.point.y - eye.transform.position.y)/10;
            //Debug.Log("Down: " + downsensor);
        }
    }

    public void think(NeuralNetwork net)
    {
        net.master = this;
        double[] inp = new double[] {asensor,bsensor,csensor,dsensor,esensor };
        double output;
        net.input.setAt(0, 0, asensor);
        net.input.setAt(1, 0, bsensor);
        net.input.setAt(2, 0, csensor);
        net.input.setAt(3, 0, dsensor);
        net.input.setAt(4, 0, esensor);
        net.input.setAt(5, 0, upsensor);
        net.input.setAt(6, 0, downsensor);
        double[] outpt = net.activate();
        output = NeuralNetwork.sigmoid(outpt[0]);
        //Debug.Log("Out: "+output);
        to_jump = output < 0.5;
    }

    public double getFitness()
    {
        return parcoured_distance + Mathf.Pow(2,number_of_blocks_parcourd);
    }
}
