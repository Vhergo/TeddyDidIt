using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FollowMousePosition : MonoBehaviour
{
    void Update()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        // Intersect the ray with the plane at fixedZPosition
        Plane plane = new Plane(Vector3.forward, new Vector3(0, 0, 0));
        if (plane.Raycast(ray, out float distance)) {
            Vector3 worldPos = ray.GetPoint(distance);
            transform.position = worldPos;
        }
    }
}
