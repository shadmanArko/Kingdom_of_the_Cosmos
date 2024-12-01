using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public class AStarComputeManager : MonoBehaviour
    {
        public ComputeShader computeShader;
        public int gridSizeX = 100;
        public int gridSizeY = 100;
        public bool canMoveDiagonally = true;

        private int initializeKernel;
        private int findPathKernel;
        private int updateWalkabilityKernel;
        private ComputeBuffer nodesBuffer;
        private ComputeBuffer pathRequestsBuffer;
        private ComputeBuffer pathResultsBuffer;
        private ComputeBuffer walkabilityUpdatesBuffer;

        private const int MAX_PATH_REQUESTS = 1000;
        private const int MAX_WALKABILITY_UPDATES = 1000;

        private List<Vector2Int> walkableTiles;

        void Start()
        {
            InitializeComputeShader();
            walkableTiles = new List<Vector2Int>();
            RandomizeWalkableTiles(1f); // Make 70% of tiles walkable
        }

        void InitializeComputeShader()
        {
            // Find kernels
            initializeKernel = computeShader.FindKernel("InitializeNodes");
            findPathKernel = computeShader.FindKernel("FindPath");
            updateWalkabilityKernel = computeShader.FindKernel("UpdateNodeWalkability");

            // Calculate stride for Node struct
            int nodeStride = sizeof(float) * 3 + // gCost, hCost, fCost
                             sizeof(int) * 3 +    // position.x, position.y, parentIndex
                             sizeof(int);         // isWalkable, isOpen, isClosed packed into one int

            // Create buffers
            nodesBuffer = new ComputeBuffer(gridSizeX * gridSizeY, nodeStride);
            pathRequestsBuffer = new ComputeBuffer(MAX_PATH_REQUESTS * 2, sizeof(int) * 2);
            pathResultsBuffer = new ComputeBuffer(MAX_PATH_REQUESTS * gridSizeX * gridSizeY, sizeof(int));
            walkabilityUpdatesBuffer = new ComputeBuffer(MAX_WALKABILITY_UPDATES, sizeof(int) * 2);

            // Set buffers for all kernels
            computeShader.SetBuffer(initializeKernel, "nodes", nodesBuffer);
            computeShader.SetBuffer(findPathKernel, "nodes", nodesBuffer);
            computeShader.SetBuffer(findPathKernel, "pathRequests", pathRequestsBuffer);
            computeShader.SetBuffer(findPathKernel, "pathResults", pathResultsBuffer);
            computeShader.SetBuffer(updateWalkabilityKernel, "nodes", nodesBuffer);
            computeShader.SetBuffer(updateWalkabilityKernel, "walkabilityUpdates", walkabilityUpdatesBuffer);

            // Set shared variables
            computeShader.SetInts("gridSize", new int[] { gridSizeX, gridSizeY });
            computeShader.SetBool("canMoveDiagonally", canMoveDiagonally);

            // Initialize nodes
            computeShader.Dispatch(initializeKernel, Mathf.CeilToInt(gridSizeX * gridSizeY / 64f), 1, 1);
        }

        public void DebugPath(Vector2Int start, Vector2Int goal)
        {
            Debug.Log($"Attempting to find path from {start} to {goal}");
            Debug.Log($"Start walkable: {walkableTiles.Contains(start)}");
            Debug.Log($"Goal walkable: {walkableTiles.Contains(goal)}");
        
            var path = FindPath(start, goal);
            if (path != null)
            {
                Debug.Log($"Path found with {path.Count} steps:");
                foreach (var point in path)
                {
                    Debug.Log($"-> {point}");
                }
            }
            else
            {
                Debug.Log("No path found");
            }
        }

        public List<Vector2Int> FindPath(Vector2Int start, Vector2Int goal)
        {
            InitializeComputeShader();
            Vector2Int[] requests = new Vector2Int[] { start, goal };
            pathRequestsBuffer.SetData(requests);
            if (!walkableTiles.Contains(start))
            {
                Debug.Log($"Is Not walkable tile {start.x}, {start.y}");
                return null;
            }
            computeShader.SetInt("numPathRequests", 1);
            computeShader.Dispatch(findPathKernel, 1, 1, 1);

            int[] results = new int[gridSizeX * gridSizeY];
            pathResultsBuffer.GetData(results);

            int pathLength = results[0];
            if (pathLength == 0) {
                Debug.Log("No Path Found");
                return null; // No path found
            }

            List<Vector2Int> path = new List<Vector2Int>();
            for (int i = pathLength - 1; i >= 1; i--)
            {
                int index = results[i];
                path.Add(new Vector2Int(index % gridSizeX, index / gridSizeX));
            }

            return path;
        }

        public List<List<Vector2Int>> FindMultiplePaths(List<Vector2Int> starts, List<Vector2Int> goals)
        {
            if (starts.Count != goals.Count || starts.Count > MAX_PATH_REQUESTS)
            {
                Debug.LogError("Invalid path requests");
                return null;
            }

            Vector2Int[] requests = new Vector2Int[starts.Count * 2];
            for (int i = 0; i < starts.Count; i++)
            {
                requests[i * 2] = starts[i];
                requests[i * 2 + 1] = goals[i];
            }
            pathRequestsBuffer.SetData(requests);

            computeShader.SetInt("numPathRequests", starts.Count);
            computeShader.Dispatch(findPathKernel, Mathf.CeilToInt(starts.Count / 64f), 1, 1);

            int[] results = new int[starts.Count * gridSizeX * gridSizeY];
            pathResultsBuffer.GetData(results);

            List<List<Vector2Int>> paths = new List<List<Vector2Int>>();
            for (int requestIndex = 0; requestIndex < starts.Count; requestIndex++)
            {
                int pathLength = results[requestIndex];
                if (pathLength == 0)
                {
                    Debug.Log($"No path found for {starts[requestIndex]}");
                    paths.Add(null); // No path found for this request
                    continue;
                }

                List<Vector2Int> path = new List<Vector2Int>();
                int resultStartIndex = requestIndex * gridSizeX * gridSizeY;
                for (int i = pathLength - 1; i >= 1; i--)
                {
                    int index = results[resultStartIndex + i];
                    path.Add(new Vector2Int(index % gridSizeX, index / gridSizeX));
                }
                paths.Add(path);
            }

            return paths;
        }

        public void SetNodeWalkable(int x, int y, bool isWalkable)
        {
            Vector2Int[] updates = new Vector2Int[] { new Vector2Int(x, y) };
            walkabilityUpdatesBuffer.SetData(updates);

            computeShader.SetInt("numWalkabilityUpdates", 1);
            computeShader.Dispatch(updateWalkabilityKernel, 1, 1, 1);

            UpdateWalkableTilesList(x, y, isWalkable);
        }

        private void UpdateWalkableTilesList(int x, int y, bool isWalkable)
        {
            Vector2Int tile = new Vector2Int(x, y);
            if (isWalkable && !walkableTiles.Contains(tile))
            {
                walkableTiles.Add(tile);
            }
            else if (!isWalkable && walkableTiles.Contains(tile))
            {
                walkableTiles.Remove(tile);
            }
        }

        public void RandomizeWalkableTiles(float walkablePercentage)
        {
            int totalTiles = gridSizeX * gridSizeY;
            int walkableCount = Mathf.RoundToInt(totalTiles * walkablePercentage);

            List<Vector2Int> allTiles = new List<Vector2Int>();
            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    allTiles.Add(new Vector2Int(x, y));
                }
            }

            walkableTiles.Clear();
            Vector2Int[] updates = new Vector2Int[totalTiles];
            int updateCount = 0;

            while (walkableTiles.Count < walkableCount && allTiles.Count > 0)
            {
                int index = Random.Range(0, allTiles.Count);
                Vector2Int tile = allTiles[index];
                allTiles.RemoveAt(index);

                updates[updateCount] = tile;
                updateCount++;

                if (updateCount == MAX_WALKABILITY_UPDATES || allTiles.Count == 0)
                {
                    walkabilityUpdatesBuffer.SetData(updates, 0, 0, updateCount);
                    computeShader.SetInt("numWalkabilityUpdates", updateCount);
                    computeShader.Dispatch(updateWalkabilityKernel, Mathf.CeilToInt(updateCount / 64f), 1, 1);

                    for (int i = 0; i < updateCount; i++)
                    {
                        walkableTiles.Add(updates[i]);
                    }

                    updateCount = 0;
                }
            }

            Debug.Log($"Walkable tiles: {walkableTiles.Count} / {totalTiles}");
        }

        void OnDestroy()
        {
            nodesBuffer?.Release();
            pathRequestsBuffer?.Release();
            pathResultsBuffer?.Release();
            walkabilityUpdatesBuffer?.Release();
        }

        // Optional: Method to get all walkable tiles
        public List<Vector2Int> GetWalkableTiles()
        {
            return new List<Vector2Int>(walkableTiles);
        }
    }
}