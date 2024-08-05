using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoomObject : MonoBehaviour
{
    public float destroyTime;

    public void TriggerDestroy(float timer)
    {
        destroyTime = timer;
        Invoke("DestroyBossRoomObject", timer);
    }

    private void DestroyBossRoomObject()
    {
        // Spawn particles
        Destroy(gameObject);
    }
}
