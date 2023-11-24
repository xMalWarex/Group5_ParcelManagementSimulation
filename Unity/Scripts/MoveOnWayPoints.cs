using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOnWaypoints : MonoBehaviour
{
    [System.Serializable]
    public class Waypoint
    {
        public GameObject point;
        public bool shouldStop;
    }

    public List<Waypoint> waypoints;
    public float speed = 2;
    public float pauseDuration = 2f; // Adjust the pause duration as needed
    int index = 0;
    public bool isLoop = true;

    bool isPaused = false;
    float pauseTimer = 0f;

    void Update()
    {
        if (!isPaused)
        {
            Vector3 destination = waypoints[index].point.transform.position;
            Vector3 newPos = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
            transform.position = newPos;

            float distance = Vector3.Distance(transform.position, destination);
            if (distance <= 0.05f)
            {
                if (waypoints[index].shouldStop)
                {
                    // Start the pause only if the current waypoint has the shouldStop flag
                    isPaused = true;
                    pauseTimer = 0f;
                }

                if (index < waypoints.Count - 1)
                {
                    index++;
                }
                else
                {
                    if (isLoop)
                    {
                        index = 0;
                    }
                }
            }
        }
        else
        {
            // Increment the timer during the pause
            pauseTimer += Time.deltaTime;

            // Check if the pause duration is reached
            if (pauseTimer >= pauseDuration)
            {
                // End the pause
                isPaused = false;
            }
        }
    }
}