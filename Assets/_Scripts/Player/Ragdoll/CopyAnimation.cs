using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyAnimation : MonoBehaviour
{
    [SerializeField] private Transform targetLimb;
    private ConfigurableJoint joint;

    private Quaternion startingRotation;

    private void Start()
    {
        joint = GetComponent<ConfigurableJoint>();
        startingRotation = transform.rotation;
    }

    private void Update()
    {
        joint.SetTargetRotationLocal(targetLimb.rotation, startingRotation);
    }
}
