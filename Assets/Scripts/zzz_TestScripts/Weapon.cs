using System;

namespace Models
{
    [Serializable]
    public class Weapon
    {
        
        
        public float fireRate;
        public float range;
        public float projectileSpeed;
        public int ammoCapacity;
        public float reloadTime;
        public float spread;
        public int projectileCount;
        public int piercing;
        public float knockBack;
        public float criticalChance;
        public float criticalDamageMultiplier;
        public string damageType;
        public float recoil;
        public float accuracy;
        public float aoeRadius;
        public bool isHoming;
        public float durability;
        public string weaponType;

    }
}