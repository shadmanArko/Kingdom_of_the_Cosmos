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
    
    public Transform playerTransform;
    public GameObject enemyObject;
    public int numberOfEnemies = 100;
    
    private NativeArray<float3> currentPositions;
    private NativeArray<float3> newPositions;
    private NativeArray<float3> obstaclePositions;
    
    private void Awake()
    {
        SpawnEnemies();
        InitializeArrays();
    }

    private void SpawnEnemies()
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            Instantiate(enemyObject,
                new Vector3(UnityEngine.Random.Range(-100, 100), UnityEngine.Random.Range(-100, 100), 0f),
                Quaternion.identity, transform);
        }
    }

    private void InitializeArrays()
    {
        // Initialize both position arrays with the actual number of enemies
        currentPositions = new NativeArray<float3>(numberOfEnemies, Allocator.Persistent);
        newPositions = new NativeArray<float3>(numberOfEnemies, Allocator.Persistent);
        
        // Cache obstacle positions
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        obstaclePositions = new NativeArray<float3>(obstacles.Length, Allocator.Persistent);
        
        for (int i = 0; i < obstacles.Length; i++)
        {
            obstaclePositions[i] = obstacles[i].transform.position;
        }
    }

    private void OnDestroy()
    {
        if (currentPositions.IsCreated) currentPositions.Dispose();
        if (newPositions.IsCreated) newPositions.Dispose();
        if (obstaclePositions.IsCreated) obstaclePositions.Dispose();
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
            float3 dirToPlayer = math.normalize(playerPosition - currentPos);
            
            // Avoid obstacles
            float3 avoidanceForce = float3.zero;
            for (int i = 0; i < obstaclePositions.Length; i++)
            {
                float3 toObstacle = currentPos - obstaclePositions[i];
                float distance = math.length(toObstacle);
                
                if (distance < obstacleRadius)
                {
                    avoidanceForce += math.normalize(toObstacle) / distance;
                }
            }
            
            // Avoid other enemies (simple flocking behavior)
            float3 separationForce = float3.zero;
            int neighborCount = 0;
            
            for (int i = 0; i < currentPositions.Length; i++)
            {
                if (i == index) continue;
                
                float3 toNeighbor = currentPos - currentPositions[i];
                float distance = math.length(toNeighbor);
                
                if (distance < neighborRadius)
                {
                    separationForce += math.normalize(toNeighbor) / distance;
                    neighborCount++;
                }
            }
            
            if (neighborCount > 0)
            {
                separationForce /= neighborCount;
            }
            
            // Combine forces
            float3 finalDirection = math.normalize(dirToPlayer + avoidanceForce + separationForce);
            
            // Update position
            newPositions[index] = currentPos + finalDirection * moveSpeed * deltaTime;
        }
    }

    private void Update()
    {
        // Update current positions array from transforms
        for (int i = 0; i < numberOfEnemies; i++)
        {
            currentPositions[i] = transform.GetChild(i).position;
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

        JobHandle handle = job.Schedule(numberOfEnemies, 64);
        handle.Complete();

        // Update actual GameObjects using the new positions
        for (int i = 0; i < numberOfEnemies; i++)
        {
            transform.GetChild(i).position = newPositions[i];
            currentPositions[i] = newPositions[i]; // Update current positions for next frame
        }
    }
}