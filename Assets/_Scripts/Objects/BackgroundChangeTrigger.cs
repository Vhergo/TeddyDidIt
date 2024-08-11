using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class BackgroundChangeTrigger : MonoBehaviour
{
    [SerializeField] private Sprite newBackground;
    [SerializeField] private AudioClip newBackgroundMusic;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            SequenceBackgroundManager.Instance.ChangeBackground(newBackground);
            SoundManager.Instance.TriggerSwitchMusic(newBackgroundMusic);
            Destroy(gameObject);
        }
    }
}
