using System.Collections;
using UnityEngine;

public class CameraWobble : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float radius = 0.2f;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float smoothTime = 0.5f;

    private Vector3 startingPosition;
    private Vector3 targetPosition;
    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        startingPosition = transform.position;
        PickNewTargetPosition();
    }

    private void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f) {
            PickNewTargetPosition();
        }
    }

    private void PickNewTargetPosition()
    {
        Vector3 randomOffset = Random.insideUnitSphere * radius;
        targetPosition = startingPosition + randomOffset;
    }
}
