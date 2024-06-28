using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectScoring : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
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
            ScoreSystem.Instance.addScore(gameObject.tag, collision.gameObject.tag);
            isGrabbed = false;
        }
    }
}
