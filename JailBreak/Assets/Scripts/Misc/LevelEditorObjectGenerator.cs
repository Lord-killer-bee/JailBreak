using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditorObjectGenerator : MonoBehaviour
{
    #region Grid generation

    [SerializeField] private GameObject baseTilePref;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject plotter;

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

    #endregion

    #region Enemy related

    [SerializeField] private GameObject securityCamPref;
    [SerializeField] private GameObject patrolPref;
    [SerializeField] private GameObject laserPref;

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
}
