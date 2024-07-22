using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPositioning : MonoBehaviour
{
    [SerializeField] private Transform grabbedObjectPosition;

    private void Update()
    {
        transform.position = grabbedObjectPosition.position;
    }

    public void SetGrabPosition(Transform grabPosition)
    {
        grabbedObjectPosition = grabPosition;
    }
}
