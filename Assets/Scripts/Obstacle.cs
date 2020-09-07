using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float speed = 1;

    private void Update()
    {
        transform.Translate(Vector3.left *speed* Time.deltaTime);
        if(transform.position.x < -10)
        {
            Destroy(gameObject);
        }
    }
}
