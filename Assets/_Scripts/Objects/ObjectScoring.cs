using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectScoring : MonoBehaviour
{
    public bool isGrabbed = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 8 && isGrabbed)
        {
            Vector3 collisionDir = collision.contacts[0].point - transform.position;
            float speed = Vector3.Dot(collisionDir.normalized, collision.relativeVelocity);
            speed = Mathf.Abs(speed/50);
            // Can set minimum speed to get a score here
            ScoreSystem.Instance.AddScore(speed + 1.0f,gameObject.tag, collision.gameObject.tag);
            isGrabbed = false;
            Debug.Log(speed.ToString());
        }
    }
}
