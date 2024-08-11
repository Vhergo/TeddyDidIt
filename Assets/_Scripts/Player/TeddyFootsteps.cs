using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeddyFootsteps : MonoBehaviour
{
    [SerializeField] private AudioClip shoesOff;
    [SerializeField] private AudioClip shoesOn;
    [SerializeField] private AudioClip currentFootstepSound;

    private TeddyMovement teddyMovement;

    private void OnEnable() => PlayerGear.OnShoesEquipped += UpdateFootstepSound;
    private void OnDisable() => PlayerGear.OnShoesEquipped -= UpdateFootstepSound;

    private void Start()
    {
        teddyMovement = TeddyMovement.Instance;
        currentFootstepSound = shoesOff;
    }

    public void UpdateFootstepSound() => currentFootstepSound = shoesOn;
    private void PlayFootstepSound()
    {
        if (teddyMovement.IsGrounded())
            SoundManager.Instance.PlaySound(currentFootstepSound);
    }
}
