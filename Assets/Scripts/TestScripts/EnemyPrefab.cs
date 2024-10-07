using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class EnemyPrefab : MonoBehaviour
{
    public AStarComputeManager pathFinder;
    public Transform playerTransform;
    public float speed = 2.0f;

    private List<Vector2Int> path; // Store the path
    private int currentPathIndex;  // Track the current point in the path

    private void Start()
    {
        Vector2 startPosition = transform.position;
        Vector2Int vectorInt = VectorFloatToInt(startPosition);

        // path = pathFinder.FindPath(vectorInt, VectorFloatToInt(playerTransform.position));
        // currentPathIndex = 0;
        //
        // if (path != null && path.Count > 0)
        // {
        //     Debug.Log($"Found path size: {path.Count}, from: ({startPosition.x}, {startPosition.y})");
        //     StartCoroutine(FollowPath()); // Start moving the enemy along the path
        // }
        // else
        // {
        //     Debug.Log($"Path not found from: ({startPosition.x}, {startPosition.y})");
        // }
    }

    // Coroutine to move along the path
    private IEnumerator FollowPath()
    {
        while (currentPathIndex < path.Count)
        {
            Vector2 targetPosition = path[currentPathIndex];
            while ((Vector2)transform.position != targetPosition)
            {
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
                yield return null; // Wait until the next frame
            }
            currentPathIndex++; // Move to the next point in the path
        }
    }

    // Utility to convert Vector2 to Vector2Int
    private static Vector2Int VectorFloatToInt(Vector2 position)
    {
        return new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
    }
}