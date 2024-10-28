using UnityEngine;
using UnityEngine.Pool;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int enemiesPerWave = 10;
    [SerializeField] private int increaseEnemiesPerWave = 3;
    [SerializeField] private float timeBetweenWaves = 5f;
    [SerializeField] private float spawnRadius = 10f;
    [SerializeField] private float forwardSpawnBias = 0.7f; // 0 to 1, higher values spawn more enemies in front

    [Header("References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private EnemyManager enemyManager;

    private ObjectPool<GameObject> enemyPool;
    private float nextWaveTime;

    private void Start()
    {
        enemyPool = new ObjectPool<GameObject>(
            createFunc: CreateEnemy,
            actionOnGet: ActivateEnemy,
            actionOnRelease: DeactivateEnemy,
            actionOnDestroy: DestroyEnemy,
            defaultCapacity: enemiesPerWave,
            maxSize: enemiesPerWave * 3
        );

        nextWaveTime = Time.time + timeBetweenWaves;
    }

    private void Update()
    {
        if (Time.time >= nextWaveTime)
        {
            SpawnWave();
            nextWaveTime = Time.time + timeBetweenWaves;
        }
    }

    private void SpawnWave()
    {
        for (int i = 0; i < enemiesPerWave; i++)
        {
            GameObject enemy = enemyPool.Get();
            enemy.transform.SetParent(transform);
            PositionEnemy(enemy);
            enemyManager.AddEnemy(enemy);
        }

        enemiesPerWave += increaseEnemiesPerWave;
    }

    private void PositionEnemy(GameObject enemy)
    {
        // Generate a random angle around a circle
        float angle = Random.Range(0f, 2f * Mathf.PI);

        // Use the full spawn radius for a more uniform distribution
        float distanceFromCenter = spawnRadius;

        // Calculate spawn position
        Vector3 spawnOffset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * distanceFromCenter;
        Vector3 spawnPosition = playerTransform.position + spawnOffset;

        enemy.transform.position = spawnPosition;

        // Set initial movement direction towards the player
        Vector3 directionToPlayer = (playerTransform.position - spawnPosition).normalized;
        enemy.GetComponent<EnemyBehavior>().SetInitialDirection(directionToPlayer);
    }

    private GameObject CreateEnemy()
    {
        GameObject enemy = Instantiate(enemyPrefab);
        enemy.GetComponent<EnemyBehavior>().SetPool(enemyPool);
        enemy.GetComponent<EnemyBehavior>().SetSpawner(this);
        return enemy;
    }

    private void ActivateEnemy(GameObject enemy)
    {
        enemy.SetActive(true);
    }

    private void DeactivateEnemy(GameObject enemy)
    {
        enemy.SetActive(false);
    }

    private void DestroyEnemy(GameObject enemy)
    {
        Destroy(enemy);
    }

    public void ReleaseEnemy(GameObject enemy)
    {
        enemyManager.RemoveEnemy(enemy);
        enemyPool.Release(enemy);
    }
}