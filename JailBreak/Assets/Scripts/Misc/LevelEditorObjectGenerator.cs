using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds the level data required by the main level manager
/// Accompanying script for Level editor script to generate objects
/// Keep one copy for every level scene generated
/// </summary>
public class LevelEditorObjectGenerator : MonoBehaviour
{
    [SerializeField] private GameObject baseTilePref;
    [SerializeField] private GameObject playerPref;
    [SerializeField] private GameObject plotterPref;

    [SerializeField] private GameObject securityCamPref;
    [SerializeField] private GameObject patrolPref;
    [SerializeField] private GameObject laserPref;

    [Space(10)]
    [Header("Level related")]
    [SerializeField] private LevelData levelData;

    private Dictionary<int, TileData> tileDataMap = new Dictionary<int, TileData>();

    private Dictionary<int, GameObject> tiles = new Dictionary<int, GameObject>();

    private GameObject player, plotter;

    #region Gameplay related (For test scenes)

    private void Start()
    {
        if (TestController.testEnvironment)
        {
            tileDataMap.Clear();

            Tile[] tiles = FindObjectsOfType<Tile>();

            for (int i = 0; i < tiles.Length; i++)
            {
                tileDataMap.Add(tiles[i].tileData.tileID, tiles[i].tileData);
            }

            SetupLevelElements();
        }
    }

    private void SetupLevelElements()
    {
        player = Instantiate(playerPref);
        player.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        player.transform.position = tileDataMap[levelData.startTileID].tileLocation;

        plotter = Instantiate(plotterPref);
        plotter.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        plotter.transform.position = tileDataMap[levelData.startTileID].tileLocation;
        plotter.GetComponent<PathPlotter>().SetInitialAllowedTiles(levelData.startTileID, tileDataMap[levelData.startTileID].neighbouringTiles);
    }

    public LevelData GetLevelData()
    {
        return levelData;
    }

    #endregion

    #region Level editor accompanying methods

    /// <summary>
    /// Creates the tiles for the level, Creates the destructable and non destructable obstacles
    /// Creates non destructable obstacles at every odd row and column 
    /// Destructible obstacles are created at random but are avoided to spawn near player
    /// </summary>
    public void CreateGrid(int rows, int columns)
    {
        tiles.Clear();
        tileDataMap.Clear();

        CalculateCellLocations(rows, columns);

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

            tiles.Add(data.Key, tile);

            TileData neighbouringTile = GetLeftTileData(data.Key);

            if (neighbouringTile != null)
            {
                data.Value.neighbouringTiles.Add(neighbouringTile.tileID);
            }

            neighbouringTile = GetRightTileData(data.Key);

            if (neighbouringTile != null)
            {
                data.Value.neighbouringTiles.Add(neighbouringTile.tileID);
            }

            neighbouringTile = GetTopTileData(data.Key);

            if (neighbouringTile != null)
            {
                data.Value.neighbouringTiles.Add(neighbouringTile.tileID);
            }

            neighbouringTile = GetBottomTileData(data.Key);

            if (neighbouringTile != null)
            {
                data.Value.neighbouringTiles.Add(neighbouringTile.tileID);
            }

            tile.GetComponent<Tile>().tileData = data.Value;
        }
    }

    /// <summary>
    /// Calculates the cell locations and tileIDs for the grid
    /// </summary>
    void CalculateCellLocations(int rows, int columns)
    {
        float x = 0, y = 0;
        int tileID;

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                x = column - ((float)columns - 1) / 2;
                y = ((float)rows - 1) / 2 - row;

                tileID = GetTileID(row, column, columns);

                tileDataMap.Add(tileID, new TileData() { tileID = tileID, tileLocation = new Vector2(x, y) });
            }
        }
    }



    /// <summary>
    /// Returns a tiledID for a specified row and column
    /// </summary>
    /// <param name="row"></param>
    /// <param name="column"></param>
    /// <returns></returns>
    public int GetTileID(int row, int column)
    {
        return column + (row) * (levelData.columnCount);
    }

    /// <summary>
    /// Returns the row number for a tileID
    /// </summary>
    /// <param name="tileID"></param>
    /// <returns></returns>
    public int GetRow(int tileID)
    {
        return tileID / levelData.columnCount;
    }

    /// <summary>
    /// Returns a column number for tileID
    /// </summary>
    /// <param name="tileID"></param>
    /// <returns></returns>
    public int GetColumn(int tileID)
    {
        return tileID - GetRow(tileID) * levelData.columnCount;
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

        if (column != levelData.columnCount - 1)
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

        if (row != levelData.rowCount - 1)
            return tileDataMap[GetTileID(row + 1, column)];
        else
            return null;
    }

    public int GetTileID(int row, int column, int maxColumns)
    {
        return column + (row) * (maxColumns);
    }

    public void ClearLevel()
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            DestroyImmediate(tiles[i]);
        }

        tiles.Clear();
        tileDataMap.Clear();

        SecurityCamera[] cams = FindObjectsOfType<SecurityCamera>();

        for (int i = 0; i < cams.Length; i++)
        {
            DestroyImmediate(cams[i].gameObject);
        }

        //Laser data
        EnemyLaser[] lasers = FindObjectsOfType<EnemyLaser>();

        for (int i = 0; i < lasers.Length; i++)
        {
            DestroyImmediate(lasers[i].gameObject);
        }

        //Patrol data
        PatrollingEnemy[] patrollers = FindObjectsOfType<PatrollingEnemy>();

        for (int i = 0; i < patrollers.Length; i++)
        {
            DestroyImmediate(patrollers[i].gameObject);
        }

    }

    public Vector2 GetTileLocation(int tileID)
    {
        return tileDataMap[tileID].tileLocation;
    }

    #region Enemy related

    public GameObject CreateSecurityCam()
    {
        return Instantiate(securityCamPref, Vector3.zero, Quaternion.identity);
    }

    public GameObject CreatePatrol()
    {
        return Instantiate(patrolPref, Vector3.zero, Quaternion.identity);
    }

    public GameObject CreateLaser()
    {
        return Instantiate(laserPref, Vector3.zero, Quaternion.identity);
    }

    #endregion

    #endregion

}

[System.Serializable]
public class LevelData
{
    public int startTileID;
    public int endTileID;
    public int secondaryObjectiveTileID;
    public int rowCount;
    public int columnCount;
    public GameRuleType gameRuleType;
}
