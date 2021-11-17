/// <summary>
/// Author : Gavriel Miles
/// This is the Zombie class, a child of the Vehicle class. 
/// This class handles some of the zombie's specific values and behaviors which different from the human behavior.
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Vehicle
{
    protected Vector3 targetLoc;

    new void Start()
    {
        base.Start();

        mass = 1.3f;
        maxSpeed = 7f;
    }

    new void Update()
    {
        base.Update();
    }

    /// <summary>
    /// Total all of the steering forces into one vector and apply them all at once.
    /// Overrides an abstract method from the Vehicle class.
    /// </summary>
    public override void CalcSteeringForces()
    {
        int index = 0;              // Will track target human's index in list of humans
        float prevDist = 100f;      // Starting value for keeping track of closest human

        // Loop through humans, find the one closest to the zombie, store index
        foreach (GameObject human in humans)
        {
            if (human != null)
            {
                Vector3 distance = transform.position - human.transform.position;
                if (Mathf.Abs(distance.magnitude) < prevDist)
                {
                    prevDist = Mathf.Abs(distance.magnitude);
                    index = humans.IndexOf(human);
                }
            }
        }       // By the end of this loop, we will have the index of the human closest to the zombie

        // Add seek steering force, but if no humans then wander
        if (index != -1 && humans.Count > 0)
        {
            ultimateForce += Pursue(humans[index])/2;
            targetLoc = humans[index].transform.position;
        }
        else    
        {
            targetLoc = Vector3.zero;
            ultimateForce += Wander();
        }

        // Stay in bounds!
        if (OutOfBounds())
        {
            ultimateForce += Seek(Vector3.zero);
        }

        // Obstacle Avoidance
        foreach (GameObject obstacle in obstacles)
        {
            ultimateForce += ObstacleAvoidance(obstacle);
        }

        // Separation
        foreach (GameObject neighbor in zombies)
        {
            ultimateForce += Separation(neighbor);
        }

        ultimateForce = ultimateForce.normalized * maxSpeed;

        ApplyForce(new Vector3(ultimateForce.x, 0, ultimateForce.z));        // do not add any force to y, do not want to change the height at all
    }

    /// <summary>
    /// Manually add in 'debug lines' to represent the following:
    ///     forward & right vectors
    ///     the targetted human
    ///     future position
    /// </summary>
    public void OnRenderObject()
    {
        if (showLines)
        {        
        
        // Forward Vector
        green.SetPass(0);               // activate material to be used
        GL.Begin(GL.LINES);             // begin to draw lines
        GL.Vertex(vehiclePosition);     // first endpoint
        GL.Vertex(vehiclePosition + direction);           // second endpoint
        GL.End();

        // Right Vector
        blue.SetPass(0);
        GL.Begin(GL.LINES);
        GL.Vertex(vehiclePosition);
        GL.Vertex(vehiclePosition + (this.transform.right)*2);
        GL.End();

        // Vector between targets
        if (targetLoc != Vector3.zero)
        {
            black.SetPass(0);
            GL.Begin(GL.LINES);
            GL.Vertex(transform.position);
            GL.Vertex(targetLoc);
            GL.End();
        }

        // Future Position
        red.SetPass(0);
        GL.Begin(GL.LINES);

        Vector3 futurePos = transform.position + (velocity.normalized * 2);
        // line one-- going along the x
            GL.Vertex(new Vector3(futurePos.x - .3f, futurePos.y, futurePos.z));
            GL.Vertex(new Vector3(futurePos.x + .3f, futurePos.y, futurePos.z));
        // line two-- going along the z
            GL.Vertex(new Vector3(futurePos.x, futurePos.y, futurePos.z - .3f));
            GL.Vertex(new Vector3(futurePos.x, futurePos.y, futurePos.z + .3f));
            GL.End();
        }

    }

}
