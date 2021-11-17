/// <summary>
/// Author: Gavriel Miles (hey)
/// This class is meant to be put on the obstacle object around which mobile creatures are supposed to maneuver.
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private float radius;

    public float Radius
    {
        get { return radius; }
        set { radius = value; }
    }


    void Start ()
    {
        radius = 2;
	}
	
	void Update ()
    {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        transform.position = new Vector3(Random.Range(-18f, 18f), 1.5f, Random.Range(-18f, 18f));
    }
}
