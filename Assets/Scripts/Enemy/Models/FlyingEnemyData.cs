using System;

namespace Enemy.Models
{
    [Serializable]
    public class FlyingEnemyData
    {
        public string Id;
        public float MovementSpeed;
        public float Health;
        public float AttackRange;
        public float Damage;
        public float AttackCooldown;
    }
}