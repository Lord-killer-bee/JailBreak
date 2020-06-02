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
        //Grid creation
        EditorGUILayout.HelpBox("Create a grid with column and rows", MessageType.Info);

        rowCount = EditorGUILayout.IntField(new GUIContent("rows "), rowCount);
        columnCount = EditorGUILayout.IntField(new GUIContent("columns "), columnCount);

        if (GUILayout.Button("Generate grid"))
        {
            CreateOrUpdateGrid();
        }

        if (GUILayout.Button("Clear grid"))
        {
            RemoveGrid();
        }

        //Start and end tiles
        EditorGUILayout.HelpBox("Select enemy start and end tiles", MessageType.Info);

        startTileID = EditorGUILayout.IntField(new GUIContent("Start tile ID "), startTileID);
        endTileID = EditorGUILayout.IntField(new GUIContent("End tile ID "), endTileID);

        //Wall tiles
        EditorGUILayout.HelpBox("Select wall tiles", MessageType.Info);

        ScriptableObject scriptableObj = this;
        SerializedObject serialObj = new SerializedObject(scriptableObj);
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

    private void RemoveGrid()
    {
        objectGenerator.RemoveGrid();
    }

    private void LoadTheLevel()
    {
        throw new NotImplementedException();
    }

    private void SaveTheLevel()
    {
        levelData.rowCount = rowCount;
        levelData.columnCount = columnCount;

        levelData.startTileID = startTileID;
        levelData.endTileID = endTileID;

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
        }

        //Laser data
        List<EnemyLaser> lasers = FindObjectsOfType<EnemyLaser>().ToList();
        levelData.lasersdata.Clear();

        for (int i = 0; i < lasers.Count; i++)
        {
            levelData.lasersdata.Add(new EnemyLaserDataUnit());
            levelData.lasersdata[i].laserEnds = lasers[i].GetLaserEndPositions();
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
        }

        //Setting dirty
        EditorUtility.SetDirty(levelData);
    }

}
