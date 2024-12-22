﻿using System.Collections.Generic;
using Enemy.Models;
using Player.Controllers;
using Player.Views;
using UnityEngine;
using UnityEngine.Pool;
using Zenject;

namespace Enemy.Services
{
    public class ShamanEnemyPool: MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private ShamanEnemy enemyPrefab;
        [SerializeField] private int enemiesPerWave = 10;
        [SerializeField] private int increaseEnemiesPerWave = 3;
        [SerializeField] private float timeBetweenWaves = 5f;
        [SerializeField] private float spawnRadius = 10f;
    
        [Inject]
        [SerializeField] private  PlayerController playerController;

        [Inject] private PlayerView _playerView;
        [Inject] private EnemyProjectilePoolManager _enemyProjectilePoolManager;
    
        // [Inject]
        // private EnemyManager _enemyManager;

        private Transform playerTransform;
        private ObjectPool<BaseEnemy> enemyPool;
        public List<BaseEnemy> activeEnemies = new List<BaseEnemy>();
        public static float nextWaveTime;
    
        // [Inject]
        // public void Initialize(EnemyManager enemyManager)
        // {
        //     // _enemyManager = enemyManager;
        //     Debug.Log("EnemyManager injected: " + (_enemyManager != null));
        // }

        private void Start()
        {
            playerTransform = _playerView.gameObject.transform;
            // if (_enemyManager == null)
            // {
            //     Debug.LogError("EnemyManager not injected!");
            //     return;
            // }

            enemyPool = new ObjectPool<BaseEnemy>(
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
            // if (Time.time >= nextWaveTime)
            // {
            //     SpawnWave();
            //     nextWaveTime = Time.time + timeBetweenWaves;
            // }    
        }
    
        public void SpawnWave()
        {
            for (int i = 0; i < enemiesPerWave; i++)
            {
                // SpawnEnemy();
                // _enemyManager.AddEnemy(enemy);
            }

            enemiesPerWave += increaseEnemiesPerWave;
        }

        public BaseEnemy CreateShamanEnemy(ShamanEnemyData shamanEnemyData)
        {
            BaseEnemy enemy = enemyPool.Get();
            enemy.transform.SetParent(transform);
            PositionEnemy(enemy);
            activeEnemies.Add(enemy);
        
            var rangedEnemy = enemy.GetComponent<ShamanEnemy>();
            rangedEnemy.AttackRange = shamanEnemyData.AttackRange;
            rangedEnemy.MaxHealth = shamanEnemyData.Health;
            rangedEnemy.Damage = shamanEnemyData.Damage;
            rangedEnemy.shamanRadius = shamanEnemyData.ShamanRadius;
            rangedEnemy.shamanInterval = shamanEnemyData.ShamanInterval;
            rangedEnemy.MinDistanceToPlayer = shamanEnemyData.AttackRange - 1f;
            enemy.Initialize();
            return enemy;
        }

        private void PositionEnemy(BaseEnemy enemy)
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

        private BaseEnemy CreateEnemy()
        {
            BaseEnemy enemy = Instantiate(enemyPrefab);
            return enemy;
        }

        private void ActivateEnemy(BaseEnemy enemy)
        {
            enemy.gameObject.SetActive(true);
        }

        private void DeactivateEnemy(BaseEnemy enemy)
        {
            enemy.gameObject.SetActive(false);
        }

        private void DestroyEnemy(BaseEnemy enemy)
        {
            Destroy(enemy);
        }

        public void ReleaseEnemy(BaseEnemy enemy)
        {
            RemoveFromActiveEnemies(enemy);
            enemyPool.Release(enemy);
        }

        public void RemoveFromActiveEnemies(BaseEnemy enemy)
        {
            if (activeEnemies.Contains(enemy))
            {
                activeEnemies.Remove(enemy);
            }
        }
        public void AddToActiveEnemies(BaseEnemy enemy)
        {
            if (!activeEnemies.Contains(enemy))
            {
                activeEnemies.Add(enemy);
            }
        }
    }
}