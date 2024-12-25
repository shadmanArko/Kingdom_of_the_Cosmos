using System;

namespace PlayerSystem.Models
{
    [Serializable]
    public class Player
    {
        public float health;
        public float maxHealth;
        public float expGain;
        public float shield;
        public float maxShield;
        public float shieldRegenerationCooldown;
        public float movementSpeed;
        public float heavyAttackChargeTime;
        public float dodgeDistance;
        public int dodgeCount;
        public float criticalChance;
        public float criticalDamage;
        public float attackArea;
        public float lifeRegeneration;
        public float pullRadius;
        public float damage;
        public float defense;
        public float damageReduction;
        public float elementalDamage;
        public float affinity;
        public float knockBack;
        public float projectileSpeed;
        public float projectilePierce;
        public float projectileRange;
        public int multiProjectile;
    }
}