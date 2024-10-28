using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInteraction : MonoBehaviour
{
    public ObjectInteractionState currentState = ObjectInteractionState.None;

    public void SetPunched()
    {
        currentState = ObjectInteractionState.Punched;
        ResetDelayed();
    }

    public void SetThrown()
    {
        currentState = ObjectInteractionState.Thrown;
        ResetDelayed();
    }

    public void SetCharged()
    {
        currentState = ObjectInteractionState.Charged;
        ResetDelayed();
    }

    public bool CheckCurrentState(ObjectInteractionState state)
    {
        return currentState == state;
    }

    public void ResetDelayed(float resetDelay = 2f)
    {
        Invoke("ResetNow", resetDelay);
    }

    public void ResetNow() {
        if (gameObject != null) {
            currentState = ObjectInteractionState.None;
        }
    }
}

[Serializable]
public enum  ObjectInteractionState
{
    None,
    Punched,
    Thrown,
    Charged
}
