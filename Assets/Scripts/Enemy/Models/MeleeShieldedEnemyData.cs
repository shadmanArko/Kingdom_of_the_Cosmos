using System;

namespace Enemy.Models
{
    [Serializable]
    public class MeleeShieldedEnemyData
    {
        public string Id;
        public float ShieldHealth;
        public float AttackRange;
        public float MinimumDistanceToPlayer;
        public float MovementSpeed;
        public float Health;
        public float Damage;
        public float AttackCooldown;
    }
}
