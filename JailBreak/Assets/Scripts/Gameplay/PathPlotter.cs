﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;

public class PathPlotter : MonoBehaviour
{
    [SerializeField] private GameObject plotterLinePref;
    [SerializeField] private GameObject markerPref;

    private List<Vector2> plottedPoints = new List<Vector2>();
    bool plotterPicked = false;
    bool objectInitialized = false;

    private LineRenderer line;

    private List<int> plottedIds = new List<int>();
    private List<int> currentAllowedTiles = new List<int>();
    private List<GameObject> markers = new List<GameObject>();

    private void Start()
    {
        if (TestController.testEnvironment)
            Initialize();
    }

    public void Initialize()
    {
        objectInitialized = true;
    }

    public void DisablePlotting()
    {
        objectInitialized = false;
        line.positionCount--;
    }

    public void ResetPlotter()
    {
        Destroy(line);

        plottedPoints.Clear();
        plottedIds.Clear();

        for (int i = 0; i < markers.Count; i++)
        {
            Destroy(markers[i]);
        }

        markers.Clear();
    }

    public void DestroyPlotterLine()
    {
        Destroy(line);

        for (int i = 0; i < markers.Count; i++)
        {
            Destroy(markers[i]);
        }

        markers.Clear();
    }


    void Update()
    {
        if (objectInitialized)
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
                if (plotterPicked)
                    GameEventManager.Instance.TriggerSyncEvent(new PathDrawingCompleteEvent(plottedPoints, plottedIds));

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
                            if (plottedIds.Count == 0)
                            {
                                GameObject lineObj = Instantiate(plotterLinePref);
                                line = lineObj.GetComponentInChildren<LineRenderer>();
                                line.transform.position = Vector3.zero;
                                line.transform.localScale = Vector3.one;

                                plottedPoints.Clear();

                                plottedIds.Add(tileData.tileID);
                                PlotLineToTile(hitsinfo[i].collider.transform.position, tileData);
                            }
                            else if (plottedIds[plottedIds.Count - 1] != tileData.tileID)
                            {
                                plottedIds.Add(tileData.tileID);
                                PlotLineToTile(hitsinfo[i].collider.transform.position, tileData);
                            }
                        }
                    }
                }

                Vector3 pos;
                pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                pos.z = 0;

                transform.position = pos;

                if (plottedIds.Count != 0)
                    line.SetPosition(line.positionCount - 1, pos);
            }
        }
    }

    private void PlotLineToTile(Vector3 position, TileData tileData)
    {
        if (line.positionCount == 0)
        {
            line.positionCount++;
            line.SetPosition(line.positionCount - 1, position);
        }
        else
        {
            line.SetPosition(line.positionCount - 1, position);
        }

        GameObject marker = Instantiate(markerPref, position, Quaternion.identity);
        markers.Add(marker);

        plottedPoints.Add(position);
        line.positionCount++;

        currentAllowedTiles = tileData.neighbouringTiles;

        GameEventManager.Instance.TriggerAsyncEvent(new PlotterPlottedPoint(tileData));
    }

    public void SetInitialAllowedTiles(int startTileID, List<int> neighbouringTiles)
    {
        currentAllowedTiles = neighbouringTiles;
        currentAllowedTiles.Add(startTileID);

        //GameEventManager.Instance.TriggerAsyncEvent(new PlotterPlottedPoint(tileData));
    }
}
