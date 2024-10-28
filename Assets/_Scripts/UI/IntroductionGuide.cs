using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class IntroductionGuide : MonoBehaviour
{
    public static IntroductionGuide Instance { get; private set; }

    [SerializeField] private List<GameObject> panels = new List<GameObject>();
    [SerializeField] private Canvas dialogueCanvas;
    [SerializeField] private Button closeButton;

    private HashSet<int> viewedPanels = new HashSet<int>();
    private GameObject currentPanel;
    private int currentIndex = 0;
    private bool introPlayed;
    private bool panelClosed;
    public bool IntroPlayed => introPlayed;
    public bool PanelClosed => panelClosed;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        foreach (GameObject panel in panels) {
            panel.SetActive(false);
        }
        currentPanel = panels[currentIndex];

        if (closeButton != null) closeButton.onClick.AddListener(EndIntroductionGuide);
        else Debug.Log("Close button is not assigned");

        closeButton.gameObject.SetActive(false);
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.T)) StartIntroductionGuide();
        //if (Input.GetKeyDown(KeyCode.Alpha8)) NextPanel();
        //if (Input.GetKeyDown(KeyCode.Alpha9)) PreviousPanel();
        //if (Input.GetKeyDown(KeyCode.Alpha0)) EndIntroductionGuide();
    }

    public void TriggerIntroductionBasedOnSequence()
    {
        if (!introPlayed) {
            StartIntroductionGuide();
            introPlayed = true;
        }
    }

    public void StartIntroductionGuide()
    {
        TeddyMovement.Instance.Freeze();
        HandlePostProcessing.Instance.EnablePostProcessing();
        HandlePostProcessing.Instance.ToggleDarkenVolume(true);
        currentPanel.SetActive(true);
        currentIndex = 0;
        panelClosed = false;

        dialogueCanvas.enabled = false;
    }

    public void NextPanel()
    {
        currentPanel.SetActive(false);
        currentIndex++;
        currentIndex %= panels.Count;
        currentPanel = panels[currentIndex];
        currentPanel.SetActive(true);

        AddCurrentPanelToViewedPanels();
    }

    public void PreviousPanel()
    {
        currentPanel.SetActive(false);
        currentIndex--;
        currentIndex = currentIndex < 0 ? panels.Count - 1 : currentIndex;
        currentPanel = panels[currentIndex];
        currentPanel.SetActive(true);

        AddCurrentPanelToViewedPanels();
    }

    public void EndIntroductionGuide()
    {
        TeddyMovement.Instance.Unfreeze();
        HandlePostProcessing.Instance.DisablePostProcessing();
        HandlePostProcessing.Instance.ToggleDarkenVolume(false);
        currentPanel.SetActive(false);
        currentIndex = 0;
        closeButton.gameObject.SetActive(false);
        viewedPanels.Clear();
        panelClosed = true;

        dialogueCanvas.enabled = true;
    }

    private void AddCurrentPanelToViewedPanels()
    {
        viewedPanels.Add(currentIndex);
        CheckIfAllPanelsHaveBeenViewed();
    }

    private void CheckIfAllPanelsHaveBeenViewed()
    {
        if (viewedPanels.Count == panels.Count - 1) closeButton.gameObject.SetActive(true);
    }
}
