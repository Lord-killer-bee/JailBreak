using Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrollingEnemy : MonoBehaviour
{
    [SerializeField] private Vector3[] waypoints;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float detectionTileRange;

    private bool objectInitialized = true;
    private bool playerDetected = false;
    private int currentPathIndex = 0;
    private int targetPathIndex = 0;
    
    private Vector2 targetPosition;
    private int moveDirection = 1;

    private void Start()
    {
        objectInitialized = true;

        currentPathIndex = 0;
        targetPathIndex = 1;

        moveDirection = 1;

        transform.position = waypoints[currentPathIndex];
        targetPosition = waypoints[targetPathIndex];

        transform.up = (targetPosition - (Vector2)transform.position).normalized;
    }

    private void Update()
    {
        if (objectInitialized)
        {
            if (currentPathIndex != targetPathIndex)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            }

            if (Vector3.Distance(transform.position, targetPosition) == 0)
            {
                if (moveDirection == 1)
                {
                    currentPathIndex++;
                    targetPathIndex++;
                }
                else
                {
                    currentPathIndex--;
                    targetPathIndex--;
                }

                if (currentPathIndex == waypoints.Length - 1 || currentPathIndex == 0)
                {
                    moveDirection *= -1;

                    if(moveDirection == 1)
                    {
                        currentPathIndex = 0;
                        targetPathIndex = 1;
                    }
                    else
                    {
                        currentPathIndex = waypoints.Length - 1;
                        targetPathIndex = currentPathIndex - 1;
                    }
                }

                if (targetPathIndex < waypoints.Length && targetPathIndex >= 0)
                    targetPosition = waypoints[targetPathIndex];

                transform.up = (targetPosition - (Vector2)transform.position).normalized;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.tag == GameConsts.PLAYER_TAG)
        {
            GameEventManager.Instance.TriggerSyncEvent(new PlayerDetectedEvent());
            playerDetected = true;
        }
    }

    public Vector3[] GetWayPoints()
    {
        return waypoints;
    }

    public float GetDetectionTileRange()
    {
        return detectionTileRange;
    }
}
