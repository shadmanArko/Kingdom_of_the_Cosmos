using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstarTester : MonoBehaviour
{
    public AStarComputeManager pathFinder;

    public Vector2Int startCoordinate;
    public Vector2Int targetCoordinate;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    [ContextMenu("Find Path")]
    public void FindPath()
    {
        var count = 0;
        var path = pathFinder.FindPath(startCoordinate, targetCoordinate);
        foreach (var coordinate in path)
        {
            Debug.Log($"coordinate {count}: x:{coordinate.x}, y:{coordinate.y}");
            count++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
