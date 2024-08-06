using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionSFX : MonoBehaviour
{
    [SerializeField] private AudioSource explosionSFX;
    public static ExplosionSFX instance;
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    public void playSFX()
    {
        SoundManager.Instance.PlaySound(explosionSFX.clip);
    }
}
