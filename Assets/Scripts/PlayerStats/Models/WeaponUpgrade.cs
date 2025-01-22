using System.Collections.Generic;

namespace PlayerStats
{
    public class WeaponUpgrade
    {
        public WeaponType WeaponType { get; set; }
        public List<StatModifier> StatModifiers { get; set; }
        public string SpecialEffect { get; set; }
    }
}