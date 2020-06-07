using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public static bool GameTicked = false;

    private DateTime startTime;
    bool gameStarted = false;

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
            else if((DateTime.Now - startTime).TotalMilliseconds >= 1000f)
            {
                GameTicked = true;
                return;
            }

        }
    }
}
