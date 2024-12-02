namespace Pathfinding
{
    [System.Serializable]
    public class AStarNode
    {
        
        //todo Need to connect IsWalkable
        //todo Need to add more values to modify HCost
        public int TileCoordinateX;
        public int TileCoordinateY;
        public AStarNode Parent;
        public float GCost;
        public float HCost;
        public bool IsWalkable;
        public int MovecostFromPreviousTile;
        

        public float FCost { get { return GCost + HCost ; } }

       
        
        public AStarNode(int tileCoordinateX, int tileCoordinateY, AStarNode parent, float gCost, float hCost, bool isWalkable)
        {
            TileCoordinateX = tileCoordinateX;
            TileCoordinateY = tileCoordinateY;
            Parent = parent;
            GCost = gCost;
            HCost = hCost;
            IsWalkable = isWalkable;
            MovecostFromPreviousTile = 0;
           
        }
        
    }
}