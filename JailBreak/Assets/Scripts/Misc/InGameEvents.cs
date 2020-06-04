using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;

public class PathDrawingCompleteEvent : GameEvent
{
    public List<Vector2> plottedPoints = new List<Vector2>();

    public PathDrawingCompleteEvent(List<Vector2> plottedPoints)
    {
        this.plottedPoints = plottedPoints;
    }
}

public class ResetPlotterEvent : GameEvent
{

}

public class PlayerDetectedEvent : GameEvent
{
    public PlayerDetectedEvent()
    {
        Debug.Log("Player detected!!");
    }
}

public class GameStateChangedEvent : GameEvent
{
    public GameStateType stateType;

    public GameStateChangedEvent(GameStateType stateType)
    {
        this.stateType = stateType;
    }
}

public class GameStateCompletedEvent : GameEvent
{
    public GameStateType stateType;

    public GameStateCompletedEvent(GameStateType stateType)
    {
        this.stateType = stateType;
    }
}

public class PlayerCompletedLevelEvent : GameEvent
{

}

public class RestartLevelEvent : GameEvent
{

}