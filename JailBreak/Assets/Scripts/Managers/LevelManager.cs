using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    #region Constants

    [SerializeField] private int gridRows = 10;
    [SerializeField] private int gridColumns = 10;

    #endregion

    #region Scene refs

    [SerializeField] private GameObject baseTilePref;
    [SerializeField] private GameObject startPoint;
    [SerializeField] private GameObject endPoint;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject plotter;

    #endregion

    #region Private variables
    private Dictionary<int, TileData> tileDataMap = new Dictionary<int, TileData>();

    #endregion

    #region Base methods

    private void Start()
    {
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

            tile.GetComponent<Tile>().tileID = data.Value.tileID;
        }
    }

    /// <summary>
    /// Calculates the cell locations and tileIDs for the grid
    /// </summary>
    void CalculateCellLocations()
    {
        float x = 0, y = 0;
        int tileID;

        for (int row = 0; row < gridRows; row++)
        {
            for (int column = 0; column < gridColumns; column++)
            {
                x = column - ((float)gridColumns - 1) / 2;
                y = ((float)gridRows - 1) / 2 - row;

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
        return column + (row) * (gridColumns);
    }

    /// <summary>
    /// Returns the row number for a tileID
    /// </summary>
    /// <param name="tileID"></param>
    /// <returns></returns>
    public int GetRow(int tileID)
    {
        return tileID / gridColumns;
    }

    /// <summary>
    /// Returns a column number for tileID
    /// </summary>
    /// <param name="tileID"></param>
    /// <returns></returns>
    public int GetColumn(int tileID)
    {
        return tileID - GetRow(tileID) * gridColumns;
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

        if (column != gridColumns - 1)
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

        if (row != gridRows - 1)
            return tileDataMap[GetTileID(row + 1, column)];
        else
            return null;
    }

    #endregion

    #region Level related

    private void SetupLevelElements()
    {
        player.transform.position = startPoint.transform.position;
        plotter.transform.position = startPoint.transform.position;
    }

    #endregion

}

/// <summary>
/// The enum denoting the type for a tile
/// </summary>
public enum TileType
{
    None,
}

/// <summary>
/// Class to store the data required for a tile
/// </summary>
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