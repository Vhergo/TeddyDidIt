using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static Action OnGameOver;

    private void Awake() {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void GameOver()
    {
        OnGameOver?.Invoke();
    }
}
