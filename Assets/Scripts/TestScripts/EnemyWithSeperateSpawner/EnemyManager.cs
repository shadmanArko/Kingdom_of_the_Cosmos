using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Burst;

public class EnemyManager : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float obstacleAvoidanceRadius = 1f;
    [SerializeField] private float neighborAvoidanceRadius = 0.5f;
    
    [Header("References")]
    public Transform playerTransform;
    
    private List<GameObject> activeEnemies = new List<GameObject>();
    private NativeArray<float3> currentPositions;
    private NativeArray<float3> newPositions;
    private NativeArray<float3> obstaclePositions;

    private void Start()
    {
        InitializeObstacles();
    }

    private void InitializeObstacles()
    {
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        obstaclePositions = new NativeArray<float3>(obstacles.Length, Allocator.Persistent);
        
        for (int i = 0; i < obstacles.Length; i++)
        {
            obstaclePositions[i] = obstacles[i].transform.position;
        }
    }

    public void AddEnemy(GameObject enemy)
    {
        activeEnemies.Add(enemy);
    }

    public void RemoveEnemy(GameObject enemy)
    {
        activeEnemies.Remove(enemy);
    }

    private void Update()
    {
        if (activeEnemies.Count == 0) return;

        UpdateEnemyPositions();
    }

    private void UpdateEnemyPositions()
    {
        int enemyCount = activeEnemies.Count;

        // Resize arrays if necessary
        if (currentPositions.Length != enemyCount)
        {
            if (currentPositions.IsCreated) currentPositions.Dispose();
            if (newPositions.IsCreated) newPositions.Dispose();

            currentPositions = new NativeArray<float3>(enemyCount, Allocator.Persistent);
            newPositions = new NativeArray<float3>(enemyCount, Allocator.Persistent);
        }

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
            obstaclePositions = obstaclePositions,
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

    [BurstCompile]
    private struct EnemyMovementJob : IJobParallelFor
    {
        [ReadOnly] public float3 playerPosition;
        [ReadOnly] public float deltaTime;
        [ReadOnly] public float moveSpeed;
        [ReadOnly] public float obstacleRadius;
        [ReadOnly] public float neighborRadius;
        [ReadOnly] public NativeArray<float3> obstaclePositions;
        [ReadOnly] public NativeArray<float3> currentPositions;
        [WriteOnly] public NativeArray<float3> newPositions;

        public void Execute(int index)
        {
            float3 currentPos = currentPositions[index];
            float3 dirToPlayer = playerPosition - currentPos;
            float distToPlayer = math.length(dirToPlayer);
            
            float3 normalizedDirToPlayer = distToPlayer > float.Epsilon 
                ? dirToPlayer / distToPlayer 
                : new float3(0, 1, 0);
            
            float3 avoidanceForce = float3.zero;
            for (int i = 0; i < obstaclePositions.Length; i++)
            {
                float3 toObstacle = currentPos - obstaclePositions[i];
                float distance = math.length(toObstacle);
                
                if (distance > float.Epsilon && distance < obstacleRadius)
                {
                    avoidanceForce += toObstacle / (distance * distance);
                }
            }
            
            float avoidanceMagnitude = math.length(avoidanceForce);
            if (avoidanceMagnitude > float.Epsilon)
            {
                avoidanceForce /= avoidanceMagnitude;
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
                float separationMagnitude = math.length(separationForce);
                if (separationMagnitude > float.Epsilon)
                {
                    separationForce /= separationMagnitude;
                }
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
            
            newPositions[index] = currentPos + finalDirection * moveSpeed * deltaTime;
        }
    }

    private void OnDestroy()
    {
        if (currentPositions.IsCreated) currentPositions.Dispose();
        if (newPositions.IsCreated) newPositions.Dispose();
        if (obstaclePositions.IsCreated) obstaclePositions.Dispose();
    }
}