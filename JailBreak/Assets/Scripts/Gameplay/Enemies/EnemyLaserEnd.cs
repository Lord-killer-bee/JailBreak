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
    float waitUnits = 0;

    #region Base methods

    private void Start()
    {
        if (TestController.testEnvironment)
            Initialize();
    }

    public void Initialize()
    {
        objectInitialized = true;

        currentPathIndex = 0;
        targetPathIndex = 0;

        moveDirection = 1;

        startWaiting = true;
        waitUnits = waitTime;

        transform.position = waypoints[currentPathIndex];
        targetPosition = waypoints[targetPathIndex];

        transform.up = (waypoints[currentPathIndex + 1] - transform.position).normalized;
    }

    private void LateUpdate()
    {
        if (objectInitialized)
        {
            if (currentPathIndex != targetPathIndex && !startWaiting)
            {
                transform.position = targetPosition;
            }

            if (Vector2.Distance(transform.position, targetPosition) <= 0.1f && !startWaiting)
            {
                startWaiting = true;
                waitUnits = waitTime;

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
                if (GameTimer.GameTicked)
                {
                    if (startWaiting)
                    {
                        waitUnits--;

                        if (waitUnits <= 0)
                            startWaiting = false;
                    }
                }

                if(!startWaiting)
                {
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

                    //transform.up = (targetPosition - transform.position).normalized;
                }
            }
        }
    }

    public void ResetLaser()
    {
        currentPathIndex = 0;
        targetPathIndex = 0;

        moveDirection = 1;

        startWaiting = true;
        waitUnits = waitTime;

        transform.position = waypoints[currentPathIndex];
        targetPosition = waypoints[targetPathIndex];

        transform.up = (waypoints[currentPathIndex + 1] - transform.position).normalized;
    }

    #endregion

    #region Getters and setters

    public Vector3[] GetWayPoints()
    {
        return waypoints;
    }

    public void SetWayPoints(Vector3[] waypoints)
    {
        this.waypoints = waypoints;
    }

    #endregion
}
