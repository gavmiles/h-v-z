/// <summary>
/// Author: Gavriel Miles (yo)
/// This purpose of this class is to go onto a Vehicle Manager object and manage the various humans and zombies. 
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;      // using linq so i can turn an rray into a list

public class VehicleManager : MonoBehaviour
{
    public GameObject humanRef;
    public GameObject zombieRef;
    public GameObject parkRef;

    public List<GameObject> humans;
    public List<GameObject> zombies;
    public List<GameObject> obstacles;

    private bool showDebugLines;

    void Start()
    {
        // Instantiate lists
        humans = new List<GameObject>();
        zombies = new List<GameObject>();
        obstacles = new List<GameObject>();

        // Spawn 4 humans & 1 zombie
        for (int i = 0; i < 4; i++)
        {
            SpawnHuman();
        }
        SpawnZombie();
    }

    void Update()
    {
        {
            if (zombies.Count > 20)
            {
                CapZombies();
            }
        }   // Cap zombies at maximum of three

        {
            if (Input.GetKeyDown("space"))
            {
                showDebugLines = !showDebugLines;
                ChangeDebugLineVisibility();
            }
        }   // Handle drawing debug lines

        {
            zombies.Clear();
            zombies = GameObject.FindGameObjectsWithTag("Zombie").ToList();

            humans.Clear();
            humans = GameObject.FindGameObjectsWithTag("Human").ToList();

            obstacles.Clear();
            obstacles = GameObject.FindGameObjectsWithTag("Obstacle").ToList();
        }   // Update Local Data Structures

        {
            // For humans
            foreach (GameObject human in humans)
            {
                human.GetComponent<Human>().UpdateLists(obstacles, humans, zombies);
            }

            // For zombies
            foreach (GameObject zombie in zombies)
            {
                zombie.GetComponent<Zombie>().UpdateLists(obstacles, humans, zombies);
            }
        }   // Update lists in all vehicles

        {
            int humanIndex = -1;                                    // use a number that will never be a valid index
            foreach (GameObject human in humans)
            {
                foreach (GameObject zombie in zombies)
                {
                    Vector3 distance = human.transform.position - zombie.transform.position;        // calculate distance between each human and zombie
                    if (Mathf.Abs(distance.magnitude) <= 1f)
                    {
                        humanIndex = humans.IndexOf(human);                                         // store index of colliding human
                    }
                }
            }

            // If there was found to be a human colliding with a zombie
            if (humanIndex != -1)                                                                   // aka if there was a human found to be colliding
            {
                GameObject tempHuman = humans[humanIndex];                                  // Local reference
                Vector3 humanLoc = tempHuman.transform.position;                            // store relative location
                float humanRot = tempHuman.GetComponent<Vehicle>().AngleOfRotation;         // and rotation for when we spawn zombie
                humans.RemoveAt(humanIndex);
                Destroy(tempHuman);

                GameObject newZombie = Instantiate(zombieRef, humanLoc, Quaternion.identity);
                newZombie.GetComponent<Vehicle>().AngleOfRotation = humanRot;                       // manually set its rotation property
                zombies.Add(newZombie);

                for (int i = 0; i < zombies.Count; i++)
                {
                    GameObject zombie = zombies[i];
                    zombie.GetComponent<Zombie>().UpdateLists(obstacles, humans, zombies);
                }

                for (int i = 0; i < humans.Count; i++)
                {
                    GameObject human = humans[i];
                    human.GetComponent<Human>().UpdateLists(obstacles, humans, zombies);
                }
            }
        }   // Check for collisions between humans and zombies
    }

    /// <summary>
    /// Instantiate a number of humans across the park.
    /// </summary>
    public void SpawnHuman()
    {
        if (humans.Count <= 10)
        {
            GameObject human = Instantiate(humanRef, new Vector3(Random.Range(-18f, 18f), 0.5f, Random.Range(-18f, 18f)), Quaternion.identity);
            humans.Add(human);
        }
        else
        {
            Debug.Log("Maximum # of humans is 10, please no more");
        }
    }

    /// <summary>
    /// Instantiate a number of zombies across the park.
    /// </summary>
    public void SpawnZombie()
    {
        if (zombies.Count <= 10)
        {
            GameObject zombie = Instantiate(zombieRef, new Vector3(Random.Range(-18f, 18f), 0.5f, Random.Range(-18f, 18f)), Quaternion.identity);
            zombies.Add(zombie);
        }
        else
        {
            Debug.Log("Maximum # of zombies is 10, please no more");
        }
    }

    /// <summary>
    /// When called, goes through every human and zombie & changes
    /// the variable to show if things are displayed or not.
    /// </summary>
    void ChangeDebugLineVisibility()
    {
        foreach (GameObject human in humans)
        {
            human.GetComponent<Vehicle>().showLines = showDebugLines;
        }

        foreach (GameObject zombie in zombies)
        {
            zombie.GetComponent<Vehicle>().showLines = showDebugLines;
        }
    }

    /// <summary>
    /// Limit maximum number of zombies to 20
    /// </summary>
    void CapZombies()
    {
        GameObject byebyeZombie = zombies[1];
        zombies.RemoveAt(1);
        Destroy(byebyeZombie);
        Debug.Log("bye bye zombie");
    }
}
