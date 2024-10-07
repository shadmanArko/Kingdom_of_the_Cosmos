using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class TestEnemySpawner : MonoBehaviour
{
    public EnemyPrefab EnemyPrefab;
    public AStarComputeManager pathFinder;
    public int numberOfEnemies = 1000;
    public Transform enemiesParent;
    public Transform playerTransform;

    private List<EnemyPrefab> enemyPrefabs = new List<EnemyPrefab>();
    private List<Vector2Int> startPositions = new List<Vector2Int>();
    private List<Vector2Int> targetPositions = new List<Vector2Int>();

    async void Start()
    {
        // Wait for pathfinder to initialize
        await Task.Yield();

        // Debug walkable tiles
        var walkableTiles = pathFinder.GetWalkableTiles();
        Debug.Log($"Total walkable tiles: {walkableTiles.Count}");
        if (walkableTiles.Count == 0)
        {
            Debug.LogError("No walkable tiles found! Check RandomizeWalkableTiles configuration.");
            return;
        }

        // Ensure player position is walkable
        Vector2Int playerPos = VectorFloatToInt(playerTransform.position);
        if (!walkableTiles.Contains(playerPos))
        {
            // Find nearest walkable tile to player
            Vector2Int nearestWalkable = FindNearestWalkableTile(playerPos, walkableTiles);
            Debug.Log($"Player position {playerPos} is not walkable. Using nearest walkable tile {nearestWalkable}");
            playerPos = nearestWalkable;
        }

        for (int i = 0; i < numberOfEnemies; i++)
        {
            // Pick random walkable tile for start position
            if (walkableTiles.Count == 0)
            {
                Debug.LogError("No more walkable tiles available for spawning!");
                break;
            }

            int randomIndex = Random.Range(0, walkableTiles.Count);
            Vector2Int randomTile = walkableTiles[randomIndex];
            
            // Create enemy at the walkable position
            Vector3 position = new Vector3(randomTile.x, randomTile.y, 0);
            var enemyPrefab = Instantiate(EnemyPrefab, position, Quaternion.identity, enemiesParent);
            enemyPrefab.pathFinder = pathFinder;
            enemyPrefab.playerTransform = playerTransform;
            enemyPrefabs.Add(enemyPrefab);

            // Store positions for pathfinding
            startPositions.Add(randomTile);
            targetPositions.Add(playerPos);
        }
        foreach (var startPos in startPositions)
        {
            // Debug.Log($"Start walkable: {pathFinder.GetWalkableTiles().Contains(startPos)}");
            // Debug.Log($"Target walkable: {pathFinder.GetWalkableTiles().Contains(targetPositions[0])}");
            // pathFinder.DebugPath(startPos, targetPositions[0]);
        }
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        // Process paths in batches
        int batchSize = 100; // Smaller batch size for better performance
        List<List<Vector2Int>> allPaths = new List<List<Vector2Int>>();

        for (int i = 0; i < startPositions.Count; i += batchSize)
        {
            int currentBatchSize = Mathf.Min(batchSize, startPositions.Count - i);
            var batchStarts = startPositions.GetRange(i, currentBatchSize);
            var batchTargets = targetPositions.GetRange(i, currentBatchSize);

            // Debug path request
            Debug.Log($"Requesting paths for batch {i/batchSize + 1}:");
            for (int j = 0; j < currentBatchSize; j++)
            {
                Debug.Log($"Path {i+j}: From {batchStarts[j]} to {batchTargets[j]}");
            }

            var paths = pathFinder.FindMultiplePaths(batchStarts, batchTargets);
            if (paths != null)
            {
                allPaths.AddRange(paths);
            }
        }

        stopwatch.Stop();
        Debug.Log($"Found paths {allPaths.Count} in {stopwatch.ElapsedMilliseconds} milliseconds");

        // Debug results
        for (int i = 0; i < allPaths.Count; i++)
        {
            var path = allPaths[i];
            if (path == null || path.Count == 0)
            {
                Debug.Log($"Path Not found for {startPositions[i]} to {targetPositions[i]}");
                Debug.Log($"Start walkable: {walkableTiles.Contains(startPositions[i])}");
                Debug.Log($"Target walkable: {walkableTiles.Contains(targetPositions[i])}");
            }
            else
            {
                Debug.Log($"Found path from {startPositions[i]} to {targetPositions[i]} with {path.Count} steps");
            }
        }
    }

    private Vector2Int FindNearestWalkableTile(Vector2Int position, List<Vector2Int> walkableTiles)
    {
        Vector2Int nearest = walkableTiles[0];
        float minDistance = float.MaxValue;

        foreach (var tile in walkableTiles)
        {
            float distance = Vector2Int.Distance(position, tile);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = tile;
            }
        }

        return nearest;
    }

    private static Vector2Int VectorFloatToInt(Vector3 position)
    {
        return new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
    }
}