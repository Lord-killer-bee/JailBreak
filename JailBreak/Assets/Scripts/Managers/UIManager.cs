using System.Collections;
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
    [SerializeField] private TextAnimator transitionTextImage;
    [SerializeField] private GameObject countdownPanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private MessageUnit[] messageSprites;

    [SerializeField] private DialogueUnit[] playerDialogueObjects;
    [SerializeField] private DialogueUnit[] duckDialogueObjects;


    int currentLevelIndex = 0;

    bool firstLevel = true;
    int duckIndex = 0;

    private void OnEnable()
    {
        GameEventManager.Instance.AddListener<GameStateChangedEvent>(OnGameStateChangedEvent);
        GameEventManager.Instance.AddListener<PlayerDetectedEvent>(OnPlayerDetectedEvent);
        GameEventManager.Instance.AddListener<SetCurrentLevelID>(OnSetCurrentLevelID);
        GameEventManager.Instance.AddListener<PathDrawingCompleteEvent>(OnPathDrawingComplete);
    }

    private void OnDisable()
    {
        GameEventManager.Instance.RemoveListener<GameStateChangedEvent>(OnGameStateChangedEvent);
        GameEventManager.Instance.RemoveListener<PlayerDetectedEvent>(OnPlayerDetectedEvent);
        GameEventManager.Instance.RemoveListener<SetCurrentLevelID>(OnSetCurrentLevelID);
        GameEventManager.Instance.RemoveListener<PathDrawingCompleteEvent>(OnPathDrawingComplete);
    }

    public void StartPlotting()
    {
        GameEventManager.Instance.TriggerSyncEvent(new GameStateCompletedEvent(GameStateType.ExamineLevel));
        PlayButtonSound();

        if(currentLevelIndex == 0)
        {
            playerDialogueObjects[0].dialogueUnits[0].SetActive(false);
            playerDialogueObjects[0].dialogueUnits[1].SetActive(true);

            Invoke("EnableDuckBubble", 1.0f);
        }
    }

    void EnableDuckBubble()
    {
        if (currentLevelIndex == 0)
        {
            duckDialogueObjects[0].dialogueUnits[duckIndex].SetActive(true);
            duckIndex++;
        }
        else if(currentLevelIndex == 1)
        {
            duckDialogueObjects[1].dialogueUnits[0].SetActive(true);

            Invoke("DisableLevelBubbles", 3.0f);
        }
        else if (currentLevelIndex == 2)
        {
            duckDialogueObjects[2].dialogueUnits[0].SetActive(true);

            Invoke("DisableLevelBubbles", 3.0f);
        }
        else if (currentLevelIndex == 3)
        {
            duckDialogueObjects[3].dialogueUnits[0].SetActive(true);

            Invoke("DisableLevelBubbles", 3.0f);
        }
    }

    void DisableLevelBubbles()
    {
        if (currentLevelIndex == 1)
        {
            duckDialogueObjects[1].dialogueUnits[0].SetActive(false);
            playerDialogueObjects[1].dialogueUnits[0].SetActive(false);
        }
        else if(currentLevelIndex == 2)
        {
            duckDialogueObjects[2].dialogueUnits[0].SetActive(false);
            playerDialogueObjects[2].dialogueUnits[0].SetActive(false);
        }
        else if (currentLevelIndex == 3)
        {
            duckDialogueObjects[3].dialogueUnits[0].SetActive(false);
            playerDialogueObjects[3].dialogueUnits[0].SetActive(false);
        }
    }

    private void OnPathDrawingComplete(PathDrawingCompleteEvent e)
    {
        if (currentLevelIndex == 0)
        {
            playerDialogueObjects[0].dialogueUnits[1].SetActive(false);
            duckDialogueObjects[0].dialogueUnits[0].SetActive(false);

            playerDialogueObjects[0].dialogueUnits[2].SetActive(true);

            Invoke("EnableDuckBubble", 2.0f);
        }
    }

    public void ConfirmPath()
    {
        GameEventManager.Instance.TriggerSyncEvent(new GameStateCompletedEvent(GameStateType.Plotting));

        if(currentLevelIndex == 0)
        {
            playerDialogueObjects[0].dialogueUnits[2].SetActive(false);
        }
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
        UpdateTutorial(e.stateType);

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
                SetLevelTransition();
                break;
            default:
                break;
        }
    }

    private void UpdateTutorial(GameStateType stateType)
    {
        switch (stateType)
        {
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
    }

    private void OnSetCurrentLevelID(SetCurrentLevelID e)
    {
        currentLevelIndex = e.levelID;

        if (e.levelID == 0)
        {
            playerDialogueObjects[0].dialogueUnits[0].SetActive(true);
        }
        else if(e.levelID == 1)
        {
            duckDialogueObjects[0].dialogueUnits[1].SetActive(false);

            playerDialogueObjects[1].dialogueUnits[0].SetActive(true);

            Invoke("EnableDuckBubble", 4.0f);
        }
        else if (e.levelID == 2)
        {
            playerDialogueObjects[2].dialogueUnits[0].SetActive(true);

            Invoke("EnableDuckBubble", 4.0f);
        }
        else if (e.levelID == 3)
        {
            playerDialogueObjects[3].dialogueUnits[0].SetActive(true);

            Invoke("EnableDuckBubble", 4.0f);
        }
    }

    private void OnPlayerDetectedEvent(PlayerDetectedEvent e)
    {
        restartButton.SetActive(true);
    }

    #endregion

    private void SetLevelTransition()
    {
        //transitionTextImage.GetComponent<Image>().sprite = messageSprites[currentLevelIndex].sprites[0];
        //transitionTextImage.SetSprites(messageSprites[currentLevelIndex].sprites);
        //transitionPanel.SetActive(true);
    }

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

[System.Serializable]
public struct MessageUnit
{
    public Sprite[] sprites;
}

[System.Serializable]
public struct DialogueUnit
{
    public GameObject[] dialogueUnits;
}