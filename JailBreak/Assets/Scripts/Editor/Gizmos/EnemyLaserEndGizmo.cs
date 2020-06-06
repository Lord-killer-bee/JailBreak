using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyLaserEnd))]
public class EnemyLaserEndGizmo : Editor
{
    private EnemyLaserEnd targetObject;

    private Vector3[] waypoints;

    private void OnEnable()
    {
        targetObject = target as EnemyLaserEnd;
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
    }
}
