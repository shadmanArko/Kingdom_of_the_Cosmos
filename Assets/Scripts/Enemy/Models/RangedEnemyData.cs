using System;

namespace Enemy.Models
{
    [Serializable]
    public class RangedEnemyData
    {
        public string Id;
        public float MovementSpeed;
        public float Health;
        public float AttackRange;
        public float Damage;
        public float AttackCooldown;
    }
}