/// <summary>
/// Author : Gavriel Miles
/// This is the Human class, a child of the Vehicle class. 
/// This class handles some of the human's specific values and behaviors which different from the zombie behavior.
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : Vehicle
{
    new void Start()
    {
        base.Start();

        mass = 1f;
        maxSpeed = 10f;
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
        // Determine zombie fleeing from, or if none then wander
        int zombCounter = 0;    // Represents how many zombies within proximity
        foreach (GameObject zombie in zombies)
        {
            if (zombie != null && zombies.Count > 0)
            {
                Vector3 distance = transform.position - zombie.transform.position;
                if (Mathf.Abs(distance.magnitude) < 15)
                {
                    zombCounter++;
                    ultimateForce += Evade(zombie)/zombCounter;     // Scales inversely with number of zombies so flee is not overpowering
                }
            }
        }
        if (zombCounter == 0)
        {
            ultimateForce += Wander();
        }

        // Stay in bounds!
        if (OutOfBounds())
        {
            Vector3 goBackToCenter = Seek(new Vector3(0, .5f, 0))*800;        // arbitrarily large value
            ultimateForce += goBackToCenter;
        }

        // Take into account obstacles now!!
        foreach (GameObject obstacle in obstacles)
        {
            ultimateForce += ObstacleAvoidance(obstacle);
        }

        // Separation
        foreach (GameObject neighbor in humans)
        {
            ultimateForce += Separation(neighbor);
        }

        // Scale, apply, and then reset ultimate force
        ultimateForce = ultimateForce.normalized * maxSpeed;
        ApplyForce(new Vector3(ultimateForce.x, 0, ultimateForce.z));
        ultimateForce = Vector3.zero;
    }

    /// <summary>
    /// Manually add in 'debug lines' to represent the following:
    ///     forward & right vectors
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
            GL.Vertex(vehiclePosition + (this.transform.right) * 2);
            GL.End();

            // Future Position
            purple.SetPass(0);
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
