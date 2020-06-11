using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerController : MonoBehaviour
{
    private List<int> currentAllowedTiles = new List<int>();

    Vector2 targetPosition;
    bool targetSet = false;

    TileData clickedTile;

    [SerializeField] GameObject leftIndicator;
    [SerializeField] GameObject rightIndicator;
    [SerializeField] GameObject topIndicator;
    [SerializeField] GameObject bottomIndicator;

    int columnCount = 0;

    public void Initialize(TileData tileData, int rowCount, int columnCount)
    {
        currentAllowedTiles = tileData.neighbouringTiles;
        this.columnCount = columnCount;

        clickedTile = tileData;
        SetIndicatorVsibility();
    }

    void LateUpdate()
    {
        if (GameTimer.GameTicked)
        {
            if (targetSet)
            {
                transform.position = targetPosition;
                targetSet = false;

                RetrieveAllowedTiles();
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D[] hitsinfo = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.forward, 100f);

            for (int i = 0; i < hitsinfo.Length; i++)
            {
                if (hitsinfo[i].collider.tag == GameConsts.BASETILE_TAG)
                {
                    if (currentAllowedTiles.Contains(hitsinfo[i].collider.GetComponent<Tile>().tileData.tileID))
                    {
                        targetSet = true;
                        clickedTile = hitsinfo[i].collider.GetComponent<Tile>().tileData;
                        targetPosition = clickedTile.tileLocation;
                    }
                }
            }
        }
    }

    private void RetrieveAllowedTiles()
    {
        currentAllowedTiles = clickedTile.neighbouringTiles;
        SetIndicatorVsibility();
    }

    void SetIndicatorVsibility()
    {
        leftIndicator.SetActive(false);
        rightIndicator.SetActive(false);
        topIndicator.SetActive(false);
        bottomIndicator.SetActive(false);

        for (int i = 0; i < currentAllowedTiles.Count; i++)
        {
            if (clickedTile.tileID - currentAllowedTiles[i] == 1)//Left indicator
            {
                leftIndicator.SetActive(true);
            }

            if (clickedTile.tileID - currentAllowedTiles[i] == -1)//Right indicator
            {
                rightIndicator.SetActive(true);
            }

            if (clickedTile.tileID - currentAllowedTiles[i] == columnCount)//Top indicator
            {
                topIndicator.SetActive(true);
            }

            if (clickedTile.tileID - currentAllowedTiles[i] == -columnCount)//Bottom indicator
            {
                bottomIndicator.SetActive(true);
            }
        }
    }
}
