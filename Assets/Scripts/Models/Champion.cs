using System;

namespace Models
{
    [Serializable]
    public class Champion
    {
        public string name;
        public string tribe;
        public float health;
        public float pickupRange;
        public float baseDamage;
        public float attackSpeed;
        public float criticalDamageMultiplier;
        public float resistance;
        public float dodgeChance;
        public string weaponType;
        public float luckOfGettingDrop;
    }
}
