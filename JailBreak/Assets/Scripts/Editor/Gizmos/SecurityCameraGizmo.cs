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
}
