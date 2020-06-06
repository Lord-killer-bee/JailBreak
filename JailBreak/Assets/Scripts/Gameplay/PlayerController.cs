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
    public List<int> plottedTileIDs;
    bool startTraversal = false;
    bool startWaiting = false;

    int currentPathIndex = 0;
    int targetPathIndex = 0;

    Vector2 targetPosition;

    DateTime delayStartTime;

    private void Start()
    {
        GameEventManager.Instance.AddListener<PathDrawingCompleteEvent>(OnPathDrawingComplete);
        GameEventManager.Instance.AddListener<GameStateChangedEvent>(OnGameStateChanged);
        GameEventManager.Instance.AddListener<PlayerDetectedEvent>(OnPlayerDetected);
    }

    private void OnDestroy()
    {
        GameEventManager.Instance.RemoveListener<PathDrawingCompleteEvent>(OnPathDrawingComplete);
        GameEventManager.Instance.RemoveListener<GameStateChangedEvent>(OnGameStateChanged);
        GameEventManager.Instance.RemoveListener<PlayerDetectedEvent>(OnPlayerDetected);
    }

    private void Update()
    {
        if (startTraversal)
        {
            if (currentPathIndex < targetPathIndex)
            {
                //transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                transform.position = targetPosition;
            }

            if (Vector3.Distance(transform.position, targetPosition) == 0 && !startWaiting)
            {
                delayStartTime = DateTime.Now;
                currentPathIndex++;
                startWaiting = true;

                if(!TestLevelManager.testEnvironment)
                    GameEventManager.Instance.TriggerAsyncEvent(new PlayerMovedToTileEvent(plottedTileIDs[currentPathIndex]));
            }

            if (startWaiting)
            {
                if ((DateTime.Now - delayStartTime).TotalMilliseconds >= moveWaitTime * 1000)
                {
                    targetPathIndex++;
                    startWaiting = false;

                    if (targetPathIndex < plottedPoints.Count)
                    {
                        targetPosition = plottedPoints[targetPathIndex];

                    }
                }
            }

            if (currentPathIndex >= plottedPoints.Count - 1)
            {
                startTraversal = false;
            }
        }
    }

    private void StartTraversal()
    {
        startTraversal = true;
        currentPathIndex = 0;
        targetPathIndex = 1;

        targetPosition = plottedPoints[targetPathIndex];
    }

    private void OnPathDrawingComplete(PathDrawingCompleteEvent e)
    {
        plottedPoints = e.plottedPoints;
        plottedTileIDs = e.plottedTileIDs;

        if (TestLevelManager.testEnvironment)
        {
            StartTraversal();
        }
    }

    private void OnGameStateChanged(GameStateChangedEvent e)
    {
        if (e.stateType == GameStateType.SimulateLevel)
            StartTraversal();
    }

    private void OnPlayerDetected(PlayerDetectedEvent e)
    {
        startTraversal = false;
    }

}
