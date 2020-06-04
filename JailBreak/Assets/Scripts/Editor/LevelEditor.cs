using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class LevelEditor : EditorWindow
{
    private int rowCount;
    private int columnCount;

    private int startTileID;

    private int endTileID;
    private int keyTileID;
    private int stationTileID;

    public GameRuleType ruleType;

    public List<int> wallTileIds;

    private int levelID;
    private LevelData levelData;

    private LevelData loadLevelData;

    private LevelEditorObjectGenerator objectGenerator;

    [MenuItem("Window/Level editor")]
    private static void OpenWindow()
    {
        LevelEditor window = (LevelEditor)GetWindow(typeof(LevelEditor));
        window.Show();
    }

    private void OnEnable()
    {
        objectGenerator = GameObject.FindObjectOfType<LevelEditorObjectGenerator>();
    }

    private void OnGUI()
    {
        ScriptableObject scriptableObj = this;
        SerializedObject serialObj = new SerializedObject(scriptableObj);

        //Grid creation
        EditorGUILayout.HelpBox("Create a grid with column and rows", MessageType.Info);

        rowCount = EditorGUILayout.IntField(new GUIContent("rows "), rowCount);
        columnCount = EditorGUILayout.IntField(new GUIContent("columns "), columnCount);

        if (GUILayout.Button("Generate grid"))
        {
            CreateOrUpdateGrid();
        }

        if (GUILayout.Button("Clear level"))
        {
            ClearLevel();
        }

        //Start and end tiles
        EditorGUILayout.HelpBox("Select player start and end tiles", MessageType.Info);

        SerializedProperty ruleTypeProp = serialObj.FindProperty("ruleType");

        EditorGUILayout.PropertyField(ruleTypeProp, true);
        serialObj.ApplyModifiedProperties();

        startTileID = EditorGUILayout.IntField(new GUIContent("Start tile ID "), startTileID);

        if (ruleType == GameRuleType.DirectExit)
        {
            endTileID = EditorGUILayout.IntField(new GUIContent("End tile ID "), endTileID);
        }
        else if (ruleType == GameRuleType.PickKeyThenExit)
        {
            endTileID = EditorGUILayout.IntField(new GUIContent("End tile ID "), endTileID);
            keyTileID = EditorGUILayout.IntField(new GUIContent("Key tile ID "), keyTileID);
        }
        else if (ruleType == GameRuleType.HackStationThenExit)
        {
            endTileID = EditorGUILayout.IntField(new GUIContent("End tile ID "), endTileID);
            stationTileID = EditorGUILayout.IntField(new GUIContent("Station tile ID "), stationTileID);
        }

        //Wall tiles
        EditorGUILayout.HelpBox("Select wall tiles", MessageType.Info);

        SerializedProperty serialProp = serialObj.FindProperty("wallTileIds");

        EditorGUILayout.PropertyField(serialProp, true);
        serialObj.ApplyModifiedProperties();

        //Enemy selection
        EditorGUILayout.HelpBox("Create enemies using buttons", MessageType.Info);

        if (GUILayout.Button("Generate Patrol"))
        {
            CreateNewPatrolEnemy();
        }

        if (GUILayout.Button("Generate Security camera"))
        {
            CreateNewSecurityCamera();
        }

        if (GUILayout.Button("Generate laser"))
        {
            CreateNewLaser();
        }

        //Save the level in assigned object
        EditorGUILayout.HelpBox("Save the level", MessageType.Info);
        levelID = EditorGUILayout.IntField(new GUIContent("Level ID "), levelID);
        levelData = EditorGUILayout.ObjectField("Level object", levelData, typeof(LevelData), true) as LevelData;

        if (GUILayout.Button("Save level"))
        {
            SaveTheLevel();
        }

        //Load a level
        EditorGUILayout.HelpBox("Load an existing level", MessageType.Info);
        loadLevelData = EditorGUILayout.ObjectField("Level object", loadLevelData, typeof(LevelData), true) as LevelData;

        if (GUILayout.Button("Load level"))
        {
            LoadTheLevel();
        }
    }

    private void CreateNewLaser()
    {
        objectGenerator.CreateLaser();
    }

    private void CreateNewSecurityCamera()
    {
        objectGenerator.CreateSecurityCam();
    }

    private void CreateNewPatrolEnemy()
    {
        objectGenerator.CreatePatrol();
    }

    private void CreateOrUpdateGrid()
    {
        objectGenerator.CreateGrid(rowCount, columnCount);
    }

    private void ClearLevel()
    {
        objectGenerator.ClearLevel();
    }

    private void LoadTheLevel()
    {
        if(loadLevelData == null)
        {
            Debug.Log("Level data should be specified");
            return;
        }

        objectGenerator.ClearLevel();

        rowCount = loadLevelData.rowCount;
        columnCount = loadLevelData.columnCount;

        startTileID = loadLevelData.startTileID;
        endTileID = loadLevelData.endTileID;

        ruleType = loadLevelData.ruleType;
        keyTileID = loadLevelData.keyTileID;
        stationTileID = loadLevelData.stationTileID;

        wallTileIds = loadLevelData.wallTileIds;

        for (int i = 0; i < loadLevelData.securityCamsdata.Count; i++)
        {
            GameObject cam = objectGenerator.CreateSecurityCam();
            SecurityCamera camComp = cam.GetComponent<SecurityCamera>();

            camComp.SetCameraPatrolAngle(loadLevelData.securityCamsdata[i].cameraPatrolAngle);
            camComp.SetCameraPatrolSpeed(loadLevelData.securityCamsdata[i].cameraPatrolSpeed);
            camComp.SetDetectionArcAngle(loadLevelData.securityCamsdata[i].detectionArcAngle);
            camComp.SetDetectionArcRadius(loadLevelData.securityCamsdata[i].detectionArcRadius);

            cam.transform.position = loadLevelData.securityCamsdata[i].position;
            cam.transform.rotation = loadLevelData.securityCamsdata[i].rotation;
            cam.transform.localScale = loadLevelData.securityCamsdata[i].scale;
        }

        for (int i = 0; i < loadLevelData.patrollersdata.Count; i++)
        {
            GameObject patrol = objectGenerator.CreatePatrol();
            PatrollingEnemy patrolComp = patrol.GetComponent<PatrollingEnemy>();

            patrolComp.SetDetectionTileRange(loadLevelData.patrollersdata[i].detectionTileRange);
            patrolComp.SetMoveSpeed(loadLevelData.patrollersdata[i].moveSpeed);
            patrolComp.SetWayPoints(loadLevelData.patrollersdata[i].waypoints);

            patrol.transform.position = loadLevelData.patrollersdata[i].position;
            patrol.transform.rotation = loadLevelData.patrollersdata[i].rotation;
            patrol.transform.localScale = loadLevelData.patrollersdata[i].scale;
        }

        for (int i = 0; i < loadLevelData.lasersdata.Count; i++)
        {
            GameObject laser = objectGenerator.CreateLaser();
            EnemyLaser laserComp = laser.GetComponent<EnemyLaser>();

            laserComp.SetLaserEndPositions(loadLevelData.lasersdata[i].laserEnds);

            laser.transform.position = loadLevelData.lasersdata[i].position;
            laser.transform.rotation = loadLevelData.lasersdata[i].rotation;
            laser.transform.localScale = loadLevelData.lasersdata[i].scale;
        }

        CreateOrUpdateGrid();
    }

    private void SaveTheLevel()
    {
        levelData.rowCount = rowCount;
        levelData.columnCount = columnCount;

        levelData.startTileID = startTileID;
        levelData.endTileID = endTileID;
        levelData.keyTileID = keyTileID;
        levelData.stationTileID = stationTileID;
        levelData.ruleType = ruleType;

        levelData.wallTileIds = wallTileIds;

        //Security cam data
        List<SecurityCamera> cams = FindObjectsOfType<SecurityCamera>().ToList();
        levelData.securityCamsdata.Clear();

        for (int i = 0; i < cams.Count; i++)
        {
            levelData.securityCamsdata.Add(new SecurityCamDataUnit());
            levelData.securityCamsdata[i].cameraPatrolAngle = cams[i].GetCameraPatrolAngle();
            levelData.securityCamsdata[i].cameraPatrolSpeed = cams[i].GetCameraPatrolSpeed();
            levelData.securityCamsdata[i].detectionArcAngle = cams[i].GetDetectionArcAngle();
            levelData.securityCamsdata[i].detectionArcRadius = cams[i].GetDetectionArcRadius();

            levelData.securityCamsdata[i].position = cams[i].transform.position;
            levelData.securityCamsdata[i].rotation = cams[i].transform.rotation;
            levelData.securityCamsdata[i].scale = cams[i].transform.localScale;
        }

        //Laser data
        List<EnemyLaser> lasers = FindObjectsOfType<EnemyLaser>().ToList();
        levelData.lasersdata.Clear();

        for (int i = 0; i < lasers.Count; i++)
        {
            levelData.lasersdata.Add(new EnemyLaserDataUnit());
            levelData.lasersdata[i].laserEnds = lasers[i].GetLaserEndPositions();

            levelData.lasersdata[i].position = lasers[i].transform.position;
            levelData.lasersdata[i].rotation = lasers[i].transform.rotation;
            levelData.lasersdata[i].scale = lasers[i].transform.localScale;
        }

        //Patrol data
        List<PatrollingEnemy> patrollers = FindObjectsOfType<PatrollingEnemy>().ToList();
        levelData.patrollersdata.Clear();

        for (int i = 0; i < patrollers.Count; i++)
        {
            levelData.patrollersdata.Add(new PatrollingEnemyDataUnit());
            levelData.patrollersdata[i].moveSpeed = patrollers[i].GetMoveSpeed();
            levelData.patrollersdata[i].detectionTileRange = patrollers[i].GetDetectionTileRange();
            levelData.patrollersdata[i].waypoints = patrollers[i].GetWayPoints();

            levelData.patrollersdata[i].position = patrollers[i].transform.position;
            levelData.patrollersdata[i].rotation = patrollers[i].transform.rotation;
            levelData.patrollersdata[i].scale = patrollers[i].transform.localScale;
        }

        //Setting dirty
        EditorUtility.SetDirty(levelData);
    }

}
