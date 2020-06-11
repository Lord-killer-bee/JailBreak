using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;
using UnityEngine.SceneManagement;
using System.Linq;

/// <summary>
/// Load the scene as per the Level ID
/// Creates player and plotter based on states and level data specified in the loaded level
/// </summary>
public class LevelManager : MonoBehaviour
{
    #region Scene refs

    [SerializeField] private GameObject playerPref;
    [SerializeField] private GameObject plotterPref;

    private int currentLevelID;
    private LevelData currentLevelData;

    private string[] gameScenes = new string[] {"Level_Start", "Level_Guard", "Level_Camera", "Level_B" };

    #endregion

    #region Private variables
    private Dictionary<int, TileData> tileDataMap = new Dictionary<int, TileData>();
    private GameObject player, plotter;

    #endregion

    #region Base methods

    private void Start()
    {
        currentLevelID = 0;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLevelLoaded;

        GameEventManager.Instance.AddListener<GameStateChangedEvent>(OnGameStateChanged);
        GameEventManager.Instance.AddListener<ResetPlotterEvent>(OnResetPlotter);
        GameEventManager.Instance.AddListener<RestartLevelEvent>(OnRestartLevel);
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLevelLoaded;

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
            case GameStateType.LoadScene:
                AddSceneLevel();
                break;
            case GameStateType.LevelSetup:

                ClearLevel();
                RetreiveCurrentLevelData();
                SetupLevelElements();

                GameEventManager.Instance.TriggerSyncEvent(new GameStateCompletedEvent(GameStateType.LevelSetup));
                break;
            case GameStateType.ExamineLevel:
                GameEventManager.Instance.TriggerSyncEvent(new SetCurrentLevelID(currentLevelID));
                break;
            case GameStateType.Plotting:
                plotter.SetActive(true);
                plotter.GetComponent<PathPlotter>().Initialize();
                break;
            case GameStateType.SimulateLevel:
                plotter.GetComponent<PathPlotter>().DestroyPlotterLine();
                plotter.SetActive(false);
                break;
            case GameStateType.TransitionToNextLevel:
                StartCoroutine(LoadNextLevel());
                break;
        }
    }

    private void AddSceneLevel()
    {
        SceneManager.LoadSceneAsync(gameScenes[currentLevelID], LoadSceneMode.Additive);
    }

    private void OnSceneLevelLoaded(Scene scene, LoadSceneMode sceneMode)
    {
        if (gameScenes.Contains(scene.name))
        {
            GameEventManager.Instance.TriggerSyncEvent(new GameStateCompletedEvent(GameStateType.LoadScene));
        }
    }

    private void OnResetPlotter(ResetPlotterEvent e)
    {
        plotter.transform.position = tileDataMap[currentLevelData.startTileID].tileLocation;
        plotter.GetComponent<PathPlotter>().ResetPlotter();
        plotter.GetComponent<PathPlotter>().SetInitialAllowedTiles(currentLevelData.startTileID, tileDataMap[currentLevelData.startTileID].neighbouringTiles);
    }

    private void OnRestartLevel(RestartLevelEvent e)
    {
        player.transform.position = tileDataMap[currentLevelData.startTileID].tileLocation;
        plotter.transform.position = tileDataMap[currentLevelData.startTileID].tileLocation;
        plotter.GetComponent<PathPlotter>().ResetPlotter();
        plotter.GetComponent<PathPlotter>().SetInitialAllowedTiles(currentLevelData.startTileID, tileDataMap[currentLevelData.startTileID].neighbouringTiles);
        plotter.SetActive(false);
    }

    private void RetreiveCurrentLevelData()
    {
        currentLevelData = FindObjectOfType<LevelEditorObjectGenerator>().GetLevelData();

        tileDataMap.Clear();

        Tile[] tiles = FindObjectsOfType<Tile>();

        for (int i = 0; i < tiles.Length; i++)
        {
            tileDataMap.Add(tiles[i].tileData.tileID, tiles[i].tileData);
        }

        TestController.testEnvironment = false;

        foreach (Camera cam in FindObjectsOfType<Camera>())
        {
            if(cam.tag != "MainCamera")
            {
                cam.gameObject.SetActive(false);
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
        return column + (row) * (currentLevelData.columnCount);
    }

    /// <summary>
    /// Returns the row number for a tileID
    /// </summary>
    /// <param name="tileID"></param>
    /// <returns></returns>
    public int GetRow(int tileID)
    {
        return tileID / currentLevelData.columnCount;
    }

    /// <summary>
    /// Returns a column number for tileID
    /// </summary>
    /// <param name="tileID"></param>
    /// <returns></returns>
    public int GetColumn(int tileID)
    {
        return tileID - GetRow(tileID) * currentLevelData.columnCount;
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

        if (column != currentLevelData.columnCount - 1)
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

        if (row != currentLevelData.rowCount - 1)
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
        player.transform.position = tileDataMap[currentLevelData.startTileID].tileLocation;

        plotter = Instantiate(plotterPref);
        plotter.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        plotter.transform.position = tileDataMap[currentLevelData.startTileID].tileLocation;
        plotter.GetComponent<PathPlotter>().SetInitialAllowedTiles(currentLevelData.startTileID, tileDataMap[currentLevelData.startTileID].neighbouringTiles);

        plotter.SetActive(false);

        //Initialize all the enemies
        SecurityCamera[] cams = FindObjectsOfType<SecurityCamera>();

        for (int i = 0; i < cams.Length; i++)
        {
            cams[i].Initialize();
        }

        PatrollingEnemy[] patrollers = FindObjectsOfType<PatrollingEnemy>();

        for (int i = 0; i < patrollers.Length; i++)
        {
            patrollers[i].Initialize();
        }

        EnemyLaser[] lasers = FindObjectsOfType<EnemyLaser>();

        for (int i = 0; i < lasers.Length; i++)
        {
            lasers[i].Initialize();
        }
    }

    private IEnumerator LoadNextLevel()
    {
        SceneManager.UnloadSceneAsync(gameScenes[currentLevelID], UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);

        currentLevelID++;

        yield return new WaitForSeconds(2.0f);

        GameEventManager.Instance.TriggerSyncEvent(new GameStateCompletedEvent(GameStateType.TransitionToNextLevel));
    }

    private void ClearLevel()
    {
        Destroy(player);

        if (plotter != null)
        {
            plotter.GetComponent<PathPlotter>().DestroyPlotterLine();
            Destroy(plotter);
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

    public List<int> neighbouringTiles = new List<int>();

    public TileData() { }

    public TileData(TileType tileType, Vector2 tileLocation)
    {
        this.tileType = tileType;
        this.tileLocation = tileLocation;
    }
}