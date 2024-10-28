using System;
using UnityEngine;
using System.Collections.Generic;
using ModestTree.Util;

public partial class EnemyManager : MonoBehaviour
{
    public static Action<int> EnemyCountUpdated;
    [Header("References")]
    public Transform playerTransform;
    public EnemySpawner spawner;
    public ComputeShader enemyComputeShader;

    [Header("Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float obstacleAvoidanceRadius = 1f;
    [SerializeField] private float neighborAvoidanceRadius = 0.5f;
    [SerializeField] private float collisionDistance = 1f;
    [SerializeField] private float checkInterval = 0.1f;
    [SerializeField] private float stucknessThreshold = 2f;

    private List<GameObject> activeEnemies = new List<GameObject>();
    private ComputeBuffer enemyBuffer;
    private ComputeBuffer obstacleBuffer;
    private int kernelIndex;
    private float timeSinceLastCheck;

    private void Start()
    {
        if (enemyComputeShader != null)
        {
            kernelIndex = enemyComputeShader.FindKernel("CSMain");
        }
        InitializeObstacles();
    }

    private void Update()
    {
        if (activeEnemies.Count == 0) return;

        timeSinceLastCheck += Time.deltaTime;

        if (timeSinceLastCheck >= checkInterval)
        {
            CheckEnemyCollisions();
            timeSinceLastCheck = 0f;
        }

        UpdateEnemyPositions();
    }

    private void UpdateEnemyPositions()
    {
        if (enemyBuffer == null || obstacleBuffer == null) return;

        // Update enemy positions in the buffer
        EnemyData[] enemyDataArray = new EnemyData[activeEnemies.Count];
        for (int i = 0; i < activeEnemies.Count; i++)
        {
            Vector3 position = activeEnemies[i].transform.position;
            enemyDataArray[i] = new EnemyData
            {
                position = new Vector2(position.x, position.y),
                velocity = Vector2.zero,
                stuckness = 0 // Initialize stuckness
            };
        }
        enemyBuffer.SetData(enemyDataArray);

        // Set compute shader parameters
        enemyComputeShader.SetBuffer(kernelIndex, "enemyBuffer", enemyBuffer);
        enemyComputeShader.SetBuffer(kernelIndex, "obstacleBuffer", obstacleBuffer);
        enemyComputeShader.SetVector("playerPosition", playerTransform.position);
        enemyComputeShader.SetFloat("deltaTime", Time.deltaTime);
        enemyComputeShader.SetFloat("moveSpeed", moveSpeed);
        enemyComputeShader.SetFloat("obstacleAvoidanceRadius", obstacleAvoidanceRadius);
        enemyComputeShader.SetFloat("neighborAvoidanceRadius", neighborAvoidanceRadius);
        enemyComputeShader.SetInt("enemyCount", activeEnemies.Count);
        enemyComputeShader.SetInt("obstacleCount", obstacleBuffer.count);
        enemyComputeShader.SetFloat("stucknessThreshold", stucknessThreshold);

        // Dispatch the compute shader
        int threadGroups = Mathf.CeilToInt(activeEnemies.Count / 256f);
        enemyComputeShader.Dispatch(kernelIndex, threadGroups, 1, 1);

        // Get results back and update enemy positions
        enemyBuffer.GetData(enemyDataArray);
        for (int i = 0; i < activeEnemies.Count; i++)
        {
            activeEnemies[i].transform.position = new Vector3(enemyDataArray[i].position.x, enemyDataArray[i].position.y, 0);
        }
    }

    private void CheckEnemyCollisions()
    {
        Vector2 playerPosition = playerTransform.position;
        float sqrCollisionDistance = collisionDistance * collisionDistance;

        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            Vector2 enemyPosition = activeEnemies[i].transform.position;
            if ((playerPosition - enemyPosition).sqrMagnitude <= sqrCollisionDistance)
            {
                // Collision detected, return enemy to pool
                GameObject enemy = activeEnemies[i];
                activeEnemies.RemoveAt(i);
                
                if (spawner != null)
                {
                    spawner.ReleaseEnemy(enemy);
                }
                else
                {
                    Debug.LogWarning("EnemySpawner not set. Destroying enemy instead.");
                    Destroy(enemy);
                }
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

        obstacleBuffer = new ComputeBuffer(obstaclePositions.Length, sizeof(float) * 2);
        obstacleBuffer.SetData(obstaclePositions);
    }

    public void AddEnemy(GameObject enemy)
    {
        activeEnemies.Add(enemy);
        UpdateBuffers();
    }

    public void RemoveEnemy(GameObject enemy)
    {
        activeEnemies.Remove(enemy);
        UpdateBuffers();
    }

    private void UpdateBuffers()
    {
        if (enemyBuffer != null)
        {
            enemyBuffer.Release();
        }
        
        if (activeEnemies.Count > 0)
        {
            enemyBuffer = new ComputeBuffer(activeEnemies.Count, sizeof(float) * 5); // 2 for position, 2 for velocity, 1 for stuckness
        }
        else
        {
            enemyBuffer = null;
        }
        EnemyCountUpdated?.Invoke(activeEnemies.Count);
    }

    private void OnDestroy()
    {
        if (enemyBuffer != null)
        {
            enemyBuffer.Release();
        }
        if (obstacleBuffer != null)
        {
            obstacleBuffer.Release();
        }
    }
}