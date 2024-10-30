﻿using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.Pool;
using Zenject;

public class MeleeEnemyPool : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int enemiesPerWave = 10;
    [SerializeField] private int increaseEnemiesPerWave = 3;
    [SerializeField] private float timeBetweenWaves = 5f;
    [SerializeField] private float spawnRadius = 10f;
    [SerializeField] private float forwardSpawnBias = 0.7f; // 0 to 1, higher values spawn more enemies in front
    
    [Inject]
    [SerializeField] private  PlayerController playerController;
    
    // [Inject]
    // private EnemyManager _enemyManager;

    private Transform playerTransform;
    private ObjectPool<GameObject> enemyPool;
    public List<GameObject> activeEnemies = new List<GameObject>();
    public static float nextWaveTime;
    
    // [Inject]
    // public void Initialize(EnemyManager enemyManager)
    // {
    //     // _enemyManager = enemyManager;
    //     Debug.Log("EnemyManager injected: " + (_enemyManager != null));
    // }

    private void Start()
    {
        playerTransform = playerController.gameObject.transform;
        // if (_enemyManager == null)
        // {
        //     Debug.LogError("EnemyManager not injected!");
        //     return;
        // }

        enemyPool = new ObjectPool<GameObject>(
            createFunc: CreateEnemy,
            actionOnGet: ActivateEnemy,
            actionOnRelease: DeactivateEnemy,
            actionOnDestroy: DestroyEnemy,
            defaultCapacity: enemiesPerWave * 10,
            maxSize: enemiesPerWave * 100
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

    public void SpawnWave()
    {
        for (int i = 0; i < enemiesPerWave; i++)
        {
            GameObject enemy = enemyPool.Get();
            enemy.transform.SetParent(transform);
            PositionEnemy(enemy);
            activeEnemies.Add(enemy);
             // _enemyManager.AddEnemy(enemy);
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

        
    }

    private GameObject CreateEnemy()
    {
        GameObject enemy = Instantiate(enemyPrefab);
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
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
        }
        enemyPool.Release(enemy);
    }
}