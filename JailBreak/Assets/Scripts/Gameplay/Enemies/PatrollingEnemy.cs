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
    [SerializeField] private float waitTime;

    private bool objectInitialized = true;
    private bool playerDetected = false;
    private int currentPathIndex = 0;
    private int targetPathIndex = 0;
    
    private Vector2 targetPosition;
    private int moveDirection = 1;
    private bool startWaiting = false;
    private DateTime delayStartTime;
    private bool reachedEnd = false;
    private float waitUnits = 0;

    private void Start()
    {
        //if (TestLevelManager.testEnvironment)
            Initialize();
    }

    private void OnEnable()
    {
        GameEventManager.Instance.AddListener<PathDrawingCompleteEvent>(OnPathDrawingComplete);
        GameEventManager.Instance.AddListener<GameStateChangedEvent>(OnGameStateChanged);
    }

    private void OnDisable()
    {
        GameEventManager.Instance.RemoveListener<PathDrawingCompleteEvent>(OnPathDrawingComplete);
        GameEventManager.Instance.RemoveListener<GameStateChangedEvent>(OnGameStateChanged);
    }

    public void Initialize()
    {
        objectInitialized = true;

        currentPathIndex = 0;
        targetPathIndex = 0;

        startWaiting = true;
        waitUnits = waitTime;

        moveDirection = 1;

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
                    reachedEnd = true;

                    if (moveDirection == 1)
                    {
                        currentPathIndex = 0;
                    }
                    else
                    {
                        currentPathIndex = waypoints.Length - 1;
                    }
                }
                else
                    reachedEnd = false;
            }

            if (startWaiting)
            {
                //if ((DateTime.Now - delayStartTime).TotalMilliseconds >= waitTime * 1000)
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
                    if (reachedEnd)
                    {
                        startWaiting = true;
                        delayStartTime = DateTime.Now;
                        reachedEnd = false;

                        if(moveDirection == 1)
                        {
                            transform.up = ((Vector2)waypoints[currentPathIndex + 1] - (Vector2)transform.position).normalized;
                        }
                        else
                        {
                            transform.up = ((Vector2)waypoints[currentPathIndex - 1] - (Vector2)transform.position).normalized;
                        }

                        return;
                    }

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

                    transform.up = (targetPosition - (Vector2)transform.position).normalized;
                }
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

    private void OnPathDrawingComplete(PathDrawingCompleteEvent e)
    {
        if (TestLevelManager.testEnvironment)
            ResetEnemy();
    }

    private void OnGameStateChanged(GameStateChangedEvent e)
    {
        if (e.stateType == GameStateType.SimulateLevel)
            ResetEnemy();
    }

    private void ResetEnemy()
    {
        currentPathIndex = 0;
        targetPathIndex = 0;

        startWaiting = true;
        waitUnits = waitTime;

        moveDirection = 1;

        transform.position = waypoints[currentPathIndex];
        targetPosition = waypoints[targetPathIndex];

        transform.up = (waypoints[currentPathIndex + 1] - transform.position).normalized;
    }

    #region Getters and Setters

    public Vector3[] GetWayPoints()
    {
        return waypoints;
    }

    public float GetMoveSpeed()
    {
        return moveSpeed;
    }

    public float GetDetectionTileRange()
    {
        return detectionTileRange;
    }

    public void SetWayPoints(Vector3[] waypoints)
    {
        this.waypoints = waypoints;
    }

    public void SetMoveSpeed(float moveSpeed)
    {
        this.moveSpeed = moveSpeed;
    }

    public void SetDetectionTileRange(float detectionTileRange)
    {
        this.detectionTileRange = detectionTileRange;
    }

    #endregion
}

[System.Serializable]
public class PatrollingEnemyDataUnit
{
    public Vector3[] waypoints;
    public float moveSpeed;
    public float detectionTileRange;

    public Vector2 position;
    public Quaternion rotation;
    public Vector3 scale;
}