using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PatrollingEnemy))]
public class PatrollingEnemyGizmo : Editor
{
    private PatrollingEnemy targetObject;

    private Vector3[] waypoints;

    private void OnEnable()
    {
        targetObject = target as PatrollingEnemy;
    }

    private void OnSceneGUI()
    {
        waypoints = targetObject.GetWayPoints();

        Handles.color = new Color(1.0f, 1.0f, 1.0f, 0.8f);
        Handles.DrawAAPolyLine(waypoints);

        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = Handles.PositionHandle(waypoints[i], Quaternion.identity);
        }

        Vector2 size = targetObject.GetComponent<BoxCollider2D>().size;
        Vector2 offset = targetObject.GetComponent<BoxCollider2D>().offset;

        size.y = 7.5f + ((targetObject.GetDetectionTileRange() - 1) * 5);
        offset.y = size.y / 2;

        targetObject.GetComponent<BoxCollider2D>().size = size;
        targetObject.GetComponent<BoxCollider2D>().offset = offset;
    }
}
