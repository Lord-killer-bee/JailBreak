using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;
using System;

public class RulesManager : MonoBehaviour
{
    int currentLevelID;

    [SerializeField] private GameObject left;
    [SerializeField] private GameObject right;
    [SerializeField] private GameObject top;
    [SerializeField] private GameObject bottom;

    bool secondaryObjectiveAchieved = false;
    private LevelData currentLevelData;

    private void OnEnable()
    {
        GameEventManager.Instance.AddListener<PlayerMovedToTileEvent>(OnPlayerMovedToTile);
        GameEventManager.Instance.AddListener<SetCurrentLevelID>(OnCurrentLevelIDrecieved);
        GameEventManager.Instance.AddListener<PlotterPlottedPoint>(OnPlotterPlottedPoint);
        GameEventManager.Instance.AddListener<GameStateChangedEvent>(OnGameStateChanged);
    }

    private void OnDisable()
    {
        GameEventManager.Instance.RemoveListener<PlayerMovedToTileEvent>(OnPlayerMovedToTile);
        GameEventManager.Instance.RemoveListener<SetCurrentLevelID>(OnCurrentLevelIDrecieved);
        GameEventManager.Instance.RemoveListener<PlotterPlottedPoint>(OnPlotterPlottedPoint);
        GameEventManager.Instance.RemoveListener<GameStateChangedEvent>(OnGameStateChanged);
    }

    private void OnCurrentLevelIDrecieved(SetCurrentLevelID e)
    {
        currentLevelID = e.levelID;
        currentLevelData = FindObjectOfType<LevelEditorObjectGenerator>().GetLevelData();
    }

    private void OnPlayerMovedToTile(PlayerMovedToTileEvent e)
    {
        switch (currentLevelData.gameRuleType)
        {
            case GameRuleType.DirectExit:
                if(e.tileID == currentLevelData.endTileID)
                {
                    GameEventManager.Instance.TriggerSyncEvent(new PlayerCompletedLevelEvent());
                }
                break;
            case GameRuleType.PickKeyThenExit:
                if (e.tileID == currentLevelData.secondaryObjectiveTileID)
                {
                    secondaryObjectiveAchieved = true;
                }
                else if (e.tileID == currentLevelData.endTileID && secondaryObjectiveAchieved)
                {
                    GameEventManager.Instance.TriggerSyncEvent(new PlayerCompletedLevelEvent());
                }
                break;
            case GameRuleType.HackStationThenExit:
                if (e.tileID == currentLevelData.secondaryObjectiveTileID)
                {
                    secondaryObjectiveAchieved = true;
                }
                else if (e.tileID == currentLevelData.endTileID && secondaryObjectiveAchieved)
                {
                    GameEventManager.Instance.TriggerSyncEvent(new PlayerCompletedLevelEvent());
                }
                break;
        }
    }

    private void OnPlotterPlottedPoint(PlotterPlottedPoint e)
    {
        left.SetActive(false);
        right.SetActive(false);
        top.SetActive(false);
        bottom.SetActive(false);

        for (int i = 0; i < e.tileData.neighbouringTiles.Count; i++)
        {
            if (e.tileData.tileID - e.tileData.neighbouringTiles[i] == 1)//Left indicator
            {
                left.SetActive(true);
                left.transform.position = e.tileData.tileLocation - new Vector2(1, 0);
            }

            if (e.tileData.tileID - e.tileData.neighbouringTiles[i] == -1)//Right indicator
            {
                right.SetActive(true);
                right.transform.position = e.tileData.tileLocation + new Vector2(1, 0);
            }

            if (e.tileData.tileID - e.tileData.neighbouringTiles[i] == currentLevelData.columnCount)//Top indicator
            {
                top.SetActive(true);
                top.transform.position = e.tileData.tileLocation + new Vector2(0, 1);
            }

            if (e.tileData.tileID - e.tileData.neighbouringTiles[i] == -currentLevelData.columnCount)//Bottom indicator
            {
                bottom.SetActive(true);
                bottom.transform.position = e.tileData.tileLocation - new Vector2(0, 1);
            }
        }
    }

    private void OnGameStateChanged(GameStateChangedEvent e)
    {
        if(e.stateType == GameStateType.SimulateLevel)
        {
            left.SetActive(false);
            right.SetActive(false);
            top.SetActive(false);
            bottom.SetActive(false);
        }
    }

}

public struct DirectExitGame
{
    public int exitTileID;
}

public struct PickKeyThenExitGame
{
    public int keyTileID;
    public int exitTileID;
}

public struct HackStationThenExitGame
{
    public int stationTileID;
    public int exitTileID;
}