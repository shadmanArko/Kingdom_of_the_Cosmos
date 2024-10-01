using UnityEngine;
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
            Debug.Log(weaponData.Name + " activated!");
        }

        public virtual void Deactivate()
        {
            Debug.Log(weaponData.Name + " deactivated!");
        }

        public abstract bool CanActivate();
        public void UpgradeWeapon(int newDamage, float newCooldown)
        {
            weaponData.Damage = newDamage;
            weaponData.Cooldown = newCooldown;
            Debug.Log($"{weaponData.Name} upgraded! New Damage: {newDamage}, New Cooldown: {newCooldown}");
        }

        public string GetName()
        {
            return weaponData.Name;
        }
    }
}