using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance;

    [Header("Game Over")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject dialogueUI;

    [Header("Menu")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Button exitToMenuButton;

    public static Action OnMenuOpen;
    public static Action OnMenuClose;

    private void Awake() {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        GameManager.OnGameOver += GameOver;
    }

    private void OnDisable()
    {
        GameManager.OnGameOver -= GameOver;
    }

    private void Start() {
        if (exitToMenuButton != null) exitToMenuButton.onClick.AddListener(OnExitToMenuButtonClick);
        gameOverPanel.SetActive(false);
    }

    private void GameOver()
    {
        gameOverPanel.SetActive(true);
        dialogueUI.SetActive(false);
        MySceneManager.Instance.PauseGame();
    }

    public void TurnOnSettings() {
        settingsPanel.SetActive(true);
        OnMenuOpen?.Invoke();
    }

    public void TurnOffSettings() {
        settingsPanel.SetActive(false);
        OnMenuClose?.Invoke();
    }

    private void OnExitToMenuButtonClick() {
        MySceneManager.Instance.SwitchScene(SceneEnum.MainMenuScene);
    }

}
