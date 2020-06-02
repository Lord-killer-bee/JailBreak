using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLaser : MonoBehaviour
{
    [SerializeField] private Transform[] laserEnds;

    private Vector3[] laserEndPositions = new Vector3[2];
    private bool playerDetected = false;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == GameConsts.PLAYER_TAG)
        {
            GameEventManager.Instance.TriggerSyncEvent(new PlayerDetectedEvent());
            playerDetected = true;
        }
    }

    public Vector3[] GetLaserEndPositions()
    {
        for (int i = 0; i < laserEndPositions.Length; i++)
        {
            laserEndPositions[i] = laserEnds[i].position;
        }

        return laserEndPositions;
    }
}
