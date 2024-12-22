using System;

namespace Enemy.Models
{
    [Serializable]
    public class ShamanEnemyData
    {
        public string Id;
        public float MovementSpeed;
        public float Health;
        public float AttackRange;
        public float ShamanRadius;
        public float ShamanInterval;
        public float Damage;
        public float AttackCooldown;
    }
}