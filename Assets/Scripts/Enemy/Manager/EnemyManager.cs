using UnityEngine;
using System;
using System.Collections.Generic;
using Player;
using Unity.Mathematics;
using Zenject;

public class EnemyManager : IInitializable, ITickable, IDisposable
{
    public static Action<int> EnemyCountUpdated;
   

    private readonly PlayerController _playerController;
    private readonly ComputeShader _enemyComputeShader;
    private readonly MeleeEnemyPool _meleeEnemyPool;

    private Transform _playerTransform;
    private List<GameObject> _activeEnemies = new List<GameObject>();
    private ComputeBuffer _computeBuffer;
    private ComputeBuffer _obstacleBuffer;
    private int _kernelIndex;
    private float _timeSinceLastCheck;
    private float enemySpawningInterval = 1f;
    private float nextEnemySpawnTime;

    public float moveSpeed = 4f;
    public float obstacleAvoidanceRadius = 1f;
    public float neighborAvoidanceRadius = 1f;
    public float collisionDistance = 1f;
    public float checkInterval = 0.1f;
    public float stucknessThreshold = 2f;
    

    public EnemyManager(PlayerController playerController,
        ComputeShader enemyComputeShader,
        MeleeEnemyPool meleeEnemyMeleeEnemyPool)
    {
        _playerController = playerController;
        _enemyComputeShader = enemyComputeShader;
        _meleeEnemyPool = meleeEnemyMeleeEnemyPool;
        Debug.Log("Enemy Manager constructor Started.");

    }

    public void Initialize()
    {
        _playerTransform = _playerController.transform;
        nextEnemySpawnTime = enemySpawningInterval + Time.time;
        Debug.Log("Enemy Manager Started.");
        if (_enemyComputeShader != null)
        {
            _kernelIndex = _enemyComputeShader.FindKernel("CSMain");
        }
        InitializeObstacles();
    }

    private void CreateEnemyFromMeleeEnemyPool()
    {
        var meleeAttacker = new MeleeAttacker()
        {
            Damage = 15,
            Health = 100,
            IsAlive = 1,
            DistanceToPlayer = 9999f
        };
        _meleeEnemyPool.CreateMeleeEnemy(meleeAttacker);
    }
    public void Tick()
    {
        HandleEnemySpawning();
        _activeEnemies = _meleeEnemyPool.activeEnemies;
        if (_activeEnemies.Count > 0)
        {
            UpdateBuffers();
        }
        
        if (_activeEnemies.Count == 0) return;

        _timeSinceLastCheck += Time.deltaTime;

        if (_timeSinceLastCheck >= checkInterval)
        {
            // CheckEnemyCollisions();
            _timeSinceLastCheck = 0f;
        }

        UpdateEnemyPositions();
        if (Input.GetMouseButtonDown(0))
        {
            for (int i = 0; i < _activeEnemies.Count; i++)
            {
                var enemy = _activeEnemies[i];
                var enemyStat = enemy.GetComponent<MeleeEnemy>().GetMeleeAttackerStat();
                if (enemyStat.DistanceToPlayer < 2f)
                {
                    _meleeEnemyPool.ReleaseEnemy(enemy);
                }
            }

        }
    }

    private void HandleEnemySpawning()
    {
        if (nextEnemySpawnTime < Time.time )
        {
            CreateEnemyFromMeleeEnemyPool();
            nextEnemySpawnTime = Time.time + enemySpawningInterval;
        }

        
    }

    private void UpdateEnemyPositions()
    {
        if (_computeBuffer == null || _obstacleBuffer == null || _activeEnemies.Count <= 0) return;

        // Update enemy positions in the buffer
        EnemyData[] enemyDataArray = new EnemyData[_activeEnemies.Count];
        for (int i = 0; i < _activeEnemies.Count; i++)
        {
            Vector3 position = _activeEnemies[i].transform.position;
            var enemyStat = _activeEnemies[i].GetComponent<MeleeEnemy>().GetMeleeAttackerStat();
            enemyDataArray[i] = new EnemyData
            {
                position = enemyStat.Position,
                velocity = enemyStat.Velocity,
                stuckness = enemyStat.Stuckness, // Initialize stuckness
                damage = enemyStat.Damage,
                health = enemyStat.Health,
                isAlive = enemyStat.IsAlive
            };
        }
        _computeBuffer.SetData(enemyDataArray);

        // Set compute shader parameters
        _enemyComputeShader.SetBuffer(_kernelIndex, "enemyBuffer", _computeBuffer);
        _enemyComputeShader.SetBuffer(_kernelIndex, "obstacleBuffer", _obstacleBuffer);
        _enemyComputeShader.SetVector("playerPosition", _playerTransform.position);
        _enemyComputeShader.SetFloat("deltaTime", Time.deltaTime);
        _enemyComputeShader.SetFloat("moveSpeed", moveSpeed);
        _enemyComputeShader.SetFloat("obstacleAvoidanceRadius", obstacleAvoidanceRadius);
        _enemyComputeShader.SetFloat("neighborAvoidanceRadius", neighborAvoidanceRadius);
        _enemyComputeShader.SetInt("enemyCount", _activeEnemies.Count);
        _enemyComputeShader.SetInt("obstacleCount", _obstacleBuffer.count);
        _enemyComputeShader.SetFloat("stucknessThreshold", stucknessThreshold);

        // Dispatch the compute shader
        int threadGroups = Mathf.CeilToInt(_activeEnemies.Count / 256f);
        _enemyComputeShader.Dispatch(_kernelIndex, threadGroups, 1, 1);

        // Get results back and update enemy positions
        _computeBuffer.GetData(enemyDataArray);
        for (int i = 0; i < _activeEnemies.Count; i++)
        {
            // _activeEnemies[i].transform.position = new Vector3(enemyDataArray[i].position.x, enemyDataArray[i].position.y, 0);
            var enemyData = enemyDataArray[i];
            var enemyStat = new MeleeAttacker();
            enemyStat.ApplyComputeData(enemyData);
            var enemy = _activeEnemies[i].GetComponent<MeleeEnemy>();
            enemy.SetMeleeAttackerStat(enemyStat);
            if (enemyStat.DistanceToPlayer < 0.01f)
            {
                enemy.HandleAttack(_playerController);
            }
        }
    }

    private void CheckEnemyCollisions()
    {
        Vector2 playerPosition = _playerTransform.position;
        float sqrCollisionDistance = collisionDistance * collisionDistance;

        for (int i = _activeEnemies.Count - 1; i >= 0; i--)
        {
            Vector2 enemyPosition = _activeEnemies[i].transform.position;
            if ((playerPosition - enemyPosition).sqrMagnitude <= sqrCollisionDistance)
            {
                // Collision detected, return enemy to pool
                GameObject enemy = _activeEnemies[i];
                _activeEnemies.RemoveAt(i);
                
                if (_meleeEnemyPool != null)
                {
                    _meleeEnemyPool.ReleaseEnemy(enemy);
                }
                // else
                // {
                //     Debug.LogWarning("EnemySpawner not set. Destroying enemy instead.");
                //     Destroy(enemy);
                // }
            }
        }
    }

    private void InitializeObstacles()
    {
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        Vector2[] obstaclePositions = new Vector2[obstacles.Length];
        for (int i = 0; i < obstacles.Length; i++)
        {
            obstaclePositions[i] = obstacles[i].transform.position;
        }

        _obstacleBuffer = new ComputeBuffer(obstaclePositions.Length, sizeof(float) * 2);
        _obstacleBuffer.SetData(obstaclePositions);
    }

    public void AddEnemy(GameObject enemy)
    {
        _activeEnemies.Add(enemy);
        UpdateBuffers();
    }

    public void RemoveEnemy(GameObject enemy)
    {
        _activeEnemies.Remove(enemy);
        UpdateBuffers();
    }

    private void UpdateBuffers()
    {
        if (_computeBuffer != null)
        {
            _computeBuffer.Release();
        }
        
        if (_activeEnemies.Count > 0)
        {
            _computeBuffer = new ComputeBuffer(_activeEnemies.Count, 36); // 2 for position, 2 for velocity, 1 for stuckness
        }
        else
        {
            _computeBuffer = null;
        }
        EnemyCountUpdated?.Invoke(_activeEnemies.Count);
    }

    private void OnDestroy()
    {
        if (_computeBuffer != null)
        {
            _computeBuffer.Release();
        }
        if (_obstacleBuffer != null)
        {
            _obstacleBuffer.Release();
        }
    }

    public void Dispose()
    {
        if (_computeBuffer != null)
        {
            _computeBuffer.Release();
        }
        if (_obstacleBuffer != null)
        {
            _obstacleBuffer.Release();
        }
    }
}