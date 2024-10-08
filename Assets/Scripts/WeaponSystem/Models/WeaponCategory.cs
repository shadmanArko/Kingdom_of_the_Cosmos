using System;
using System.Collections.Generic;

namespace WeaponSystem.Models
{
    [Serializable]
    public class WeaponCategory
    {
        public string category;
        public List<WeaponData> weapons;

        
    }
}