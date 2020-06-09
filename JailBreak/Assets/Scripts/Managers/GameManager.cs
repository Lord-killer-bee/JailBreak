using Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameStateType currentGameState;

    private void OnEnable()
    {
        GameEventManager.Instance.AddListener<GameStateCompletedEvent>(OnGameStateCompleted);
        GameEventManager.Instance.AddListener<RestartLevelEvent>(OnLevelRestart);
        GameEventManager.Instance.AddListener<RetryLevelEvent>(OnLevelRetry);
        GameEventManager.Instance.AddListener<PlayerCompletedLevelEvent>(OnLevelComplete);
    }

    private void OnDisable()
    {
        GameEventManager.Instance.RemoveListener<GameStateCompletedEvent>(OnGameStateCompleted);
        GameEventManager.Instance.RemoveListener<RestartLevelEvent>(OnLevelRestart);
        GameEventManager.Instance.RemoveListener<RetryLevelEvent>(OnLevelRetry);
        GameEventManager.Instance.RemoveListener<PlayerCompletedLevelEvent>(OnLevelComplete);
    }

    private void Start()
    {
        SetState(GameStateType.LoadScene);
    }

    #region States related

    void SetState(GameStateType state)
    {
        if (currentGameState != state)
        {
            currentGameState = state;

            switch (currentGameState)
            {
                case GameStateType.LoadScene:
                    break;
                case GameStateType.LevelSetup:
                    break;
                case GameStateType.ExamineLevel:
                    break;
                case GameStateType.Plotting:
                    break;
                case GameStateType.SimulateLevel:
                    break;
                case GameStateType.TransitionToNextLevel:
                    break;
            }

            GameEventManager.Instance.TriggerSyncEvent(new GameStateChangedEvent(currentGameState));
        }
    }

    #endregion

    #region Event listeners

    private void OnGameStateCompleted(GameStateCompletedEvent e)
    {
        switch (e.stateType)
        {
            case GameStateType.LoadScene:
                SetState(GameStateType.LevelSetup);
                break;
            case GameStateType.LevelSetup:
                SetState(GameStateType.ExamineLevel);
                break;
            case GameStateType.ExamineLevel:
                SetState(GameStateType.Plotting);
                break;
            case GameStateType.Plotting:
                SetState(GameStateType.SimulateLevel);
                break;
            case GameStateType.SimulateLevel:
                break;
            case GameStateType.TransitionToNextLevel:
                SetState(GameStateType.LoadScene);
                break;
        }
    }

    private void OnLevelRestart(RestartLevelEvent e)
    {
        SetState(GameStateType.ExamineLevel);
    }

    private void OnLevelComplete(PlayerCompletedLevelEvent e)
    {
        SetState(GameStateType.TransitionToNextLevel);
    }

    private void OnLevelRetry(RetryLevelEvent e)
    {
        SetState(GameStateType.LevelSetup);
    }

    #endregion

}
