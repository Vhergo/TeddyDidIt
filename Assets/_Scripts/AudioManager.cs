using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource01;
    public AudioSource audioSource02;
    public AudioSource audioSource03;
    public AudioSource audioSource04;
    // Boss Music
    public AudioSource audioSource05;

    public static AudioManager instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSource01.Play();
    }

    public void PlayNext(AudioSource audioSourcePrev, AudioSource audioSourceNext)
    {
        StartCoroutine(AudioFade.StartCrossFade(audioSourcePrev, audioSourceNext, 5.0f));
    }

    public static class AudioFade
    {

        public static IEnumerator StartCrossFade(AudioSource audioSourcePrev,AudioSource audioSourceNext, float duration)
        {
            float currentTime = 0;
            float startPrev = audioSourcePrev.volume;
            float startNext = 0.0f;
            audioSourceNext.Play();
            while (currentTime < duration)
            {   
                if(audioSourcePrev.volume <= 0.0f)
                {
                    audioSourcePrev.Stop();
                }
                currentTime += Time.deltaTime;
                audioSourcePrev.volume = Mathf.Lerp(startPrev, 0.0f, currentTime / duration);
                audioSourceNext.volume = Mathf.Lerp(startNext, 1.0f, currentTime / duration);
                yield return null;
            }
            yield break;
        }
    }
}
