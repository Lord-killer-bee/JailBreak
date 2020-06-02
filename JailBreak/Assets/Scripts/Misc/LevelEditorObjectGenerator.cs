using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditorObjectGenerator : MonoBehaviour
{
    #region Grid generation

    [SerializeField] private GameObject baseTilePref;

    private Dictionary<int, TileData> tileDataMap = new Dictionary<int, TileData>();

    private List<GameObject> tiles = new List<GameObject>();

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

            tiles.Add(tile);

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

    public int GetTileID(int row, int column, int maxColumns)
    {
        return column + (row) * (maxColumns);
    }

    public void RemoveGrid()
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            DestroyImmediate(tiles[i]);
        }

        tiles.Clear();
        tileDataMap.Clear();
    }

    #endregion

    #region Enemy related

    [SerializeField] private GameObject securityCamPref;
    [SerializeField] private GameObject patrolPref;
    [SerializeField] private GameObject laserPref;

    public void CreateSecurityCam()
    {
        Instantiate(securityCamPref, Vector3.zero, Quaternion.identity);
    }

    public void CreatePatrol()
    {
        Instantiate(patrolPref, Vector3.zero, Quaternion.identity);
    }

    public void CreateLaser()
    {
        Instantiate(laserPref, Vector3.zero, Quaternion.identity);
    }
    #endregion
}
