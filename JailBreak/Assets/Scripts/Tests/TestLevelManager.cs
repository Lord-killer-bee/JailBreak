using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;

public class TestLevelManager : MonoBehaviour
{
    public static bool testEnvironment = false;

    #region Scene refs

    [SerializeField] private GameObject baseTilePref;
    [SerializeField] private GameObject playerPref;
    [SerializeField] private GameObject plotterPref;
    [SerializeField] private GameObject securityCamPref;
    [SerializeField] private GameObject enemyPatrolPref;
    [SerializeField] private GameObject laserPref;
    [SerializeField] private LevelData currentLevelDataObj;

    #endregion

    #region Private variables
    private Dictionary<int, TileData> tileDataMap = new Dictionary<int, TileData>();
    private GameObject player, plotter;

    #endregion

    #region Base methods

    private void Start()
    {
        testEnvironment = true;
        CreateGrid();
        SetupLevelElements();
    }

    #endregion

    #region Grid Creation

    /// <summary>
    /// Creates the tiles for the level, Creates the destructable and non destructable obstacles
    /// Creates non destructable obstacles at every odd row and column 
    /// Destructible obstacles are created at random but are avoided to spawn near player
    /// </summary>
    void CreateGrid()
    {
        CalculateCellLocations();

        GameObject tile = null;
        GameObject tilePref = null;

        foreach (KeyValuePair<int, TileData> data in tileDataMap)
        {
            tilePref = baseTilePref;

            tile = Instantiate(tilePref, transform);
            tile.transform.localPosition = data.Value.tileLocation;
            tile.transform.localScale = Vector3.one;
            tile.transform.localRotation = Quaternion.identity;

            data.Value.tileType = TileType.BaseTile;

            tile.GetComponent<Tile>().tileData = data.Value;
        }
    }

    /// <summary>
    /// Calculates the cell locations and tileIDs for the grid
    /// </summary>
    void CalculateCellLocations()
    {
        float x = 0, y = 0;
        int tileID;

        for (int row = 0; row < currentLevelDataObj.rowCount; row++)
        {
            for (int column = 0; column < currentLevelDataObj.columnCount; column++)
            {
                x = column - ((float)currentLevelDataObj.columnCount - 1) / 2;
                y = ((float)currentLevelDataObj.rowCount - 1) / 2 - row;

                tileID = GetTileID(row, column);

                tileDataMap.Add(tileID, new TileData() { tileID = tileID, tileLocation = new Vector2(x, y) });
            }
        }
    }

    #endregion

    #region Helper methods

    /// <summary>
    /// Returns a tiledID for a specified row and column
    /// </summary>
    /// <param name="row"></param>
    /// <param name="column"></param>
    /// <returns></returns>
    public int GetTileID(int row, int column)
    {
        return column + (row) * (currentLevelDataObj.columnCount);
    }

    /// <summary>
    /// Returns the row number for a tileID
    /// </summary>
    /// <param name="tileID"></param>
    /// <returns></returns>
    public int GetRow(int tileID)
    {
        return tileID / currentLevelDataObj.columnCount;
    }

    /// <summary>
    /// Returns a column number for tileID
    /// </summary>
    /// <param name="tileID"></param>
    /// <returns></returns>
    public int GetColumn(int tileID)
    {
        return tileID - GetRow(tileID) * currentLevelDataObj.columnCount;
    }

    /// <summary>
    /// Gets the left side tile data for a specified tile
    /// Returns null if at left most edge
    /// </summary>
    /// <param name="tileID"></param>
    /// <returns></returns>
    public TileData GetLeftTileData(int tileID)
    {
        int row = GetRow(tileID);
        int column = GetColumn(tileID);

        if (column != 0)
            return tileDataMap[GetTileID(row, column - 1)];
        else
            return null;
    }

    /// <summary>
    /// Gets the right side tile data for a specified tile
    /// Returns null if at right most edge
    /// </summary>
    /// <param name="tileID"></param>
    /// <returns></returns>
    public TileData GetRightTileData(int tileID)
    {
        int row = GetRow(tileID);
        int column = GetColumn(tileID);

        if (column != currentLevelDataObj.columnCount - 1)
            return tileDataMap[GetTileID(row, column + 1)];
        else
            return null;
    }

    /// <summary>
    /// Gets the top side tile data for a specified tile
    /// Returns null if at top most edge</summary>
    /// <param name="tileID"></param>
    /// <returns></returns>
    public TileData GetTopTileData(int tileID)
    {
        int row = GetRow(tileID);
        int column = GetColumn(tileID);

        if (row != 0)
            return tileDataMap[GetTileID(row - 1, column)];
        else
            return null;
    }

    /// <summary>
    /// Gets the bottom side tile data for a specified tile
    /// Returns null if at bottom most edge</summary>
    /// <param name="tileID"></param>
    /// <returns></returns>
    public TileData GetBottomTileData(int tileID)
    {
        int row = GetRow(tileID);
        int column = GetColumn(tileID);

        if (row != currentLevelDataObj.rowCount - 1)
            return tileDataMap[GetTileID(row + 1, column)];
        else
            return null;
    }

    #endregion

    #region Level related

    private void SetupLevelElements()
    {
        player = Instantiate(playerPref);
        player.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        player.transform.position = tileDataMap[currentLevelDataObj.startTileID].tileLocation;

        plotter = Instantiate(plotterPref);
        plotter.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        plotter.transform.position = tileDataMap[currentLevelDataObj.startTileID].tileLocation;

        for (int i = 0; i < currentLevelDataObj.securityCamsdata.Count; i++)
        {
            GameObject cam = Instantiate(securityCamPref);
            SecurityCamera camComp = cam.GetComponent<SecurityCamera>();

            camComp.SetCameraPatrolAngle(currentLevelDataObj.securityCamsdata[i].cameraPatrolAngle);
            camComp.SetCameraPatrolSpeed(currentLevelDataObj.securityCamsdata[i].cameraPatrolSpeed);
            camComp.SetDetectionArcAngle(currentLevelDataObj.securityCamsdata[i].detectionArcAngle);
            camComp.SetDetectionArcRadius(currentLevelDataObj.securityCamsdata[i].detectionArcRadius);

            cam.transform.position = currentLevelDataObj.securityCamsdata[i].position;
            cam.transform.rotation = currentLevelDataObj.securityCamsdata[i].rotation;
            cam.transform.localScale = currentLevelDataObj.securityCamsdata[i].scale;
        }

        for (int i = 0; i < currentLevelDataObj.patrollersdata.Count; i++)
        {
            GameObject patrol = Instantiate(enemyPatrolPref);
            PatrollingEnemy patrolComp = patrol.GetComponent<PatrollingEnemy>();

            patrolComp.SetDetectionTileRange(currentLevelDataObj.patrollersdata[i].detectionTileRange);
            patrolComp.SetMoveSpeed(currentLevelDataObj.patrollersdata[i].moveSpeed);
            patrolComp.SetWayPoints(currentLevelDataObj.patrollersdata[i].waypoints);

            patrol.transform.position = currentLevelDataObj.patrollersdata[i].position;
            patrol.transform.rotation = currentLevelDataObj.patrollersdata[i].rotation;
            patrol.transform.localScale = currentLevelDataObj.patrollersdata[i].scale;
        }

        for (int i = 0; i < currentLevelDataObj.lasersdata.Count; i++)
        {
            GameObject laser = Instantiate(laserPref);
            EnemyLaser laserComp = laser.GetComponent<EnemyLaser>();

            laserComp.SetLaserEndPositions(currentLevelDataObj.lasersdata[i].laserEnds);

            laser.transform.position = currentLevelDataObj.lasersdata[i].position;
            laser.transform.rotation = currentLevelDataObj.lasersdata[i].rotation;
            laser.transform.localScale = currentLevelDataObj.lasersdata[i].scale;
        }
    }

    #endregion

}