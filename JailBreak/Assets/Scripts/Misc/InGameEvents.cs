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

public class PlayerDetectedEvent : GameEvent
{
    public PlayerDetectedEvent()
    {
        Debug.Log("Player detected!!");
    }
}

