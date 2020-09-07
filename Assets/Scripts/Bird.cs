using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NNet))]
public class Bird : MonoBehaviour
{
    Rigidbody2D rb;
    public float impulseIntensity;

    public float startPosition;
    public  NNet brain;

    public bool should_jump;
    public float to_allow_juming;
    public float start_time;
    public float surviving_time;
    public float parcoured_distance;
    public int number_of_blocks_parcourd = 0;

    float hsensor;
    float upsensor, downsensor; //for the ground
    float upBlock, downBlock;   //for the blocks

    public Transform eye;

    bool alive;

    public int index = 0;

    private void Awake()
    {
        startPosition = transform.position.x;
        brain = GetComponent<NNet>();
        alive = true;
        rb = GetComponent<Rigidbody2D>();
        to_allow_juming = Time.time;
        start_time = Time.time;
    }

    private void FixedUpdate()
    {
        InputSensor();

        should_jump = brain.RunNetwork(hsensor, upsensor, downsensor,upBlock,downBlock) > 0.5f;

        //if (should_jump && Time.time > to_allow_juming)
        //{
        //    jump();
        //    to_allow_juming = Time.time + 0.4f;
        //}

        if (should_jump)
        {
            jump();
        }

    }

    public void jump()
    {
        rb.AddForce(Vector2.up * impulseIntensity, ForceMode2D.Impulse);
    }

    private void InputSensor()
    {

        Debug.DrawRay(eye.position, Vector2.right, Color.red);
        Debug.DrawRay(eye.position, Vector2.up, Color.yellow);
        Debug.DrawRay(eye.position, Vector2.down, Color.yellow);

        RaycastHit2D hit;

        hit = Physics2D.Raycast(eye.transform.position, Vector2.right);
        if (hit.collider != null && hit.collider.CompareTag("obstacle"))
        {

            hsensor = 1;
        }

        if (hit.collider != null && hit.collider.CompareTag("middle"))
        {

            hsensor = 0;
        }

        hit = Physics2D.Raycast(eye.position, Vector2.up);
        if (hit.collider != null && (hit.collider.CompareTag("ground")))
        {

            upsensor = Mathf.Abs(hit.point.y - eye.transform.position.y) / 10;
        }

        hit = Physics2D.Raycast(eye.position, Vector2.down);
        if (hit.collider != null && (hit.collider.CompareTag("ground")))
        {

            downsensor = Mathf.Abs(hit.point.y - eye.transform.position.y) / 10;
        }

        hit = Physics2D.Raycast(eye.position, Vector2.up);
        if (hit.collider != null && hit.collider.CompareTag("obstacle"))
        {

            upBlock = 1;
        }

        if (hit.collider != null && hit.collider.CompareTag("obstacle"))
        {

            upBlock = 0;
        }

        hit = Physics2D.Raycast(eye.position, Vector2.down);
        if (hit.collider != null && hit.collider.CompareTag("obstacle"))
        {

            downBlock =1;
        }

        if (hit.collider != null && hit.collider.CompareTag("obstacle"))
        {

            downBlock = 0;
        }
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
            Destroy(gameObject);
        }
    }

    void Die()
    {
        if (alive)
        {
            alive = false;
            GeneticManager.instance.decreaseBird(brain, (float)getFitness(),index);
            //transform.eulerAngles += new Vector3(0, 0, 38);
            impulseIntensity = 0;
            parcoured_distance = transform.position.x - startPosition;
            surviving_time = Time.time - start_time;
        }
    }

    public double getFitness()
    {
        return 10*parcoured_distance + Mathf.Pow(2, number_of_blocks_parcourd)+surviving_time;
    }

    public void setNetwork(NNet net)
    {
        brain = net;
    }
}
