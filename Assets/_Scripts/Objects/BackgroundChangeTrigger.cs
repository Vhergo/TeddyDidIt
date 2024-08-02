using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class BackgroundChangeTrigger : MonoBehaviour
{
    [SerializeField] private Sprite newBackground;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            SequenceBackgroundManager.Instance.ChangeBackground(newBackground);
            Destroy(gameObject);
        }
    }
}
