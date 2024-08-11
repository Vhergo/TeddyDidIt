using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectScoring : MonoBehaviour
{
    public float speedThreshold = 0.1f;
    public bool isGrabbed = false;
    public bool isInteracted;
    [SerializeField] private List<AudioClip> collisonSounds;


    public void PlayCollisionSound()
    {
        if (collisonSounds.Count == 0) return;

        int randomIndex = Random.Range(0, collisonSounds.Count);
        BackupAudioSource.Instance.PlaySoundFromBackupSource(collisonSounds[randomIndex], true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 8 && isInteracted)
        {
            Vector3 collisionDir = collision.contacts[0].point - transform.position;
            float speed = Vector3.Dot(collisionDir.normalized, collision.relativeVelocity);
            speed = Mathf.Abs(speed / 50);

            if (speed >= speedThreshold && isGrabbed) // Set a minimum speed threshold if needed
            {
                ScoreSystem.Instance.AddScore(speed + 1.0f, gameObject.tag, collision.gameObject.tag);
            }
            PlayCollisionSound();
            isGrabbed = false;
            isInteracted = false;
            Debug.Log(speed.ToString());
        }
    }
}
