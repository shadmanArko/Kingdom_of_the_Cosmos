﻿using UnityEngine;
using WeaponSystem.WeaponModels;

namespace WeaponSystem
{
    public abstract class WeaponBase : IWeapon
    {
        
        protected WeaponData weaponData;

        public WeaponBase(WeaponData data)
        {
            this.weaponData = data;
        }

        public virtual void Activate()
        {
            Debug.Log(weaponData.name + " activated!");
        }

        public virtual void Deactivate()
        {
            Debug.Log(weaponData.name + " deactivated!");
        }

        public abstract bool CanActivate();
        public void UpgradeWeapon(int newDamage, float newCooldown)
        {
            weaponData.damage = newDamage;
            weaponData.cooldown = newCooldown;
            Debug.Log($"{weaponData.name} upgraded! New damage: {newDamage}, New cooldown: {newCooldown}");
        }

        public string GetName()
        {
            return weaponData.name;
        }
    }
}