using Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLaser : MonoBehaviour
{
    [SerializeField] private Transform[] laserEnds;
    //[SerializeField] private LaserEndMotionData[] motionData = new LaserEndMotionData[2];

    private Vector3[] laserEndPositions = new Vector3[2];
    private bool playerDetected = false;
    private bool objectInitialized = true;


    void Start()
    {
        if (TestLevelManager.testEnvironment)
            Initialize();
    }

    public void Initialize()
    {
        laserEnds[0].GetComponent<EnemyLaserEnd>().Initialize();
        laserEnds[1].GetComponent<EnemyLaserEnd>().Initialize();
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

    private void Update()
    {
        LineRenderer line = GetComponent<LineRenderer>();
        line.positionCount = 2;
        line.SetPositions(GetLaserEndPositions());
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == GameConsts.PLAYER_TAG)
        {
            GameEventManager.Instance.TriggerSyncEvent(new PlayerDetectedEvent());
            playerDetected = true;
        }
    }

    private void OnPathDrawingComplete(PathDrawingCompleteEvent e)
    {
        if (TestLevelManager.testEnvironment)
        {
            laserEnds[0].GetComponent<EnemyLaserEnd>().ResetLaser();
            laserEnds[1].GetComponent<EnemyLaserEnd>().ResetLaser();
        }
    }

    private void OnGameStateChanged(GameStateChangedEvent e)
    {
        if (e.stateType == GameStateType.SimulateLevel)
        {
            laserEnds[0].GetComponent<EnemyLaserEnd>().ResetLaser();
            laserEnds[1].GetComponent<EnemyLaserEnd>().ResetLaser();
        }
    }

    #region Getters and Setters

    public Vector3[] GetLaserEndPositions()
    {
        for (int i = 0; i < laserEndPositions.Length; i++)
        {
            laserEndPositions[i] = laserEnds[i].position;
        }

        return laserEndPositions;
    }

    public void SetLaserEndPositions(Vector3[] laserEndPositions)
    {
        this.laserEndPositions = laserEndPositions;

        for (int i = 0; i < laserEndPositions.Length; i++)
        {
            laserEnds[i].position = laserEndPositions[i];
        }
    }

    #endregion

}

[System.Serializable]
public class EnemyLaserDataUnit
{
    public Vector3[] laserEnds;

    public Vector2 position;
    public Quaternion rotation;
    public Vector3 scale;
}

[System.Serializable]
public class LaserEndMotionData
{
    public LaserMoveDirection moveDirection;
    public float moveSpeed;
    public float moveDistance;
}