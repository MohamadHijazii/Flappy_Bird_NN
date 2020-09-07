using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesSpawner : MonoBehaviour
{
    public float time_to_spawn;
    public GameObject obstacle;

    float next_time_to_spawn;

    public List<GameObject> spawnedObstables;

    private void Start()
    {
        next_time_to_spawn = Time.time + time_to_spawn;
        spawnedObstables = new List<GameObject>();
        SpawnObstacle();
    }

    private void Update()
    {
        if(Time.time >= next_time_to_spawn)
        {
            SpawnObstacle();
            next_time_to_spawn = Time.time + time_to_spawn;
        }
    }

    void SpawnObstacle()
    {
        // [-1,2.75]    spawn between this 2 y
        float y = UnityEngine.Random.Range(-1, 2.75f);
        Vector3 toSpawn = new Vector3(transform.position.x, y);
        GameObject obs = Instantiate(obstacle,toSpawn,Quaternion.identity);
        spawnedObstables.Add(obs);

    }

    public void ClearObstacles()
    {
        foreach(var o in spawnedObstables)
        {
            Destroy(o.gameObject);
        }
    }
}
