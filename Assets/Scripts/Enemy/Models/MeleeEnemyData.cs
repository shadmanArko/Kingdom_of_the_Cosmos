using System;

namespace Enemy.Models
{
    [Serializable]
    public class MeleeEnemyData
    {
        public string Id;
        public float MovementSpeed;
        public float Health;
        public float Damage;
        public float AttackCooldown;
    }
}