using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCameraCollision : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == GameConsts.PLAYER_TAG)
        {
            GameEventManager.Instance.TriggerSyncEvent(new PlayerDetectedEvent());
        }
    }
}
