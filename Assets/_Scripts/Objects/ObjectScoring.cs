using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectScoring : MonoBehaviour
{
    public float speedThreshold = 0.1f;
    public bool isGrabbed = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 8 && isGrabbed)
        {
            Vector3 collisionDir = collision.contacts[0].point - transform.position;
            float speed = Vector3.Dot(collisionDir.normalized, collision.relativeVelocity);
            speed = Mathf.Abs(speed / 50);

            if (speed >= speedThreshold) // Set a minimum speed threshold if needed
            {
                ScoreSystem.Instance.AddScore(speed + 1.0f, gameObject.tag, collision.gameObject.tag);
            }
            isGrabbed = false;
            Debug.Log(speed.ToString());
        }
    }
}
