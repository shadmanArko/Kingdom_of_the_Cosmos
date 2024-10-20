using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Burst;
using Unity.VisualScripting;

public class EnemyManager : MonoBehaviour
{
    public static Action<int> EnemyCountUpdated;
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float obstacleAvoidanceRadius = 1f;
    [SerializeField] private float neighborAvoidanceRadius = 0.5f;
    [SerializeField] private bool useComputeShader = false;
    
    [Header("References")]
    public Transform playerTransform;
    public ComputeShader enemyComputeShader;
    
    private List<GameObject> activeEnemies = new List<GameObject>();
    private List<Vector2> obstaclePositions = new List<Vector2>();
    private NativeArray<float3> currentPositions;
    private NativeArray<float3> newPositions;
    private NativeArray<float2> obstaclePositionsArray;

    // Compute Shader variables
    private ComputeBuffer enemyBuffer;
    private ComputeBuffer obstacleBuffer;
    private int kernelIndex;

    private struct EnemyData
    {
        public Vector2 position;
        public Vector2 velocity;
    }

    private void Start()
    {
        if (enemyComputeShader != null)
        {
            kernelIndex = enemyComputeShader.FindKernel("CSMain");
        }
        InitializeObstacles();
    }

    private void InitializeObstacles()
    {
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        foreach (GameObject obstacle in obstacles)
        {
            obstaclePositions.Add(obstacle.transform.position);
        }
        obstaclePositionsArray = new NativeArray<float2>(obstaclePositions.Count, Allocator.Persistent);
        for (int i = 0; i < obstaclePositions.Count; i++)
        {
            obstaclePositionsArray[i] = obstaclePositions[i];
        }
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
        int enemyCount = activeEnemies.Count;
        EnemyCountUpdated?.Invoke(enemyCount);
        // Update Job System buffers
        if (currentPositions.IsCreated) currentPositions.Dispose();
        if (newPositions.IsCreated) newPositions.Dispose();
        currentPositions = new NativeArray<float3>(enemyCount, Allocator.Persistent);
        newPositions = new NativeArray<float3>(enemyCount, Allocator.Persistent);

        if (enemyCount > 0)
        {
            // Update Compute Shader buffers
            if (enemyBuffer != null) enemyBuffer.Release();
            enemyBuffer = new ComputeBuffer(enemyCount, sizeof(float) * 4); // 2 floats for position, 2 for velocity
        }
        else
        {
            // If there are no enemies, make sure to release any existing buffer
            if (enemyBuffer != null)
            {
                enemyBuffer.Release();
                enemyBuffer = null;
            }
        }
       

        if (obstacleBuffer != null) obstacleBuffer.Release();
        obstacleBuffer = new ComputeBuffer(obstaclePositions.Count, sizeof(float) * 2);
        obstacleBuffer.SetData(obstaclePositions);
    }

    private void Update()
    {
        if (activeEnemies.Count == 0) return;

        if (useComputeShader)
        {
            UpdateEnemyPositionsComputeShader();
        }
        else
        {
            UpdateEnemyPositionsJobSystem();
        }
    }

    private void UpdateEnemyPositionsJobSystem()
    {
        int enemyCount = activeEnemies.Count;

        // Update current positions
        for (int i = 0; i < enemyCount; i++)
        {
            currentPositions[i] = activeEnemies[i].transform.position;
        }

        // Create and schedule the job
        var job = new EnemyMovementJob
        {
            playerPosition = playerTransform.position,
            deltaTime = Time.deltaTime,
            moveSpeed = moveSpeed,
            obstacleRadius = obstacleAvoidanceRadius,
            neighborRadius = neighborAvoidanceRadius,
            obstaclePositions = obstaclePositionsArray,
            currentPositions = currentPositions,
            newPositions = newPositions
        };

        JobHandle handle = job.Schedule(enemyCount, 64);
        handle.Complete();

        // Update enemy positions
        for (int i = 0; i < enemyCount; i++)
        {
            activeEnemies[i].transform.position = newPositions[i];
        }
    }

    private void UpdateEnemyPositionsComputeShader()
    {
        int enemyCount = activeEnemies.Count;
        if (enemyCount == 0 || enemyBuffer == null) return;
        // Prepare data for the compute shader
        EnemyData[] enemyDataArray = new EnemyData[enemyCount];
        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 position = activeEnemies[i].transform.position;
            enemyDataArray[i] = new EnemyData
            {
                position = new Vector2(position.x, position.y),
                velocity = Vector2.zero
            };
        }

        // Set buffer and variables
        enemyBuffer.SetData(enemyDataArray);
        enemyComputeShader.SetBuffer(kernelIndex, "enemyBuffer", enemyBuffer);
        enemyComputeShader.SetBuffer(kernelIndex, "obstacleBuffer", obstacleBuffer);
        enemyComputeShader.SetVector("playerPosition", playerTransform.position);
        enemyComputeShader.SetFloat("deltaTime", Time.deltaTime);
        enemyComputeShader.SetFloat("moveSpeed", moveSpeed);
        enemyComputeShader.SetFloat("obstacleAvoidanceRadius", obstacleAvoidanceRadius);
        enemyComputeShader.SetFloat("neighborAvoidanceRadius", neighborAvoidanceRadius);
        enemyComputeShader.SetInt("enemyCount", enemyCount);
        enemyComputeShader.SetInt("obstacleCount", obstaclePositions.Count);

        // Dispatch the compute shader
        int threadGroups = Mathf.CeilToInt(enemyCount / 256f);
        enemyComputeShader.Dispatch(kernelIndex, threadGroups, 1, 1);

        // Get results back
        enemyBuffer.GetData(enemyDataArray);

        // Update enemy positions
        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 newPosition = new Vector3(enemyDataArray[i].position.x, enemyDataArray[i].position.y, 0);
            activeEnemies[i].transform.position = newPosition;
        }
    }

    [BurstCompile]
    private struct EnemyMovementJob : IJobParallelFor
    {
        [ReadOnly] public float3 playerPosition;
        [ReadOnly] public float deltaTime;
        [ReadOnly] public float moveSpeed;
        [ReadOnly] public float obstacleRadius;
        [ReadOnly] public float neighborRadius;
        [ReadOnly] public NativeArray<float2> obstaclePositions;
        [ReadOnly] public NativeArray<float3> currentPositions;
        [WriteOnly] public NativeArray<float3> newPositions;

        public void Execute(int index)
        {
            float3 currentPos = currentPositions[index];
            float3 dirToPlayer = playerPosition - currentPos;
            float distToPlayer = math.length(dirToPlayer);
            
            float3 normalizedDirToPlayer = distToPlayer > float.Epsilon 
                ? dirToPlayer / distToPlayer 
                : new float3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0);
            
            float3 avoidanceForce = float3.zero;
            for (int i = 0; i < obstaclePositions.Length; i++)
            {
                float3 toObstacle = currentPos - new float3(obstaclePositions[i].x, obstaclePositions[i].y, 0);
                float distance = math.length(toObstacle);
                
                if (distance > float.Epsilon && distance < obstacleRadius)
                {
                    avoidanceForce += toObstacle / (distance * distance);
                }
            }
            
            float3 separationForce = float3.zero;
            int neighborCount = 0;
            
            for (int i = 0; i < currentPositions.Length; i++)
            {
                if (i == index) continue;
                
                float3 toNeighbor = currentPos - currentPositions[i];
                float distance = math.length(toNeighbor);
                
                if (distance > float.Epsilon && distance < neighborRadius)
                {
                    separationForce += toNeighbor / (distance * distance);
                    neighborCount++;
                }
            }
            
            if (neighborCount > 0)
            {
                separationForce /= neighborCount;
            }
            
            float3 finalDirection = normalizedDirToPlayer + avoidanceForce * 1.5f + separationForce;
            float finalMagnitude = math.length(finalDirection);
            
            if (finalMagnitude > float.Epsilon)
            {
                finalDirection /= finalMagnitude;
            }
            else
            {
                finalDirection = new float3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0);
            }
            
            float speed = math.max(moveSpeed * 0.5f, moveSpeed * finalMagnitude);
            newPositions[index] = currentPos + finalDirection * speed * deltaTime;
        }
    }

    private void OnDestroy()
    {
        if (currentPositions.IsCreated) currentPositions.Dispose();
        if (newPositions.IsCreated) newPositions.Dispose();
        if (obstaclePositionsArray.IsCreated) obstaclePositionsArray.Dispose();
        if (enemyBuffer != null) enemyBuffer.Release();
        if (obstacleBuffer != null) obstacleBuffer.Release();
    }
}