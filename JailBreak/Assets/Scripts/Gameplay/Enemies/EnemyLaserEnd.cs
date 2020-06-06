using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLaserEnd : MonoBehaviour
{
    [SerializeField] private Vector3[] waypoints;
    [SerializeField] private float waitTime;

    private bool objectInitialized = true;
    private int currentPathIndex = 0;
    private int targetPathIndex = 0;

    private Vector3 targetPosition;
    private int moveDirection = 1;
    private bool startWaiting = false;
    private DateTime delayStartTime;

    private void Start()
    {
        if (TestLevelManager.testEnvironment)
            Initialize();
    }

    public void Initialize()
    {
        objectInitialized = true;

        currentPathIndex = 0;
        targetPathIndex = 1;

        moveDirection = 1;

        transform.position = waypoints[currentPathIndex];
        targetPosition = waypoints[targetPathIndex];

        transform.up = (targetPosition - transform.position).normalized;
    }

    private void Update()
    {
        if (objectInitialized)
        {
            if (currentPathIndex != targetPathIndex)
            {
                //transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                transform.position = targetPosition;
            }

            if (Vector3.Distance(transform.position, targetPosition) == 0 && !startWaiting)
            {
                startWaiting = true;
                delayStartTime = DateTime.Now;

                if (moveDirection == 1)
                {
                    currentPathIndex++;
                }
                else
                {
                    currentPathIndex--;
                }

                if (currentPathIndex == waypoints.Length - 1 || currentPathIndex == 0)
                {
                    moveDirection *= -1;

                    if (moveDirection == 1)
                    {
                        currentPathIndex = 0;
                    }
                    else
                    {
                        currentPathIndex = waypoints.Length - 1;
                    }
                }
            }

            if (startWaiting)
            {
                if ((DateTime.Now - delayStartTime).TotalMilliseconds >= waitTime * 1000)
                {
                    startWaiting = false;

                    if (moveDirection == 1)
                    {
                        targetPathIndex++;
                    }
                    else
                    {
                        targetPathIndex--;
                    }

                    if (currentPathIndex == waypoints.Length - 1 || currentPathIndex == 0)
                    {
                        if (moveDirection == 1)
                        {
                            targetPathIndex = 1;
                        }
                        else
                        {
                            targetPathIndex = currentPathIndex - 1;
                        }
                    }

                    if (targetPathIndex < waypoints.Length && targetPathIndex >= 0)
                        targetPosition = waypoints[targetPathIndex];

                    transform.up = (targetPosition - transform.position).normalized;
                }
            }
        }
    }

    public Vector3[] GetWayPoints()
    {
        return waypoints;
    }

    public void SetWayPoints(Vector3[] waypoints)
    {
        this.waypoints = waypoints;
    }
}
