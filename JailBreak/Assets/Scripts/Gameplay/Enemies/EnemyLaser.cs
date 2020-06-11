using Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLaser : MonoBehaviour
{
    [SerializeField] private Transform[] laserEnds;

    private Vector3[] laserEndPositions = new Vector3[2];
    private bool objectInitialized = true;

    #region Base methods

    void Start()
    {
        if (TestController.testEnvironment)
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
        GameEventManager.Instance.AddListener<SimulateCountDownEnded>(OnSimulationCoundownEnded);
        GameEventManager.Instance.AddListener<ResetPlotterEvent>(OnResetPlotter);
        GameEventManager.Instance.AddListener<PlayerDetectedEvent>(OnPlayerDetected);
        GameEventManager.Instance.AddListener<RestartLevelEvent>(OnLevelRestart);
    }

    private void OnDisable()
    {
        GameEventManager.Instance.RemoveListener<PathDrawingCompleteEvent>(OnPathDrawingComplete);
        GameEventManager.Instance.RemoveListener<GameStateChangedEvent>(OnGameStateChanged);
        GameEventManager.Instance.RemoveListener<SimulateCountDownEnded>(OnSimulationCoundownEnded);
        GameEventManager.Instance.RemoveListener<ResetPlotterEvent>(OnResetPlotter);
        GameEventManager.Instance.RemoveListener<PlayerDetectedEvent>(OnPlayerDetected);
        GameEventManager.Instance.RemoveListener<RestartLevelEvent>(OnLevelRestart);
    }

    private void OnLevelRestart(RestartLevelEvent e)
    {
        laserEnds[0].GetComponent<EnemyLaserEnd>().Initialize();
        laserEnds[1].GetComponent<EnemyLaserEnd>().Initialize();
    }

    private void OnPlayerDetected(PlayerDetectedEvent e)
    {
        laserEnds[0].GetComponent<EnemyLaserEnd>().StopLaser();
        laserEnds[1].GetComponent<EnemyLaserEnd>().StopLaser();
    }

    private void LateUpdate()
    {
        laserEnds[0].GetComponent<EnemyLaserEnd>().LateUpdateForced();
        laserEnds[1].GetComponent<EnemyLaserEnd>().LateUpdateForced();

        LineRenderer line = GetComponent<LineRenderer>();
        line.positionCount = 2;
        line.SetPositions(GetLaserEndPositions());

        EdgeCollider2D collider = GetComponent<EdgeCollider2D>();
        collider.points = GetLaserEndLocalPositons();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == GameConsts.PLAYER_TAG)
        {
            Debug.Log(gameObject.name);
            GameEventManager.Instance.TriggerSyncEvent(new PlayerDetectedEvent());

        }
    }

    //private void OnDestroy()
    //{
    //    laserEnds[0] = null;
    //    laserEnds[1] = null;
    //}

    #endregion

    #region Event listeners

    private void OnPathDrawingComplete(PathDrawingCompleteEvent e)
    {
        if (TestController.testEnvironment)
        {
            laserEnds[0].GetComponent<EnemyLaserEnd>().ResetLaser();
            laserEnds[1].GetComponent<EnemyLaserEnd>().ResetLaser();
        }
    }

    private void OnGameStateChanged(GameStateChangedEvent e)
    {
        if (e.stateType == GameStateType.Plotting)
        {
            laserEnds[0].GetComponent<EnemyLaserEnd>().ResetLaser();
            laserEnds[1].GetComponent<EnemyLaserEnd>().ResetLaser();
        }
        else if (e.stateType == GameStateType.SimulateLevel)
        {
            laserEnds[0].GetComponent<EnemyLaserEnd>().PauseLaser();
            laserEnds[1].GetComponent<EnemyLaserEnd>().PauseLaser();
        }
    }

    private void OnSimulationCoundownEnded(SimulateCountDownEnded e)
    {
        laserEnds[0].GetComponent<EnemyLaserEnd>().ResetLaser();
        laserEnds[1].GetComponent<EnemyLaserEnd>().ResetLaser();

        laserEnds[0].GetComponent<EnemyLaserEnd>().ResumeLaser();
        laserEnds[1].GetComponent<EnemyLaserEnd>().ResumeLaser();
    }

    private void OnResetPlotter(ResetPlotterEvent e)
    {
        laserEnds[0].GetComponent<EnemyLaserEnd>().ResetLaser();
        laserEnds[1].GetComponent<EnemyLaserEnd>().ResetLaser();
    }

    #endregion

    #region Getters and Setters

    public Vector3[] GetLaserEndPositions()
    {
        for (int i = 0; i < laserEndPositions.Length; i++)
        {
            laserEndPositions[i] = laserEnds[i].position;
        }

        return laserEndPositions;
    }

    public Vector2[] GetLaserEndLocalPositons()
    {
        Vector2[] result = new Vector2[2];

        for (int i = 0; i < laserEndPositions.Length; i++)
        {
            result[i] = laserEnds[i].localPosition;
        }

        return result;
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
