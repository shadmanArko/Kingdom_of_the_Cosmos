using UnityEngine;

namespace Enemy.Models
{
    public struct EnemyData
    {
        public Vector2 position;
        public Vector2 velocity;
        public float stuckness;
        public float damage;
        public float distanceToPlayer;
        public float minDistanceToPlayer;
        public int isAlive; // 1 means alive, 0 means dead
    }
}

