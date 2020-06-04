using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;

public class LevelManager : MonoBehaviour
{
    #region Scene refs

    [SerializeField] private GameObject baseTilePref;
    [SerializeField] private GameObject playerPref;
    [SerializeField] private GameObject plotterPref;
    [SerializeField] private GameObject securityCamPref;
    [SerializeField] private GameObject enemyPatrolPref;
    [SerializeField] private GameObject laserPref;

    private LevelData currentLevelDataObj;
    private int currentLevelID;

    #endregion

    #region Private variables
    private Dictionary<int, TileData> tileDataMap = new Dictionary<int, TileData>();
    private List<GameObject> tiles = new List<GameObject>();
    private GameObject player, plotter;

    #endregion

    #region Base methods

    private void Start()
    {
        currentLevelID = 0;
        currentLevelDataObj = Resources.Load<LevelData>(GameConsts.LEVEL_DATA_PATH + currentLevelID);
    }

    private void OnEnable()
    {
        GameEventManager.Instance.AddListener<GameStateChangedEvent>(OnGameStateChanged);
        GameEventManager.Instance.AddListener<ResetPlotterEvent>(OnResetPlotter);
        GameEventManager.Instance.AddListener<RestartLevelEvent>(OnRestartLevel);
    }

    private void OnDisable()
    {
        GameEventManager.Instance.RemoveListener<GameStateChangedEvent>(OnGameStateChanged);
        GameEventManager.Instance.RemoveListener<ResetPlotterEvent>(OnResetPlotter);
        GameEventManager.Instance.RemoveListener<RestartLevelEvent>(OnRestartLevel);
    }

    #endregion

    #region Event listeners

    private void OnGameStateChanged(GameStateChangedEvent e)
    {
        switch (e.stateType)
        {
            case GameStateType.LevelSetup:
                CreateGrid();
                SetupLevelElements();
                break;
            case GameStateType.ExamineLevel:
                break;
            case GameStateType.Plotting:
                plotter.SetActive(true);
                plotter.GetComponent<PathPlotter>().Initialize();
                break;
            case GameStateType.SimulateLevel:
                plotter.GetComponent<PathPlotter>().DisablePlotting();
                plotter.SetActive(false);
                break;
            case GameStateType.TransitionToNextLevel:
                LoadNextLevel();
                break;
        }
    }

    private void OnResetPlotter(ResetPlotterEvent e)
    {
        plotter.transform.position = tileDataMap[currentLevelDataObj.startTileID].tileLocation;
        plotter.GetComponent<PathPlotter>().ResetPlotter();
    }

    private void OnRestartLevel(RestartLevelEvent e)
    {
        player.transform.position = tileDataMap[currentLevelDataObj.startTileID].tileLocation;
        plotter.transform.position = tileDataMap[currentLevelDataObj.startTileID].tileLocation;
        plotter.GetComponent<PathPlotter>().ResetPlotter();
        plotter.SetActive(false);
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

        tiles.Clear();

        foreach (KeyValuePair<int, TileData> data in tileDataMap)
        {
            tilePref = baseTilePref;

            tile = Instantiate(tilePref, transform);
            tile.transform.localPosition = data.Value.tileLocation;
            tile.transform.localScale = Vector3.one;
            tile.transform.localRotation = Quaternion.identity;

            tiles.Add(tile);

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

        plotter.SetActive(false);

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

            cam.GetComponent<SecurityCamera>().Initialize();
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

            patrol.GetComponent<PatrollingEnemy>().Initialize();
        }

        for (int i = 0; i < currentLevelDataObj.lasersdata.Count; i++)
        {
            GameObject laser = Instantiate(laserPref);
            EnemyLaser laserComp = laser.GetComponent<EnemyLaser>();

            laserComp.SetLaserEndPositions(currentLevelDataObj.lasersdata[i].laserEnds);

            laser.transform.position = currentLevelDataObj.lasersdata[i].position;
            laser.transform.rotation = currentLevelDataObj.lasersdata[i].rotation;
            laser.transform.localScale = currentLevelDataObj.lasersdata[i].scale;

            laser.GetComponent<EnemyLaser>().Initialize();
        }

        GameEventManager.Instance.TriggerSyncEvent(new GameStateCompletedEvent(GameStateType.LevelSetup));
    }

    public void ClearLevel()
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            Destroy(tiles[i]);
        }

        tiles.Clear();
        tileDataMap.Clear();

        Destroy(player);

        plotter.GetComponent<PathPlotter>().DestroyPlotterLine();
        Destroy(plotter);

        SecurityCamera[] cams = FindObjectsOfType<SecurityCamera>();

        for (int i = 0; i < cams.Length; i++)
        {
            Destroy(cams[i].gameObject);
        }

        //Laser data
        EnemyLaser[] lasers = FindObjectsOfType<EnemyLaser>();

        for (int i = 0; i < lasers.Length; i++)
        {
            Destroy(lasers[i].gameObject);
        }

        //Patrol data
        PatrollingEnemy[] patrollers = FindObjectsOfType<PatrollingEnemy>();

        for (int i = 0; i < patrollers.Length; i++)
        {
            Destroy(patrollers[i].gameObject);
        }

    }

    private void LoadNextLevel()
    {
        currentLevelID++;
        currentLevelDataObj = Resources.Load<LevelData>(GameConsts.LEVEL_DATA_PATH + currentLevelID);

        if (currentLevelDataObj == null)
        {
            Debug.LogError("Level file not found!!");
            return;
        }
        else
        {
            ClearLevel();
            CreateGrid();
            SetupLevelElements();

            GameEventManager.Instance.TriggerSyncEvent(new GameStateCompletedEvent(GameStateType.TransitionToNextLevel));
        }
    }

    #endregion

}

/// <summary>
/// The enum denoting the type for a tile
/// </summary>
public enum TileType
{
    None,
    BaseTile,
}

/// <summary>
/// Class to store the data required for a tile
/// </summary>
[System.Serializable]
public class TileData
{
    public int tileID;
    public TileType tileType;
    public Vector2 tileLocation;

    public TileData() { }

    public TileData(TileType tileType, Vector2 tileLocation)
    {
        this.tileType = tileType;
        this.tileLocation = tileLocation;
    }
}