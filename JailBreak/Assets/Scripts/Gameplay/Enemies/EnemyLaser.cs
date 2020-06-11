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
        //if (e.stateType == GameStateType.SimulateLevel)
        //{
        //    laserEnds[0].GetComponent<EnemyLaserEnd>().ResetLaser();
        //    laserEnds[1].GetComponent<EnemyLaserEnd>().ResetLaser();
        //}
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
