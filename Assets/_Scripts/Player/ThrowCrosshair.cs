using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowCrosshair : MonoBehaviour
{
    [SerializeField] private Texture2D crosshairTexture;

    private void OnEnable()
    {
        CombatSystem.OnGrab += ShowCrosshair;
        CombatSystem.OnThrow += HideCrosshair;
    }

    private void OnDisable()
    {
        CombatSystem.OnGrab -= ShowCrosshair;
        CombatSystem.OnThrow -= HideCrosshair;
    }

    private void ShowCrosshair()
    {
        CursorManager.Instance.SetCursor(crosshairTexture);
    }

    private void HideCrosshair()
    {
        CursorManager.Instance.ResetCursor();
    }
}
