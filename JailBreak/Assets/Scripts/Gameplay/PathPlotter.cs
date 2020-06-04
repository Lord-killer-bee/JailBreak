using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;

public class PathPlotter : MonoBehaviour
{
    [SerializeField] private GameObject plotterLinePref;

    private List<Vector2> plottedPoints = new List<Vector2>();
    bool plotterPicked = false;
    bool objectInitialized = false;

    private LineRenderer line;

    private List<int> plottedIds = new List<int>();

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
        line.positionCount = 0;
        plottedPoints.Clear();
        plottedIds.Clear();
    }

    public void DestroyPlotterLine()
    {
        Destroy(line);
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

                        if (plottedIds.Count == 0)
                        {
                            GameObject lineObj = Instantiate(plotterLinePref);
                            line = lineObj.GetComponentInChildren<LineRenderer>();
                            line.transform.position = Vector3.zero;
                            line.transform.localScale = Vector3.one;

                            plottedPoints.Clear();

                            plottedIds.Add(tileData.tileID);
                            PlotLineToTile(hitsinfo[i].collider.transform.position);
                        }
                        else if (plottedIds[plottedIds.Count - 1] != tileData.tileID)
                        {
                            plottedIds.Add(tileData.tileID);
                            PlotLineToTile(hitsinfo[i].collider.transform.position);
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

    private void PlotLineToTile(Vector3 position)
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

        plottedPoints.Add(position);
        line.positionCount++;
    }
}
