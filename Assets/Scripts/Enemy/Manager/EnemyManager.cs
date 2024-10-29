using UnityEngine;
using System;
using System.Collections.Generic;
using Player;
using Zenject;

public class EnemyManager : IInitializable, ITickable, IDisposable
{
    public static Action<int> EnemyCountUpdated;
   

    private readonly PlayerController _playerController;
    private readonly ComputeShader _enemyComputeShader;
    private readonly MeleeEnemyPool _pool;

    private Transform _playerTransform;
    private List<GameObject> _activeEnemies = new List<GameObject>();
    private ComputeBuffer _computeBuffer;
    private ComputeBuffer _obstacleBuffer;
    private int _kernelIndex;
    private float _timeSinceLastCheck;

    public float moveSpeed = 5f;
    public float obstacleAvoidanceRadius = 1f;
    public float neighborAvoidanceRadius = 0.5f;
    public float collisionDistance = 1f;
    public float checkInterval = 0.1f;
    public float stucknessThreshold = 2f;
    

    public EnemyManager(PlayerController playerController,
        ComputeShader enemyComputeShader,
        MeleeEnemyPool meleeEnemyPool)
    {
        _playerController = playerController;
        _enemyComputeShader = enemyComputeShader;
        _pool = meleeEnemyPool;
        Debug.Log("Enemy Manager constructor Started.");

    }

    public void Initialize()
    {
        _playerTransform = _playerController.gameObject.transform;
        Debug.Log("Enemy Manager Started.");
        if (_enemyComputeShader != null)
        {
            _kernelIndex = _enemyComputeShader.FindKernel("CSMain");
        }
        InitializeObstacles();
    }

    public void Tick()
    {
        _activeEnemies = _pool.activeEnemies;
        if (_activeEnemies.Count > 0)
        {
            UpdateBuffers();
        }
        
        if (_activeEnemies.Count == 0) return;

        _timeSinceLastCheck += Time.deltaTime;

        if (_timeSinceLastCheck >= checkInterval)
        {
            CheckEnemyCollisions();
            _timeSinceLastCheck = 0f;
        }

        UpdateEnemyPositions();
    }

    private void UpdateEnemyPositions()
    {
        if (_computeBuffer == null || _obstacleBuffer == null || _activeEnemies.Count <= 0) return;

        // Update enemy positions in the buffer
        EnemyData[] enemyDataArray = new EnemyData[_activeEnemies.Count];
        for (int i = 0; i < _activeEnemies.Count; i++)
        {
            Vector3 position = _activeEnemies[i].transform.position;
            enemyDataArray[i] = new EnemyData
            {
                position = new Vector2(position.x, position.y),
                velocity = Vector2.zero,
                stuckness = 0, // Initialize stuckness
                damage = 10,
                health = 100,
                isAlive = 1
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
            _activeEnemies[i].transform.position = new Vector3(enemyDataArray[i].position.x, enemyDataArray[i].position.y, 0);
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
                
                if (_pool != null)
                {
                    _pool.ReleaseEnemy(enemy);
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
            _computeBuffer = new ComputeBuffer(_activeEnemies.Count, 32); // 2 for position, 2 for velocity, 1 for stuckness
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