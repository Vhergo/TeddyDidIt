using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressionSystem : MonoBehaviour
{
    public static ProgressionSystem Instance { get; private set; }

    [Tooltip("Add to list in order of progression")]
    [SerializeField] private List<GameObject> progressionIndicators = new List<GameObject>();
    [SerializeField] private RectTransform progressionBar;
    [SerializeField] private bool allowProgressionShortcut;

    [Header("Achivement")]
    [SerializeField] private Image achievementIcon;
    [SerializeField] private TMP_Text achievementTitle;
    [SerializeField] private Image achievementGuide;
    [Space(10)]
    [SerializeField] private Sprite powerPunch;
    [SerializeField] private Sprite grabAndThrow;
    [SerializeField] private Sprite doubleJump;
    [SerializeField] private Sprite chargeThrow;
    [Space(10)]
    [SerializeField] private Sprite powerPunchGuide;
    [SerializeField] private Sprite grabAndThrowGuide;
    [SerializeField] private Sprite doubleJumpGuide;
    [SerializeField] private Sprite chargeThrowGuide;
    [Space(10)]
    [SerializeField] private AudioClip achievementSound;
    [SerializeField] private Animator achievementAnim;
    [SerializeField] private AnimationClip achievementShow;
    [SerializeField] private AnimationClip achievementHide;
    [SerializeField] private Button achievementCloseButton;
    [SerializeField] private float autoHideDelay = 20f;

    private ProgressStage currentState;

    public static Action OnProgressionChanged;
    public static Action OnPunchEnabled;
    public static Action OnGrabAndThrowEnabled;
    public static Action OnDoubleJumpEnabled;
    public static Action OnChargeThrowEnabled;

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        BossManager.OnBossDeath += () => SetProgressionStage(ProgressStage.Base);
    }

    private void OnDisable()
    {
        BossManager.OnBossDeath -= () => SetProgressionStage(ProgressStage.Base);
    }

    private void Start()
    {
        InitializeUI();
        SetProgressionStage(ProgressStage.Base);
    }

    private void Update()
    {
        if (!allowProgressionShortcut) return;

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            SetProgressionStage(ProgressStage.Base);
        } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            SetProgressionStage(ProgressStage.Punch);
        } else if (Input.GetKeyDown(KeyCode.Alpha3)) {
            SetProgressionStage(ProgressStage.GrabAndThrow);
        } else if (Input.GetKeyDown(KeyCode.Alpha4)) {
            SetProgressionStage(ProgressStage.DoubleJump);
        } else if (Input.GetKeyDown(KeyCode.Alpha5)) {
            SetProgressionStage(ProgressStage.ChargeThrow);
        }
#endif
    }

    private void InitializeUI()
    {
        foreach (GameObject indicator in progressionIndicators) {
            indicator.transform.GetChild(0).GetComponent<Image>().enabled = false;
        }

        achievementCloseButton.onClick.AddListener(HideAchievementNotification);
    }

    public void SetProgressionStage(ProgressStage stage)
    {
        // Debug.Log("Entering Stage: " + stage.ToString());
        switch (stage) {
            case ProgressStage.Base:
                currentState = ProgressStage.Base;
                ActivateUIIndicator(0);
                break;
            case ProgressStage.Punch:
                currentState = ProgressStage.Punch;
                OnPunchEnabled?.Invoke();
                ActivateUIIndicator(1);

                AchievementNotification(powerPunch, "Power Punch", powerPunchGuide);
                break;
            case ProgressStage.GrabAndThrow:
                currentState = ProgressStage.GrabAndThrow;
                OnGrabAndThrowEnabled?.Invoke();
                ActivateUIIndicator(2);

                AchievementNotification(grabAndThrow, "Grab And Throw", grabAndThrowGuide);
                break;
            case ProgressStage.DoubleJump:
                currentState = ProgressStage.DoubleJump;
                OnDoubleJumpEnabled?.Invoke();
                ActivateUIIndicator(3);

                AchievementNotification(doubleJump, "Double Jump", doubleJumpGuide);
                break;
            case ProgressStage.ChargeThrow:
                currentState = ProgressStage.ChargeThrow;
                OnChargeThrowEnabled?.Invoke();
                ActivateUIIndicator(4);

                AchievementNotification(chargeThrow, "Charged Throw", chargeThrowGuide);
                break;
        }
    }

    public ProgressStage GetCurrentStage()
    {
        return currentState;
    }

    private void ActivateUIIndicator(int index)
    {
        OnProgressionChanged?.Invoke();
        progressionIndicators[index].transform.GetChild(0).GetComponent<Image>().enabled = true;
    }

    public void SetProgressIndicatorPositions(int punch, int grab, int jump, int charge)
    {
        float barWidth = progressionBar.rect.width;

        float punchPercent = (float)punch / charge;
        float grabPercent = (float)grab / charge;
        float jumpPercent = (float)jump / charge;
        float chargePercent = (float)charge / charge;
        List<float> percentages = new List<float> { 0, punchPercent, grabPercent, jumpPercent, chargePercent };

        float startX = progressionIndicators[0].GetComponent<RectTransform>().anchoredPosition.x;

        for (int i = 0; i < progressionIndicators.Count; i++) {
            // Debug.Log(percentages[i]);
            progressionIndicators[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(startX + (barWidth * percentages[i]), 0, 0);
        }
    }

    private void AchievementNotification(Sprite icon, string title, Sprite guide)
    {
        achievementIcon.sprite = icon;
        achievementTitle.text = title;
        achievementGuide.sprite = guide;
        achievementGuide.GetComponent<RectTransform>().sizeDelta = new Vector2(guide.rect.width, guide.rect.height);

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySound(achievementSound);

        achievementAnim.Play(achievementShow.name);
        Invoke("AutoHideAchievementNotification", autoHideDelay);

    }

    private void HideAchievementNotification()
    {
        achievementAnim.Play(achievementHide.name);
    }

    private void AutoHideAchievementNotification()
    {
        Debug.Log("Auto Hide Achievement Notification");
        if (achievementAnim.GetCurrentAnimatorStateInfo(0).IsName("AchievementShow")) {
            HideAchievementNotification();
        }
    }
}

[Serializable]
public enum ProgressStage
{
    Base,
    Punch,
    GrabAndThrow,
    DoubleJump,
    ChargeThrow
}
