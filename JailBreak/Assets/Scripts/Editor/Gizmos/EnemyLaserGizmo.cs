using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyLaser))]
public class EnemyLaserGizmo : Editor
{
    private EnemyLaser targetObject;

    private Vector2[] laserEnds = new Vector2[2];
    private Vector3[] recievedEnds;

    private void OnEnable()
    {
        targetObject = target as EnemyLaser;
    }

    private void OnSceneGUI()
    {
        recievedEnds = targetObject.GetLaserEndPositions();

        for (int i = 0; i < laserEnds.Length; i++)
        {
            laserEnds[i] = targetObject.transform.position - recievedEnds[i];
        }

        EdgeCollider2D collider = targetObject.GetComponent<EdgeCollider2D>();
        collider.points = targetObject.GetLaserEndLocalPositons();

        LineRenderer line = targetObject.GetComponent<LineRenderer>();
        line.positionCount = 2;
        line.SetPositions(recievedEnds);
    }
}
