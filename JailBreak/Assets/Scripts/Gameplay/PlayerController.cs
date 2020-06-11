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
    float waitUnits = 0;

    int currentPathIndex = 0;
    int targetPathIndex = 0;

    Vector2 targetPosition;

    private void Start()
    {
        GameEventManager.Instance.AddListener<PathDrawingCompleteEvent>(OnPathDrawingComplete);
        GameEventManager.Instance.AddListener<GameStateChangedEvent>(OnGameStateChanged);
        GameEventManager.Instance.AddListener<SimulateCountDownEnded>(OnSimulateCountdownEnded);
        GameEventManager.Instance.AddListener<PlayerDetectedEvent>(OnPlayerDetected);
    }

    private void OnDestroy()
    {
        GameEventManager.Instance.RemoveListener<PathDrawingCompleteEvent>(OnPathDrawingComplete);
        GameEventManager.Instance.RemoveListener<GameStateChangedEvent>(OnGameStateChanged);
        GameEventManager.Instance.RemoveListener<SimulateCountDownEnded>(OnSimulateCountdownEnded);
        GameEventManager.Instance.RemoveListener<PlayerDetectedEvent>(OnPlayerDetected);
    }

    private void LateUpdate()
    {
        if (startTraversal)
        {
            if (currentPathIndex < targetPathIndex && !startWaiting)
            {
                transform.up = targetPosition - (Vector2)transform.position;
                transform.position = targetPosition;
            }

            if (Vector3.Distance(transform.position, targetPosition) == 0 && !startWaiting)
            {
                currentPathIndex++;
                startWaiting = true;

                if(!TestController.testEnvironment)
                    GameEventManager.Instance.TriggerAsyncEvent(new PlayerMovedToTileEvent(plottedTileIDs[currentPathIndex]));
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
        targetPathIndex = 0;

        startWaiting = true;
        waitUnits = moveWaitTime;

        targetPosition = plottedPoints[targetPathIndex];
    }

    private void OnPathDrawingComplete(PathDrawingCompleteEvent e)
    {
        plottedPoints = e.plottedPoints;
        plottedTileIDs = e.plottedTileIDs;

        if (TestController.testEnvironment)
        {
            StartTraversal();
        }
    }

    private void OnGameStateChanged(GameStateChangedEvent e)
    {
        if (e.stateType == GameStateType.SimulateLevel)
        {
            //TODO: OPTIONAL DO A SMOOTH REWIND
        }
    }

    private void OnSimulateCountdownEnded(SimulateCountDownEnded e)
    {
        StartTraversal();
    }

    private void OnPlayerDetected(PlayerDetectedEvent e)
    {
        startTraversal = false;
    }

}
