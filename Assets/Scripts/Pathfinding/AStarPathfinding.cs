using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public class AStarPathfinding
    {

        //todo Connect with main program
        // public int GridSizeX = 10; // Adjust this based on your grid size
        // public int GridSizeY = 10; // Adjust this based on your grid size

        //private Node[,] _grid;
        private List<AStarNode> _openSet;
        private HashSet<AStarNode> _closedSet;

        private readonly int _gridSizeX;
        private readonly int _gridSizeY;

        private readonly bool _canMoveDiagonally;
        //private readonly Node[,] _grid;

        public AStarPathfinding(bool canMoveDiagonally/*, Node[,] grid*/)
        {
            // _gridSizeX = gridSizeX;
            // _gridSizeY = gridSizeY;
            _canMoveDiagonally = canMoveDiagonally;
            //_grid = grid;
        }


        public List<Vector2Int> FindPath(Vector2Int startTileCoordinates, Vector2Int goalTileCoordinates, List<AStarNode> grid)
        {
            startTileCoordinates *= -1;
            goalTileCoordinates *= -1;

            _openSet = new List<AStarNode>();
            _closedSet = new HashSet<AStarNode>();

            
            AStarNode startNode = GetNode(grid, startTileCoordinates.x, startTileCoordinates.y);
            AStarNode goalNode = GetNode(grid, goalTileCoordinates.x, goalTileCoordinates.y);

            startNode.GCost = 0; // Set GCost of the start node to zero
            startNode.HCost = GetDistance(startNode, goalNode); // Set HCost using the heuristic

            _openSet.Add(startNode);

            while (_openSet.Count > 0)
            {
                AStarNode currentNode = GetLowestFCostNode(_openSet);

                if (currentNode == goalNode)
                {
                    // Path found
                    return RetracePath(startNode, goalNode);
                }

                _openSet.Remove(currentNode);
                _closedSet.Add(currentNode);

                foreach (AStarNode neighbor in GetNeighbors(currentNode, grid))
                {
                    if (!neighbor.IsWalkable || _closedSet.Contains(neighbor))
                        continue;

                    float newGCost = currentNode.GCost + GetDistance(currentNode, neighbor);

                    if (newGCost < neighbor.GCost || !_openSet.Contains(neighbor))
                    {
                        neighbor.GCost = newGCost;
                        neighbor.HCost = GetDistance(neighbor, goalNode);
                        neighbor.Parent = currentNode;

                        if (!_openSet.Contains(neighbor))
                            _openSet.Add(neighbor);
                    }
                }
            }

            // No path found
            // Debug.Log("No path found.");
            return null;
        }
        private AStarNode GetLowestFCostNode(List<AStarNode> nodeList)
        {
            AStarNode lowestFCostAStarNode = nodeList[0];

            for (int i = 1; i < nodeList.Count; i++)
            {
                if (nodeList[i].FCost < lowestFCostAStarNode.FCost)
                    lowestFCostAStarNode = nodeList[i];
            }

            return lowestFCostAStarNode;
        }

        private List<AStarNode> GetNeighbors(AStarNode aStarNode, List<AStarNode> grid)
        {
            List<AStarNode> neighbors = new List<AStarNode>();

            int[] neighborOffsets = { -1, 0, 1 };

            foreach (int xOffset in neighborOffsets)
            {
                foreach (int yOffset in neighborOffsets)
                {
                    // include diagonal neighbors
                    if (!_canMoveDiagonally && !(xOffset == 0 || yOffset == 0))
                    {
                        continue;
                    }
                    // Skip the center (current) node 
                    if (xOffset != 0 || yOffset != 0)
                    {
                        int neighborX = aStarNode.TileCoordinateX + xOffset;
                        int neighborY = aStarNode.TileCoordinateY + yOffset;
                        
                        foreach (var starNode in grid)
                        {
                            if (starNode.TileCoordinateX == neighborX && starNode.TileCoordinateY == neighborY)
                            {
                                int movementCost = xOffset != 0 && yOffset != 0 ? 14 : 10; // 14 for diagonals, 10 for straight
                                starNode.GCost = aStarNode.GCost + movementCost;
                                neighbors.Add(starNode);
                            }
                        }
                    }
                }
            }

            return neighbors;
        }

        AStarNode GetNode(List<AStarNode> grid, int xPos, int yPos)
        {
            foreach (var starNode in grid)
            {
                if (starNode.TileCoordinateX == xPos && starNode.TileCoordinateY == yPos)
                {
                    return starNode;
                }
            }

            Debug.LogError($"Fatal Error: Node not found for {xPos},{yPos}");

            foreach (var node in grid)
                Debug.Log($"node Pos: {node.TileCoordinateX},{node.TileCoordinateY}");

            return null;
        }

        private List<Vector2Int> RetracePath(AStarNode startAStarNode, AStarNode endAStarNode)
        {
            List<Vector2Int> path = new List<Vector2Int>();
            AStarNode currentAStarNode = endAStarNode;

            while (currentAStarNode != startAStarNode)
            {
                path.Add(new Vector2Int(currentAStarNode.TileCoordinateX * -1, currentAStarNode.TileCoordinateY * -1));// todo If it makes problem then create new TileCoord and assign it.
                currentAStarNode = currentAStarNode.Parent;
            }

            path.Reverse();
            return path;
        }

        private int GetDistance(AStarNode aStarNodeA, AStarNode aStarNodeB)
        {
            int distanceX = Math.Abs(aStarNodeA.TileCoordinateX - aStarNodeB.TileCoordinateX);
            int distanceY = Math.Abs(aStarNodeA.TileCoordinateY - aStarNodeB.TileCoordinateY);

            // Adjust the cost of diagonal movement as desired (e.g., multiply by 1.4 for 45-degree movement)
            int diagonalCost = 14; // Cost of diagonal movement (approximately 1.4 times straight cost)
            int straightCost = 10; // Cost of straight movement

            int diagonalSteps = Math.Min(distanceX, distanceY);
            int straightSteps = Math.Abs(distanceX - distanceY);

            if (!_canMoveDiagonally && diagonalSteps != 0)
            {
                return int.MaxValue;
            }
            else
            {
                return ((diagonalCost * diagonalSteps) + (straightCost * straightSteps));
            }


        }


    }
}