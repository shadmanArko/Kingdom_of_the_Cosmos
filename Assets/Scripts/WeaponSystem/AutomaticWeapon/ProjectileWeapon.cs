using UnityEngine;
using WeaponSystem.WeaponModels;

namespace WeaponSystem.AutomaticWeapon
{
    public class ProjectileWeapon : WeaponBase
    {
        public ProjectileWeapon(WeaponData data) : base(data) { }

        public override bool CanActivate()
        {
            return Time.time % weaponData.Cooldown == 0;
        }
    }
}