using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;

public class SecurityCamera : MonoBehaviour
{
    [SerializeField] private float waitTime = 1f;
    [SerializeField] private List<CameraMoveLocation> moveLocations;
    [SerializeField] private CameraMoveLocation startLocation = CameraMoveLocation.Top;
    [SerializeField] private CameraRotationDirection rotationDirection = CameraRotationDirection.Clockwise;

    [SerializeField] private GameObject camImage;

    CameraMoveLocation currentLocation;
    bool objectInitialized = false;
    bool waitStarted = false;
    float waitUnits = 0;

    #region Base methods

    private void Start()
    {
        if(TestController.testEnvironment)
            Initialize();
    }

    private void OnEnable()
    {
        GameEventManager.Instance.AddListener<PathDrawingCompleteEvent>(OnPathDrawingComplete);
        GameEventManager.Instance.AddListener<GameStateChangedEvent>(OnGameStateChanged);
    }

    private void OnDisable()
    {
        GameEventManager.Instance.RemoveListener<PathDrawingCompleteEvent>(OnPathDrawingComplete);
        GameEventManager.Instance.RemoveListener<GameStateChangedEvent>(OnGameStateChanged);
    }

    public void Initialize()
    {
        objectInitialized = true;

        waitStarted = true;
        waitUnits = waitTime;

        currentLocation = startLocation;
        camImage.transform.localEulerAngles = GetRotationForDirection(currentLocation);
    }

    void LateUpdate()
    {
        if (moveLocations.Count == 0)
            return;

        if (objectInitialized)
        {
            //if ((DateTime.Now - waitStartedTime).TotalMilliseconds >= waitTime * 1000)
            if (GameTimer.GameTicked)
            {
                if (waitStarted)
                {
                    waitUnits--;

                    if (waitUnits <= 0)
                        waitStarted = false;
                }
            }

            if(!waitStarted)
            {
                currentLocation = GetNextMoveDirection(currentLocation, rotationDirection);
                camImage.transform.localEulerAngles = GetRotationForDirection(currentLocation);
                waitStarted = true;
                waitUnits = waitTime;
            }
        }
    }

    void ResetEnemy()
    {
        waitStarted = true;
        waitUnits = waitTime;

        currentLocation = startLocation;
        camImage.transform.localEulerAngles = GetRotationForDirection(currentLocation);
    }

    #endregion

    #region Event listeners

    private void OnPathDrawingComplete(PathDrawingCompleteEvent e)
    {
        if (TestController.testEnvironment)
            ResetEnemy();
    }

    private void OnGameStateChanged(GameStateChangedEvent e)
    {
        //if(e.stateType == GameStateType.SimulateLevel)
        //    ResetEnemy();
    }

    #endregion

    #region Helper methods

    public CameraMoveLocation GetNextMoveDirection(CameraMoveLocation currentDirection, CameraRotationDirection rotationDirection)
    {
        CameraMoveLocation result = CameraMoveLocation.None;

        switch (currentDirection)
        {
            case CameraMoveLocation.Left:
                if (rotationDirection == CameraRotationDirection.Clockwise)
                    result = CameraMoveLocation.Top;
                else
                    result = CameraMoveLocation.Bottom;
                break;
            case CameraMoveLocation.Right:
                if (rotationDirection == CameraRotationDirection.Clockwise)
                    result = CameraMoveLocation.Bottom;
                else
                    result = CameraMoveLocation.Top;
                break;
            case CameraMoveLocation.Top:
                if (rotationDirection == CameraRotationDirection.Clockwise)
                    result = CameraMoveLocation.Right;
                else
                    result = CameraMoveLocation.Left;
                break;
            case CameraMoveLocation.Bottom:
                if (rotationDirection == CameraRotationDirection.Clockwise)
                    result = CameraMoveLocation.Left;
                else
                    result = CameraMoveLocation.Right;
                break;
        }

        if (!moveLocations.Contains(result))
        {
            result = GetNextMoveDirection(result, rotationDirection);
        }

        return result;
    }

    public Vector3 GetRotationForDirection(CameraMoveLocation moveDirection)
    {
        switch (moveDirection)
        {
            case CameraMoveLocation.Left:
                return new Vector3(0, 0, 90);
            case CameraMoveLocation.Right:
                return new Vector3(0, 0, -90);
            case CameraMoveLocation.Top:
                return Vector3.zero;
            case CameraMoveLocation.Bottom:
                return new Vector3(0, 0, 180);
        }

        return Vector3.zero;
    }

    #endregion

    #region Getters and setters

    public Transform GetCamImageTransform()
    {
        return camImage.transform;
    }

    public float GetWaitTime()
    { 
        return waitTime;
    }

    public List<CameraMoveLocation> GetMoveLocations()
    {
        return moveLocations;
    }

    public CameraMoveLocation GetStartLocation()
    {
        return startLocation;
    }

    public CameraRotationDirection GetRotationDirection()
    {
        return rotationDirection;
    }

    public void SetWaitTime(float waitTime)
    {
        this.waitTime = waitTime;
    }

    public void SetMoveLocations(List<CameraMoveLocation> moveLocations)
    {
        this.moveLocations = moveLocations;
    }

    public void SetStartLocation(CameraMoveLocation startLocation)
    {
        this.startLocation = startLocation;
    }

    public void SetRotationDirection(CameraRotationDirection rotationDirection)
    {
        this.rotationDirection = rotationDirection;
    }

    #endregion
}