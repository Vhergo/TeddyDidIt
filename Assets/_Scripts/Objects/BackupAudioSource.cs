using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackupAudioSource : MonoBehaviour
{
    public static BackupAudioSource Instance { get; private set; }

    [SerializeField] private AudioSource backupAudioSource;
    [SerializeField] private Vector2 pitchVariation = new Vector2(0.8f, 1.2f);

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        backupAudioSource = GetComponent<AudioSource>();
    }

    public void PlaySoundFromBackupSource(AudioClip clip, bool randomPitch = false)
    {
        if (randomPitch) backupAudioSource.pitch = Random.Range(pitchVariation.x, pitchVariation.y);
        backupAudioSource.clip = clip;
        backupAudioSource.Play();
    }
}
