using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectScoring : MonoBehaviour
{
    public bool isGrabbed = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 8 && isGrabbed)
        {
            Vector3 collisionDir = collision.contacts[0].point - transform.position;
            float speed = Vector3.Dot(collisionDir.normalized, collision.relativeVelocity);
            speed = Mathf.Abs(speed/50);
            ScoreSystem.Instance.addScore(speed+1.0f,gameObject.tag, collision.gameObject.tag);
            isGrabbed = false;
            Debug.Log(speed.ToString());
        }
    }
}
