using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; private set; }

    public Texture2D cursorDefault;
    private Vector2 cursorHotspot;

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
            cursorHotspot = new Vector2(cursorDefault.width / 2, cursorDefault.height / 2);
            Cursor.SetCursor(cursorDefault, cursorHotspot, CursorMode.Auto);
        } else {
            Destroy(gameObject);
        }
    }

    public void SetCursor(Texture2D cursorTexture)
    {
        Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
    }

    public void ResetCursor()
    {
        Cursor.SetCursor(cursorDefault, cursorHotspot, CursorMode.Auto);
    }
}