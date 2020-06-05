using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;

public class SecurityCamera : MonoBehaviour
{
    [SerializeField] private float detectionArcRadius = 3f;
    [SerializeField] private float detectionArcAngle = 30f;
    [SerializeField] private float cameraPatrolAngle = 120f;
    [SerializeField] private float cameraPatrolSpeed = 30f;

    //[SerializeField] private float waitTime = 1f;
    //[SerializeField] private CameraMoveDirection[] moveDirections;

    [SerializeField] private GameObject camImage;

    int rotationDirection = -1;
    bool objectInitialized = false;

    private Vector2 leftMargin, rightMargin;
    private Vector2 camMarginOne, camMarginTwo;

    bool playerDetected = false;

    private void Start()
    {
        if(TestLevelManager.testEnvironment)
            Initialize();
    }

    public void Initialize()
    {
        leftMargin = Quaternion.AngleAxis((cameraPatrolAngle / 2) - (detectionArcAngle / 2), Vector3.forward) * camImage.transform.up;
        rightMargin = Quaternion.AngleAxis(-((cameraPatrolAngle / 2) - (detectionArcAngle / 2)), Vector3.forward) * camImage.transform.up;

        objectInitialized = true;
        playerDetected = false;
    }

    void Update()
    {
        if (objectInitialized)
        {
            Vector2 rotation;
            if (rotationDirection == 1)
            {
                rotation = Vector3.RotateTowards(camImage.transform.up, rightMargin, cameraPatrolSpeed * Time.deltaTime, 0.0f);
            }
            else
            {
                rotation = Vector3.RotateTowards(camImage.transform.up, leftMargin, cameraPatrolSpeed * Time.deltaTime, 0.0f);
            }

            camImage.transform.up = rotation;

            if (rotationDirection == 1)
            {
                if (Vector3.Angle(rotation, rightMargin) <= 0.3f)
                    rotationDirection = -1;
            }
            else
            {
                if (Vector3.Angle(rotation, leftMargin) < 0.3f)
                    rotationDirection = 1;
            }

            RecalculateMargins();

            if(!playerDetected)
                DetectIfPlayerinFOW();
        }
    }

    private void RecalculateMargins()
    {
        camMarginOne = Quaternion.AngleAxis((detectionArcAngle / 2), Vector3.forward) * camImage.transform.up;
        camMarginTwo = Quaternion.AngleAxis(-(detectionArcAngle / 2), Vector3.forward) * camImage.transform.up;
    }

    private void DetectIfPlayerinFOW()
    {
        Collider2D target = Physics2D.OverlapCircle(transform.position, detectionArcRadius, GameConsts.PLAYER_LAYER_MASK);

        if (target)
        {
            Vector2 dir = target.transform.position - transform.position;

            float angle1 = Vector2.Angle(camMarginOne, dir);
            float angle2 = Vector2.Angle(camMarginTwo, dir);

            if (Vector2.Angle(camMarginOne, dir) < detectionArcAngle && Vector2.Angle(camMarginTwo, dir) < detectionArcAngle)
            {
                GameEventManager.Instance.TriggerSyncEvent(new PlayerDetectedEvent());
                playerDetected = true;
            }
        }
    }

    #region Getters and setters

    public float GetDetectionArcRadius()
    {
        return detectionArcRadius;
    }

    public float GetDetectionArcAngle()
    {
        return detectionArcAngle;
    }

    public float GetCameraPatrolAngle()
    {
        return cameraPatrolAngle;
    }

    public float GetCameraPatrolSpeed()
    {
        return cameraPatrolSpeed;
    }

    public void SetDetectionArcRadius(float detectionArcRadius)
    {
        this.detectionArcRadius = detectionArcRadius;
    }

    public void SetDetectionArcAngle(float detectionArcAngle)
    {
        this.detectionArcAngle = detectionArcAngle;
    }

    public void SetCameraPatrolAngle(float cameraPatrolAngle)
    {
        this.cameraPatrolAngle = cameraPatrolAngle;
    }

    public void SetCameraPatrolSpeed(float cameraPatrolSpeed)
    {
        this.cameraPatrolSpeed = cameraPatrolSpeed;
    }

    public Transform GetCamImageTransform()
    {
        return camImage.transform;
    }

    #endregion
}

[System.Serializable]
public class SecurityCamDataUnit
{
    public float detectionArcRadius;
    public float detectionArcAngle;
    public float cameraPatrolAngle;
    public float cameraPatrolSpeed;

    public Vector2 position;
    public Quaternion rotation;
    public Vector3 scale;
}
