using Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public static bool GameTicked = false;

    private DateTime startTime;
    bool gameStarted = false;

    private void OnEnable()
    {
        GameEventManager.Instance.AddListener<PathDrawingCompleteEvent>(OnPathDrawingComplete);
        GameEventManager.Instance.AddListener<GameStateChangedEvent>(OnGameStateChanged);
        GameEventManager.Instance.AddListener<SimulateCountDownEnded>(OnSimulationCoundownEnded);
        GameEventManager.Instance.AddListener<ResetPlotterEvent>(OnResetPlotter);
    }

    private void OnDisable()
    {
        GameEventManager.Instance.RemoveListener<PathDrawingCompleteEvent>(OnPathDrawingComplete);
        GameEventManager.Instance.RemoveListener<GameStateChangedEvent>(OnGameStateChanged);
        GameEventManager.Instance.RemoveListener<SimulateCountDownEnded>(OnSimulationCoundownEnded);
        GameEventManager.Instance.RemoveListener<ResetPlotterEvent>(OnResetPlotter);
    }

    void Start()
    {
        gameStarted = true;
        startTime = DateTime.Now;
    }

    private void Update()
    {
        if (gameStarted)
        {
            if (GameTicked)
            {
                GameTicked = false;
                startTime = DateTime.Now;
            }
            else if((DateTime.Now - startTime).TotalMilliseconds >= 900f)
            {
                GameTicked = true;
                return;
            }

        }
    }

    private void OnPathDrawingComplete(PathDrawingCompleteEvent e)
    {
        if (TestController.testEnvironment)
            ResetTimer();
    }

    private void OnGameStateChanged(GameStateChangedEvent e)
    {
        if (e.stateType == GameStateType.Plotting)
        {
            ResetTimer();
        }
    }

    private void OnSimulationCoundownEnded(SimulateCountDownEnded e)
    {
        ResetTimer();
    }

    private void OnResetPlotter(ResetPlotterEvent e)
    {
        ResetTimer();
    }

    private void ResetTimer()
    {
        startTime = DateTime.Now;
        GameTicked = false;
    }
}
