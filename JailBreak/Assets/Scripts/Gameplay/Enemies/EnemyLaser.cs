using Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLaser : MonoBehaviour
{
    [SerializeField] private Transform[] laserEnds;
    [SerializeField] private LaserEndMotionData[] motionData = new LaserEndMotionData[2];

    private Vector3[] laserEndPositions = new Vector3[2];
    private bool playerDetected = false;
    private bool objectInitialized = true;

    private Vector2 target01, target02, target11, target12;
    private int[] moveDirection = new int[2];

    void Start()
    {
        //if (TestLevelManager.testEnvironment)
            Initialize();
    }

    public void Initialize()
    {
        objectInitialized = true;

        CalculateMoveTargets();
    }

    private void CalculateMoveTargets()
    {
        if (motionData[0].moveDirection == LaserMoveDirection.Horizontal)
        {
            target01 = laserEnds[0].transform.localPosition - new Vector3(motionData[0].moveDistance / 2, 0, 0);
            target02 = laserEnds[0].transform.localPosition + new Vector3(motionData[0].moveDistance / 2, 0, 0);
        }
        else if (motionData[0].moveDirection == LaserMoveDirection.Vertical)
        {
            target01 = laserEnds[0].transform.localPosition - new Vector3(0, motionData[0].moveDistance / 2, 0);
            target02 = laserEnds[0].transform.localPosition + new Vector3(0, motionData[0].moveDistance / 2, 0);
        }

        if (motionData[1].moveDirection == LaserMoveDirection.Horizontal)
        {
            target11 = laserEnds[1].transform.localPosition - new Vector3(motionData[1].moveDistance / 2, 0, 0);
            target12 = laserEnds[1].transform.localPosition + new Vector3(motionData[1].moveDistance / 2, 0, 0);
        }
        else if (motionData[1].moveDirection == LaserMoveDirection.Vertical)
        {
            target11 = laserEnds[1].transform.localPosition - new Vector3(0, motionData[1].moveDistance / 2, 0);
            target12 = laserEnds[1].transform.localPosition + new Vector3(0, motionData[1].moveDistance / 2, 0);
        }

        moveDirection[0] = 1;
        moveDirection[1] = 1;
    }

    private void Update()
    {
        if (objectInitialized)
        {
            for (int i = 0; i < motionData.Length; i++)
            {
                if (moveDirection[0] == 1)
                {
                    laserEnds[0].transform.localPosition = Vector3.MoveTowards(laserEnds[0].transform.localPosition, target02, motionData[0].moveSpeed * Time.deltaTime);

                    if (Vector3.Distance(laserEnds[0].transform.localPosition, target02) == 0)
                    {
                        moveDirection[0] *= -1;
                    }
                }
                else if(moveDirection[0] == -1)
                {
                    laserEnds[0].transform.localPosition = Vector3.MoveTowards(laserEnds[0].transform.localPosition, target01, motionData[0].moveSpeed * Time.deltaTime);

                    if (Vector3.Distance(laserEnds[0].transform.localPosition, target01) == 0)
                    {
                        moveDirection[0] *= -1;
                    }
                }

                if (moveDirection[1] == 1)
                {
                    laserEnds[1].transform.localPosition = Vector3.MoveTowards(laserEnds[1].transform.localPosition, target12, motionData[1].moveSpeed * Time.deltaTime);

                    if (Vector3.Distance(laserEnds[1].transform.localPosition, target12) == 0)
                    {
                        moveDirection[1] *= -1;
                    }
                }
                else if (moveDirection[1] == -1)
                {
                    laserEnds[1].transform.localPosition = Vector3.MoveTowards(laserEnds[1].transform.localPosition, target11, motionData[1].moveSpeed * Time.deltaTime);

                    if (Vector3.Distance(laserEnds[1].transform.localPosition, target11) == 0)
                    {
                        moveDirection[1] *= -1;
                    }
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == GameConsts.PLAYER_TAG)
        {
            GameEventManager.Instance.TriggerSyncEvent(new PlayerDetectedEvent());
            playerDetected = true;
        }
    }


    #region Getters and Setters

    public Vector3[] GetLaserEndPositions()
    {
        for (int i = 0; i < laserEndPositions.Length; i++)
        {
            laserEndPositions[i] = laserEnds[i].localPosition;
        }

        return laserEndPositions;
    }

    public void SetLaserEndPositions(Vector3[] laserEndPositions)
    {
        this.laserEndPositions = laserEndPositions;

        for (int i = 0; i < laserEndPositions.Length; i++)
        {
            laserEnds[i].localPosition = laserEndPositions[i];
        }
    }

    #endregion

}

[System.Serializable]
public class EnemyLaserDataUnit
{
    public Vector3[] laserEnds;

    public Vector2 position;
    public Quaternion rotation;
    public Vector3 scale;
}

[System.Serializable]
public class LaserEndMotionData
{
    public LaserMoveDirection moveDirection;
    public float moveSpeed;
    public float moveDistance;
}