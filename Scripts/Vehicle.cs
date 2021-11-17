/// <summary>
/// Author: Gavriel Miles (hi)
/// This is the parent class for anything that will end up moving-- both the human and zombie classes will inherit from this.
/// This class handles applying forces onto an object & its movement as a result of these forces.
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Vehicle : MonoBehaviour
{
    // Vectors & forces
    protected Vector3 vehiclePosition;
    private Vector3 acceleration;
    protected Vector3 direction;
    protected Vector3 velocity;
    protected Vector3 ultimateForce;

    // Lists of references
    public List<GameObject> obstacles;
    public List<GameObject> humans;
    public List<GameObject> zombies;

    // Values
    protected float mass;
    protected float maxSpeed;
    protected float radius;
    protected float angleOfRotation;
    public float AngleOfRotation
    {
        get { return angleOfRotation; }
        set { angleOfRotation = value; }
    }

    // Debug lines - materials & tracker for if they're shown
    public Material green;      // forward vector
    public Material blue;       // right vector
    public Material black;      // target to target
    public Material red;        // zombie's future position
    public Material purple;     // human's future position
    public bool showLines;

    protected void Start()
    {
        // Manually set local position to global position
        vehiclePosition = transform.position;

        // Randomize angle of rotaton while adjusting global rotation.
        angleOfRotation = Random.Range(-180f, 180f);
        transform.rotation = Quaternion.Euler(0, angleOfRotation, 0);

        radius = 0.5f;
        showLines = false;
        obstacles = new List<GameObject>();
        humans = new List<GameObject>();
        zombies = new List<GameObject>();
    }

    protected void Update()
    {
        CalcSteeringForces();
        Move();
        SetTransform();
    }

    /// <summary>
    /// Update the position, velocity, and acceleration of the vehicle
    /// </summary>
    public void Move()
    {
        velocity += acceleration * Time.deltaTime;                  // accelerate vehicle
        vehiclePosition += velocity * Time.deltaTime;               // apply velocity

        direction = velocity.normalized;                            // set direction
        acceleration = Vector3.zero;                                // reset accel for next frame
    }

    /// <summary>
    /// Updates global position & rotation based on local values
    /// </summary>
    public void SetTransform()
    {
        transform.position = vehiclePosition;

        angleOfRotation = Mathf.Atan2(direction.x, direction.z);
        angleOfRotation *= Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, angleOfRotation, 0);
    }

    /// <summary>
    /// Apply a force to the vehicle's acceleration vector.
    /// </summary>
    /// <param name="force"></param>
    public void ApplyForce(Vector3 force)
    {
        acceleration += force / mass;
    }

    /// <summary>
    /// Calculcate and return a steering force that will steer the vehicle towards seeking that GameObject.
    /// The functionality is meant primarily for zombies.
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public Vector3 Seek(GameObject target)
    {
        Vector3 desiredVelocity = target.transform.position - vehiclePosition;          // vector pointing from me (vehicle) to my target
        desiredVelocity = desiredVelocity.normalized * maxSpeed;                    // normalize & multiply by max speed
        Vector3 steerForce = desiredVelocity - velocity;                            // SF = DV - CV     
        return steerForce;
    }

    /// <summary>
    /// The same as the above, only that the parameter is different.
    /// This will be primarily used for steering vehicles back towards the center.
    /// </summary>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    public Vector3 Seek(Vector3 targetPos)
    {
        Vector3 desiredVelocity = targetPos - vehiclePosition;          // vector pointing from me (vehicle) to my target
        desiredVelocity = desiredVelocity.normalized * maxSpeed;        // normalize & multiply by max speed
        Vector3 steerForce = desiredVelocity - velocity;                // SF = DV - CV     
        return steerForce;
    }

    /// <summary>
    /// Calculate and return a steering force that will steer the vehicle away from the GameObject passed through.
    /// The functionality is primarily meant for humans.
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public Vector3 Flee(GameObject target)
    {
        Vector3 desiredVelocity = vehiclePosition - target.transform.position;          // vector pointing from target to me
        desiredVelocity = desiredVelocity.normalized * maxSpeed;                        // normalize & multiply by max speed
        Vector3 steerForce = desiredVelocity - velocity;                                // SF = DV - CV
        return steerForce;
    }

    /// <summary>
    /// Calculcate and return a steering force that steers the vehicle towards the target's future position (a.k.a. pursue)
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public Vector3 Pursue(GameObject target)
    {
        Vector3 futurePos = target.transform.position + (velocity.normalized * 2);
        Vector3 desiredVelocity = futurePos - vehiclePosition;          // vector pointing from me (vehicle) to my target after they move
        desiredVelocity = desiredVelocity.normalized * maxSpeed;        // normalize & multiply by max speed
        Vector3 steerForce = desiredVelocity - velocity;                // SF = DV - CV     
        return steerForce;
    }

    /// <summary>
    /// Calculate and return a steering force that steers the vehicle away from the target's future position (a.k.a. evade)
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public Vector3 Evade(GameObject target)
    {
        Vector3 futurePos = target.transform.position + (velocity.normalized * 2);
        Vector3 desiredVelocity = vehiclePosition - futurePos;          // vector pointing from target's future position to me
        desiredVelocity = desiredVelocity.normalized * maxSpeed;                        // normalize & multiply by max speed
        Vector3 steerForce = desiredVelocity - velocity;                                // SF = DV - CV
        return steerForce;
    }

    /// <summary>
    /// Calculate and return a steering force, which makes the vehicle looks like it's wandering.
    /// </summary>
    /// <returns></returns>
    public Vector3 Wander()
    {
        Vector3 future = transform.position + (velocity.normalized);
        Vector3 target = future + new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2));
        return Seek(target)/4;
    }

    /// <summary>
    /// Calculates and returns steering vector to steer away from obstacle if it is in the way
    /// </summary>
    /// <param name="obs"></param>
    /// <returns></returns>
    public Vector3 ObstacleAvoidance(GameObject obstacle)
    {
        Vector3 vecToCenter = obstacle.transform.position - transform.position;
        float dotForward = Vector3.Dot(vecToCenter, transform.forward);

        // Check if in front of vehicle
        if (dotForward < 0)
        {
            return Vector3.zero;        // negative value means it's not in front of you
        }

        // Check if close enough to vehicle
        if (vecToCenter.magnitude > 5)
        {
            return Vector3.zero;
        }

        // Check radii sum against distance on one axis
        float dotRight = Vector3.Dot(vecToCenter, transform.right);
        float radiiSum = obstacle.GetComponent<Obstacle>().Radius + radius;
        if (radiiSum < Mathf.Abs(dotRight))
        {
            return Vector3.zero;
        }

        // Determine direction
        Vector3 desiredVelocity;

        if (dotRight < 0)       // Left
        {
            desiredVelocity = transform.right * maxSpeed;
        }
        else                    // Right
        {
            desiredVelocity = -transform.right * maxSpeed;
        }

        // See what obstacle
        Debug.DrawLine(transform.position, obstacle.transform.position, Color.red);

        // Return steering force
        Vector3 steeringForce = desiredVelocity - velocity;
        return steeringForce;
    }

    /// <summary>
    /// Calculcate and return steering force to steer away from neighbor if within range
    /// </summary>
    /// <param name="neighbor"></param>
    /// <returns></returns>
    public Vector3 Separation(GameObject neighbor)
    {
        // Check for distance
        Vector3 vecToCenter = neighbor.transform.position - transform.position;
        if (vecToCenter.magnitude > 1.5f || vecToCenter.magnitude == 0)
        {
            return Vector3.zero;
        }

        // See which neighbor
        Debug.DrawLine(transform.position, neighbor.transform.position, Color.cyan);

        // Calculate & return steering force proportional to distance
        Vector3 desiredVelocity = Flee(neighbor);
        Vector3 steeringForce = desiredVelocity - velocity;
        return steeringForce / vecToCenter.magnitude;
    }

    /// <summary>
    /// Return true if the vehicle does not exceed 18 or -18 on either x or z, return false otherwise.
    /// </summary>
    /// <returns></returns>
    public bool OutOfBounds()
    {
        if (transform.position.x >= 18 || transform.position.x <= -18 || transform.position.z >= 18 || transform.position.z <= -18)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Allows the manager to refresh the vehicle's list of references to objects
    /// </summary>
    public void UpdateLists(List<GameObject> obs, List<GameObject> hums, List<GameObject> zombs)
    {
        obstacles.Clear();
        obstacles = obs;

        humans.Clear();
        humans = hums;

        zombies.Clear();
        zombies = zombs;
    }

    /// <summary>
    /// Calculate steering forces all at once (abstract because different for humans and zombies)
    /// </summary>
    public abstract void CalcSteeringForces();

}
