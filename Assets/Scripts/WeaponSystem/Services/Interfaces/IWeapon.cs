using WeaponSystem.Models;

namespace WeaponSystem.Services.Interfaces
{
    public interface IWeapon
    {
        void Activate();
        void Deactivate();
        bool CanActivate();
        bool CanAttack();
        void TriggerLightAttack();
        void TriggerHeavyAttack();
        void UpgradeWeapon(int newDamage, float newCooldown);
        string GetName();
        WeaponData GetWeaponData();
    }
}