using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchedOrThrown : MonoBehaviour
{
    public bool punchedOrThrown;

    public void ResetDelayed(float resetDelay = 2f)
    {
        Invoke("ResetNow", resetDelay);
    }

    public void ResetNow() {
        if (gameObject != null) punchedOrThrown = false;
    }
}
