using UnityEngine;
using WeaponSystem.Models;

namespace WeaponSystem.AutomaticWeapon
{
    public class ProjectileWeapon : WeaponBase
    {
        public ProjectileWeapon(WeaponData data) : base(data) { }

        public override bool CanActivate()
        {
            return Time.time % weaponData.cooldown == 0;
        }
    }
}