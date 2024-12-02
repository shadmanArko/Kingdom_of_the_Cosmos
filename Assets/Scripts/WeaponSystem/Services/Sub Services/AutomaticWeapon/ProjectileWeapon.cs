using UnityEngine;
using WeaponSystem.Models;
using WeaponSystem.Services.Bases;
using Zenject;

namespace WeaponSystem.Services.Sub_Services.AutomaticWeapon
{
    public class ProjectileWeapon : WeaponBase
    {
        private SignalBus _signalBus;
        public ProjectileWeapon(WeaponData data, SignalBus signalBus) : base(data)
        {
            _signalBus = signalBus;
        }

        public override bool CanActivate()
        {
            return Time.time % weaponData.cooldown == 0;
        }

        public override bool CanAttack()
        {
            //TODO: check for attack eligibility
            return false;
        }
    }
}