using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;

public class PathDrawingCompleteEvent : GameEvent
{
    public List<Vector2> plottedPoints = new List<Vector2>();
    public List<int> plottedTileIDs = new List<int>();

    public PathDrawingCompleteEvent(List<Vector2> plottedPoints, List<int> plottedTileIDs)
    {
        this.plottedPoints = plottedPoints;
        this.plottedTileIDs = plottedTileIDs;
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

public class PlayerMovedToTileEvent : GameEvent
{
    public int tileID;

    public PlayerMovedToTileEvent(int tileID)
    {
        this.tileID = tileID;
    }
}

public class SetCurrentLevelID : GameEvent
{
    public int levelID;

    public SetCurrentLevelID(int levelID)
    {
        this.levelID = levelID;
    }
}

public class PlayerCompletedLevelEvent : GameEvent
{

}

public class RestartLevelEvent : GameEvent
{

}