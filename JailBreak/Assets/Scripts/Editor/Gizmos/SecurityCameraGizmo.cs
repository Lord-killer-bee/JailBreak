using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(SecurityCamera))]
public class SecurityCameraGizmo : Editor
{
    private SecurityCamera targetObject;

    private void OnEnable()
    {
        targetObject = target as SecurityCamera;
    }

    private void OnSceneGUI()
    {
        Handles.color = new Color(0.8f, 0.1f, 0.1f, 0.3f);
        Handles.DrawSolidArc(targetObject.transform.position, Vector3.back, Quaternion.AngleAxis(targetObject.GetCameraPatrolAngle() / 2, Vector3.forward) * targetObject.transform.up, targetObject.GetCameraPatrolAngle(), targetObject.GetDetectionArcRadius());

        Handles.color = new Color(0.1f, 0.8f, 0.1f, 0.8f);
        Handles.DrawSolidArc(targetObject.transform.position, Vector3.back, Quaternion.AngleAxis(targetObject.GetDetectionArcAngle() / 2, Vector3.forward) * targetObject.GetCamImageTransform().up, targetObject.GetDetectionArcAngle(), targetObject.GetDetectionArcRadius());
    }
}
