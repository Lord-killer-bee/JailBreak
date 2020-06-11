using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;
using System;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float moveWaitTime;

    bool plotterPicked = false;
    private List<int> currentAllowedTiles = new List<int>();

    private List<Vector2> plottedPoints;
    public List<int> plottedTileIDs;
    bool startTraversal = false;
    bool startWaiting = false;
    float waitUnits = 0;

    int currentPathIndex = 0;
    int targetPathIndex = 0;

    Vector2 targetPosition;

    private void Start()
    {
        GameEventManager.Instance.AddListener<PathDrawingCompleteEvent>(OnPathDrawingComplete);
        GameEventManager.Instance.AddListener<GameStateChangedEvent>(OnGameStateChanged);
        GameEventManager.Instance.AddListener<PlayerDetectedEvent>(OnPlayerDetected);
    }

    private void OnDestroy()
    {
        GameEventManager.Instance.RemoveListener<PathDrawingCompleteEvent>(OnPathDrawingComplete);
        GameEventManager.Instance.RemoveListener<GameStateChangedEvent>(OnGameStateChanged);
        GameEventManager.Instance.RemoveListener<PlayerDetectedEvent>(OnPlayerDetected);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D[] hitsinfo = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.forward, 100f);

            for (int i = 0; i < hitsinfo.Length; i++)
            {
                if (hitsinfo[i].collider.gameObject == gameObject)
                {
                    plotterPicked = true;
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            plotterPicked = false;
        }

        if (plotterPicked)
        {
            RaycastHit2D[] hitsinfo = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.forward, 100f);

            for (int i = 0; i < hitsinfo.Length; i++)
            {
                if (hitsinfo[i].collider.tag == GameConsts.BASETILE_TAG)
                {
                    TileData tileData = hitsinfo[i].collider.GetComponent<Tile>().tileData;

                    if (currentAllowedTiles.Contains(tileData.tileID))
                    {
                        
                    }
                }
            }

            Vector3 pos;
            pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;

            transform.position = pos;

            //if (plottedIds.Count != 0)
            //    line.SetPosition(line.positionCount - 1, pos);
        }
    }

    private void LateUpdate()
    {
        if (startTraversal)
        {
            if (currentPathIndex < targetPathIndex && !startWaiting)
            {
                transform.position = targetPosition;
            }

            if (Vector3.Distance(transform.position, targetPosition) == 0 && !startWaiting)
            {
                currentPathIndex++;
                startWaiting = true;

                if(!TestController.testEnvironment)
                    GameEventManager.Instance.TriggerAsyncEvent(new PlayerMovedToTileEvent(plottedTileIDs[currentPathIndex]));
            }

            if (startWaiting)
            {
                if (GameTimer.GameTicked)
                {
                    if (startWaiting)
                    {
                        waitUnits--;

                        if (waitUnits <= 0)
                            startWaiting = false;
                    }
                }

                if(!startWaiting)
                {
                    targetPathIndex++;
                    startWaiting = false;

                    if (targetPathIndex < plottedPoints.Count)
                    {
                        targetPosition = plottedPoints[targetPathIndex];

                    }
                }
            }

            if (currentPathIndex >= plottedPoints.Count - 1)
            {
                startTraversal = false;
            }
        }
    }

    private void StartTraversal()
    {
        startTraversal = true;
        currentPathIndex = 0;
        targetPathIndex = 0;

        startWaiting = true;
        waitUnits = moveWaitTime;

        targetPosition = plottedPoints[targetPathIndex];
    }

    private void OnPathDrawingComplete(PathDrawingCompleteEvent e)
    {
        plottedPoints = e.plottedPoints;
        plottedTileIDs = e.plottedTileIDs;

        if (TestController.testEnvironment)
        {
            StartTraversal();
        }
    }

    private void OnGameStateChanged(GameStateChangedEvent e)
    {
        //if (e.stateType == GameStateType.SimulateLevel)
        //    StartTraversal();
    }

    private void OnPlayerDetected(PlayerDetectedEvent e)
    {
        startTraversal = false;
    }

}
