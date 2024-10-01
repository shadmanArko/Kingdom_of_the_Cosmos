using System.Collections.Generic;

namespace WeaponSystem.WeaponModels
{
    public class WeaponCategory
    {
        public string Category { get; set; }
        public List<WeaponData> Weapons { get; set; }
    }
}