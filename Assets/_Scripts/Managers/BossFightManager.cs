using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFightManager : MonoBehaviour
{
    public static BossFightManager Instance { get; private set; }

    [SerializeField] private float offset;
    [SerializeField] private float spawnRate;
    [SerializeField] private int burstAmount;
    [SerializeField] private int spawnCooldown;
    [SerializeField] private float objectDestroyTime;
    [SerializeField] private List<GameObject> spawnableObjects;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private Transform bossRespawnPoint;
    [SerializeField] private float bossObjectMass = 2f;
    private bool bossFightInProgress;
    private Coroutine spawnCoroutine;

    [Space(10)]
    [SerializeField] private GameObject restartUI;

    public static Action OnBossFightReset;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        Player.OnPlayerDeath += PlayerDeath;
        GameManager.OnGameOver += EndBossFight;
        BossManager.OnBossDeath += EndBossFight;
    }

    private void OnDisable()
    {
        Player.OnPlayerDeath -= PlayerDeath;
        GameManager.OnGameOver -= EndBossFight;
        BossManager.OnBossDeath -= EndBossFight;
    }

    private void Start()
    {
        restartUI.SetActive(false);
    }

    public void RestartBossFight()
    {
        Debug.Log("Restarting Boss Fight");
        StopAllCoroutines();
        RemoveAllBossRoomObjects();
        ResetPositions();
        BeginBossFight();
        UnpauseGame();
        OnBossFightReset.Invoke();
        BeginSpawningBossFightObjects();
        restartUI.SetActive(false);
    }

    private void RemoveAllBossRoomObjects()
    {
        BossRoomObject[] bossRoomObjects = FindObjectsOfType<BossRoomObject>();
        foreach (BossRoomObject obj in bossRoomObjects) {
            ObjectPoolManager.Instance.ReturnPoolObject(obj.gameObject);
        }
    }

    private void ResetPositions()
    {
        TeddyMovement.Instance.Unfreeze();
        Player.Instance.transform.position = respawnPoint.position;
        BossManager.Instance.transform.position = bossRespawnPoint.position;
    }

    private void UnpauseGame()
    {
        MySceneManager.Instance.UnpauseGame();
    }

    public void BeginSpawningBossFightObjects()
    {
        BossManager.Instance.StartBossCombatCycle();
        bossFightInProgress = true;
        spawnCoroutine = StartCoroutine(SpawnObjects());
    }

    public void BeginBossFight()
    {
        BossManager.Instance.ShowBossHealth();
        BossManager.Instance.StartBossCombatCycle();
    }

    public void EndBossFight()
    {
        bossFightInProgress = false;
        StopCoroutine(spawnCoroutine);
    }

    public void PlayerDeath()
    {
        restartUI.SetActive(true);
        EndBossFight();
    }

    private IEnumerator SpawnObjects()
    {
        yield return new WaitUntil(() => bossFightInProgress);

        int spawnCount = 0;
        while (bossFightInProgress) {
            int objectIndex = UnityEngine.Random.Range(0, spawnableObjects.Count);
            GameObject obj = ObjectPoolManager.Instance.GetPoolObject(spawnableObjects[objectIndex], GetRandomSpawnPoint(), GetRandomQuaternion());
            obj.transform.parent = transform;

            Destroy(obj.GetComponent<ObjectScoring>());
          
            BossRoomObject bossObj = obj.AddComponent<BossRoomObject>();
            bossObj.TriggerDestroy(objectDestroyTime);
            ObjectInteraction punchedOrThrown = obj.AddComponent<ObjectInteraction>();
            SetBossObjectMass(rb: obj.GetComponent<Rigidbody>());

            yield return new WaitForSeconds(spawnRate);
            spawnCount++;

            if (spawnCount >= burstAmount) {
                spawnCount = 0;
                yield return new WaitForSeconds(spawnCooldown);
            }
        }
    }

    private Vector3 GetRandomSpawnPoint()
    {
        float randomXOffset = UnityEngine.Random.Range(-offset, offset);
        return new Vector3(transform.position.x + randomXOffset, transform.position.y, transform.position.z);
    }

    private Quaternion GetRandomQuaternion()
    {
        float randomX = UnityEngine.Random.Range(0f, 360f);
        float randomY = UnityEngine.Random.Range(0f, 360f);
        float randomZ = UnityEngine.Random.Range(0f, 360f);

        return Quaternion.Euler(randomX, randomY, randomZ);
    }

    private void SetBossObjectMass(Rigidbody rb) => rb.mass = bossObjectMass;
}
