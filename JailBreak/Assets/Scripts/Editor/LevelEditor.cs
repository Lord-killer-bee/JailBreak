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

    private LevelEditorObjectGenerator objectGenerator;

    private Dictionary<int, WallTileType> tilesMapped = new Dictionary<int, WallTileType>();

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

}
