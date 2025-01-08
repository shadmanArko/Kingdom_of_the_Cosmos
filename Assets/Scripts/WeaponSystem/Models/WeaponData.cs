using System;

namespace WeaponSystem.Models
{
    [Serializable]
    public class WeaponData
    {
        public string name;
        public int damage;
        public float cooldown;
        public string type; // Controlled or Automatic
        public string triggerCondition; // Only for automatic weapons
        public float knockBackStrength;
        public float knockBackDuration;
    }
}