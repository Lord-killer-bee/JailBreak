﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Core;
using System;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject startPlottingButton;
    [SerializeField] private GameObject plotConfirmButton;
    [SerializeField] private GameObject plotResetButton;
    [SerializeField] private GameObject rePlotButton;
    [SerializeField] private GameObject restartButton;
    [SerializeField] private GameObject transitionPanel;
    [SerializeField] private GameObject countdownPanel;
    [SerializeField] private GameObject mainMenuPanel;

    bool firstLevel = true;

    private void OnEnable()
    {
        GameEventManager.Instance.AddListener<GameStateChangedEvent>(OnGameStateChangedEvent);
        GameEventManager.Instance.AddListener<PlayerDetectedEvent>(OnPlayerDetectedEvent);
    }

    private void OnDisable()
    {
        GameEventManager.Instance.RemoveListener<GameStateChangedEvent>(OnGameStateChangedEvent);
        GameEventManager.Instance.RemoveListener<PlayerDetectedEvent>(OnPlayerDetectedEvent);
    }

    public void StartPlotting()
    {
        GameEventManager.Instance.TriggerSyncEvent(new GameStateCompletedEvent(GameStateType.ExamineLevel));
        PlayButtonSound();
    }

    public void ConfirmPath()
    {
        GameEventManager.Instance.TriggerSyncEvent(new GameStateCompletedEvent(GameStateType.Plotting));
    }

    public void ResetPath()
    {
        GameEventManager.Instance.TriggerSyncEvent(new ResetPlotterEvent());
        PlayButtonSound();
    }

    public void RestartCurrentLevel()
    {
        GameEventManager.Instance.TriggerSyncEvent(new RestartLevelEvent());
        PlayButtonSound();
    }

    public void RePlot()
    {
        GameEventManager.Instance.TriggerSyncEvent(new RetryLevelEvent());
        PlayButtonSound();
    }

    public void StartGame()
    {
        if (firstLevel)
        {
            transitionPanel.SetActive(true);
            firstLevel = false;
        }
        mainMenuPanel.SetActive(false);
        PlayButtonSound();

        Invoke("TriggerMainMenuComplete", 1.5f);
    }

    void TriggerMainMenuComplete()
    {
        GameEventManager.Instance.TriggerSyncEvent(new GameStateCompletedEvent(GameStateType.MainMenu));
    }

    void PlayButtonSound()
    {
        GameEventManager.Instance.TriggerAsyncEvent(new ButtonClicked());
    }

    #region Event Listeners

    private void OnGameStateChangedEvent(GameStateChangedEvent e)
    {
        switch (e.stateType)
        {
            case GameStateType.LoadScene:

                break;
            case GameStateType.LevelSetup:
                break;
            case GameStateType.ExamineLevel:
                transitionPanel.SetActive(false);

                startPlottingButton.SetActive(true);
                restartButton.SetActive(false);
                rePlotButton.SetActive(false);
                break;
            case GameStateType.Plotting:
                startPlottingButton.SetActive(false);
                plotConfirmButton.SetActive(true);
                plotResetButton.SetActive(true);
                break;
            case GameStateType.SimulateLevel:
                PlaySimulationCountdown();

                plotConfirmButton.SetActive(false);
                plotResetButton.SetActive(false);
                rePlotButton.SetActive(true);
                break;
            case GameStateType.TransitionToNextLevel:
                transitionPanel.SetActive(true);

                break;
            default:
                break;
        }
    }

    private void OnPlayerDetectedEvent(PlayerDetectedEvent e)
    {
        restartButton.SetActive(true);
    }

    #endregion

    private void PlaySimulationCountdown()
    {
        countdownPanel.SetActive(true);
        Invoke("FireCountdownComplete", 3.0f);
    }

    private void FireCountdownComplete()
    {
        countdownPanel.SetActive(false);
        GameEventManager.Instance.TriggerSyncEvent(new SimulateCountDownEnded());
    }
}
