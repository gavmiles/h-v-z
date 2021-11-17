/// <summary>
/// Author: Gavriel Miles (hey)
/// This class is meant to instantiate the objects that are going to be around the level.
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour {

    public GameObject[] obstacleRefs;
    protected List<GameObject> obstacles;

	void Start ()
    {
        obstacles = new List<GameObject>();
        SpawnObstacles();
	}
	
	void Update ()
    {
		
	}

    void SpawnObstacles()
    {
        for (int i = 0; i < 8; i++)
        {
            GameObject newObs = Instantiate(obstacleRefs[Random.Range(0, 4)], new Vector3(Random.Range(-18f, 18f), 1.4f, Random.Range(-18f, 18f)), Quaternion.identity);
            obstacles.Add(newObs);
        }
    }
}
