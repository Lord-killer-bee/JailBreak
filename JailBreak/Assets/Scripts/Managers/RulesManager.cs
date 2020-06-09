using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;
using System;

public class RulesManager : MonoBehaviour
{
    int currentLevelID;

    bool secondaryObjectiveAchieved = false;
    private LevelData currentLevelData;

    private void OnEnable()
    {
        GameEventManager.Instance.AddListener<PlayerMovedToTileEvent>(OnPlayerMovedToTile);
        GameEventManager.Instance.AddListener<SetCurrentLevelID>(OnCurrentLevelIDrecieved);
    }

    private void OnDisable()
    {
        GameEventManager.Instance.RemoveListener<PlayerMovedToTileEvent>(OnPlayerMovedToTile);
        GameEventManager.Instance.RemoveListener<SetCurrentLevelID>(OnCurrentLevelIDrecieved);
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