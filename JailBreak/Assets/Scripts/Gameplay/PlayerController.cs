using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;
using System;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float moveWaitTime;

    private List<Vector2> plottedPoints;
    bool startTraversal = false;
    bool startWaiting = false;

    int currentPathIndex = 0;
    int targetPathIndex = 0;

    Vector2 targetPosition;

    DateTime delayStartTime;

    private void Start()
    {
        GameEventManager.Instance.AddListener<PathDrawingCompleteEvent>(OnPathDrawingComplete);
    }

    private void OnDestroy()
    {
        GameEventManager.Instance.RemoveListener<PathDrawingCompleteEvent>(OnPathDrawingComplete);
    }

    private void Update()
    {
        if (startTraversal)
        {
            if(currentPathIndex < targetPathIndex)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            }

            if(Vector3.Distance(transform.position, targetPosition) == 0 && !startWaiting)
            {
                delayStartTime = DateTime.Now;
                currentPathIndex++;
                startWaiting = true;
            }

            if (startWaiting)
            {
                Debug.Log((DateTime.Now - delayStartTime).TotalMilliseconds / 1000f);

                if ((DateTime.Now - delayStartTime).TotalMilliseconds >= moveWaitTime * 1000)
                {
                    targetPathIndex++;
                    startWaiting = false;

                    if (targetPathIndex < plottedPoints.Count)
                        targetPosition = plottedPoints[targetPathIndex];
                }
            }

            if (currentPathIndex >= plottedPoints.Count)
            {
                startTraversal = false;
            }
        }
    }

    private void OnPathDrawingComplete(PathDrawingCompleteEvent e)
    {
        plottedPoints = e.plottedPoints;

        StartTraversal();
    }

    private void StartTraversal()
    {
        startTraversal = true;
        currentPathIndex = 0;
        targetPathIndex = 1;

        targetPosition = plottedPoints[targetPathIndex];
    }
}
