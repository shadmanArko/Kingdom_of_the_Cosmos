namespace Player.Models
{
    public class Player
    {
        public int Health { get; set; }
        public float ExpGain { get; set; }
        
        public int Shield { get; set; }
        public float ShieldRegenerationCooldown { get; set; }
        
        public float MovementSpeed { get; set; }
        public float HeavyAttackChargeTime { get; set; }
        
        public float DodgeDistance { get; set; }
        public int DodgeCount { get; set; }
        
        public float CriticalChance { get; set; }
        public float CriticalDamage { get; set; }
        
        public float AttackArea { get; set; }
        public float LifeRegeneration { get; set; }
        public float PullRadius { get; set; }
        
        public float Damage { get; set; }
        public float Defense { get; set; }
        public float DamageReduction { get; set; }
        public float ElementalDamage { get; set; }
        public float Affinity { get; set; }
        public float KnockBack { get; set; }
        
        public float ProjectileSpeed { get; set; }
        public float ProjectilePierce { get; set; }
        public float ProjectileRange { get; set; }
        public int MultiProjectile { get; set; }
    }
}