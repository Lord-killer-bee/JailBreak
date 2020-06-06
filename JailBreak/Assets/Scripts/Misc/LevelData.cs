using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Level data asset")]
public class LevelData : ScriptableObject
{
    public int rowCount;
    public int columnCount;
    
    public int startTileID;
    public int endTileID;
    public int keyTileID;
    public int stationTileID;

    public GameRuleType ruleType;

    public Dictionary<int, WallTileType> wallTiles = new Dictionary<int, WallTileType>();

    public List<SecurityCamDataUnit> securityCamsdata = new List<SecurityCamDataUnit>(); 
    public List<EnemyLaserDataUnit> lasersdata = new List<EnemyLaserDataUnit>(); 
    public List<PatrollingEnemyDataUnit> patrollersdata = new List<PatrollingEnemyDataUnit>(); 
    //Add more enemy types if created
}
