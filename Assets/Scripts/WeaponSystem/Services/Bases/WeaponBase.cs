using UnityEngine;
using WeaponSystem.Models;
using WeaponSystem.Services.Interfaces;

namespace WeaponSystem.Services.Bases
{
    public abstract class WeaponBase : IWeapon
    {
        
        protected WeaponData weaponData;

        public WeaponBase(WeaponData data)
        {
            weaponData = data;
        }

        public virtual void Activate()
        {
            Debug.Log(weaponData.name + " activated!");
        }

        public virtual void Deactivate()
        {
            Debug.Log(weaponData.name + " deactivated!");
        }

        public virtual void TriggerLightAttack()
        {
            
        }

        public virtual void TriggerHeavyAttack()
        {
            
        }

        public abstract bool CanActivate();
        public abstract bool CanAttack();
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

        public WeaponData GetWeaponData()
        {
            return weaponData;
        }
    }
}