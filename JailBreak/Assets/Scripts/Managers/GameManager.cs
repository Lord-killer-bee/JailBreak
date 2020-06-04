using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameStateType currentGameState;

    void SetState(GameStateType state)
    {
        if(currentGameState != state)
            currentGameState = state;

        switch (currentGameState)
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

    void UpdateCurrentState()
    {
        switch (currentGameState)
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
}
