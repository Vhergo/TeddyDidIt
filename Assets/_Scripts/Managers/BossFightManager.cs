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
    private bool bossFightInProgress;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        Player.OnPlayerDeath += EndBossFight;
    }

    private void OnDisable()
    {
        Player.OnPlayerDeath -= EndBossFight;
    }

    public void BeginBossFight()
    {
        bossFightInProgress = true;
        BossManager.Instance.StartFiring();
        StartCoroutine(SpawnObjects());
    }

    public void EndBossFight()
    {
        bossFightInProgress = false;
        BossManager.Instance.StopFiring();
    }

    private IEnumerator SpawnObjects()
    {
        yield return new WaitUntil(() => bossFightInProgress);

        int spawnCount = 0;
        while (bossFightInProgress) {
            int objectIndex = Random.Range(0, spawnableObjects.Count);
            GameObject obj = ObjectPoolManager.Instance.GetPoolObject(spawnableObjects[objectIndex], GetRandomSpawnPoint(), GetRandomQuaternion());
            obj.transform.parent = transform;

            Destroy(obj.GetComponent<ObjectScoring>());
          
            BossRoomObject bossObj = obj.AddComponent<BossRoomObject>();
            bossObj.TriggerDestroy(objectDestroyTime);

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
        float randomXOffset = Random.Range(-offset, offset);
        return new Vector3(transform.position.x + randomXOffset, transform.position.y, transform.position.z);
    }

    private Quaternion GetRandomQuaternion()
    {
        float randomX = Random.Range(0f, 360f);
        float randomY = Random.Range(0f, 360f);
        float randomZ = Random.Range(0f, 360f);

        return Quaternion.Euler(randomX, randomY, randomZ);
    }

}
