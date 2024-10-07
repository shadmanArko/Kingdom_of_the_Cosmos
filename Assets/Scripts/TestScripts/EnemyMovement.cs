using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Burst;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float obstacleAvoidanceRadius = 1f;
    [SerializeField] private float neighborAvoidanceRadius = 0.5f;
    [SerializeField] private float minSpawnDistance = 1f; // Minimum distance between spawned enemies
    
    public Transform playerTransform;
    public GameObject enemyObject;
    public int numberOfEnemies = 100;
    [SerializeField] private float spawnAreaSize = 100f; // Size of the spawn area
    
    private NativeArray<float3> currentPositions;
    private NativeArray<float3> newPositions;
    private NativeArray<float3> obstaclePositions;

    private void Awake()
    {
        SpawnEnemiesWithSafeDistance();
        InitializeArrays();
    }

    private void SpawnEnemiesWithSafeDistance()
    {
        List<Vector2> spawnedPositions = new List<Vector2>();
        
        for (int i = 0; i < numberOfEnemies; i++)
        {
            Vector2 spawnPos = GetSafeSpawnPosition(spawnedPositions);
            GameObject enemy = Instantiate(enemyObject,
                new Vector3(spawnPos.x, spawnPos.y, 0f),
                Quaternion.identity, transform);
            spawnedPositions.Add(spawnPos);
        }
    }

    private Vector2 GetSafeSpawnPosition(List<Vector2> existingPositions)
    {
        const int maxAttempts = 100;
        int attempts = 0;
        
        while (attempts < maxAttempts)
        {
            Vector2 candidatePos = new Vector2(
                UnityEngine.Random.Range(-spawnAreaSize, spawnAreaSize),
                UnityEngine.Random.Range(-spawnAreaSize, spawnAreaSize)
            );
            
            bool isSafe = true;
            foreach (Vector2 existingPos in existingPositions)
            {
                if (Vector2.Distance(candidatePos, existingPos) < minSpawnDistance)
                {
                    isSafe = false;
                    break;
                }
            }
            
            if (isSafe || attempts == maxAttempts - 1)
            {
                return candidatePos;
            }
            
            attempts++;
        }
        
        // If we couldn't find a safe position after max attempts,
        // add some random offset to avoid exact overlap
        Vector2 fallbackPos = existingPositions.Count > 0 ? existingPositions[0] : Vector2.zero;
        return fallbackPos + UnityEngine.Random.insideUnitCircle * minSpawnDistance;
    }

    private void InitializeArrays()
    {
        currentPositions = new NativeArray<float3>(numberOfEnemies, Allocator.Persistent);
        newPositions = new NativeArray<float3>(numberOfEnemies, Allocator.Persistent);
        
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        obstaclePositions = new NativeArray<float3>(obstacles.Length, Allocator.Persistent);
        
        for (int i = 0; i < obstacles.Length; i++)
        {
            obstaclePositions[i] = obstacles[i].transform.position;
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
            
            // Safe normalization of direction to player
            float3 normalizedDirToPlayer = distToPlayer > float.Epsilon 
                ? dirToPlayer / distToPlayer 
                : new float3(0, 1, 0); // Default direction if at same position
            
            // Avoid obstacles
            float3 avoidanceForce = float3.zero;
            for (int i = 0; i < obstaclePositions.Length; i++)
            {
                float3 toObstacle = currentPos - obstaclePositions[i];
                float distance = math.length(toObstacle);
                
                if (distance > float.Epsilon && distance < obstacleRadius)
                {
                    avoidanceForce += toObstacle / (distance * distance); // Squared falloff
                }
            }
            
            // Normalize avoidance force safely
            float avoidanceMagnitude = math.length(avoidanceForce);
            if (avoidanceMagnitude > float.Epsilon)
            {
                avoidanceForce /= avoidanceMagnitude;
            }
            
            // Avoid other enemies
            float3 separationForce = float3.zero;
            int neighborCount = 0;
            
            for (int i = 0; i < currentPositions.Length; i++)
            {
                if (i == index) continue;
                
                float3 toNeighbor = currentPos - currentPositions[i];
                float distance = math.length(toNeighbor);
                
                if (distance > float.Epsilon && distance < neighborRadius)
                {
                    separationForce += toNeighbor / (distance * distance); // Squared falloff
                    neighborCount++;
                }
            }
            
            // Normalize separation force safely
            if (neighborCount > 0)
            {
                float separationMagnitude = math.length(separationForce);
                if (separationMagnitude > float.Epsilon)
                {
                    separationForce /= separationMagnitude;
                }
            }
            
            // Combine forces with weights
            float3 finalDirection = normalizedDirToPlayer + avoidanceForce * 1.5f + separationForce;
            float finalMagnitude = math.length(finalDirection);
            
            // Safe normalization of final direction
            if (finalMagnitude > float.Epsilon)
            {
                finalDirection /= finalMagnitude;
            }
            else
            {
                finalDirection = new float3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0); // Random direction if forces cancel out
            }
            
            // Update position
            newPositions[index] = currentPos + finalDirection * moveSpeed * deltaTime;
        }
    }

    private void Update()
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            currentPositions[i] = transform.GetChild(i).position;
        }

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

        JobHandle handle = job.Schedule(numberOfEnemies, 64);
        handle.Complete();

        for (int i = 0; i < numberOfEnemies; i++)
        {
            transform.GetChild(i).position = newPositions[i];
            currentPositions[i] = newPositions[i];
        }
    }

    private void OnDestroy()
    {
        if (currentPositions.IsCreated) currentPositions.Dispose();
        if (newPositions.IsCreated) newPositions.Dispose();
        if (obstaclePositions.IsCreated) obstaclePositions.Dispose();
    }
}