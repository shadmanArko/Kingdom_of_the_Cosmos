using WeaponSystem.Models;

namespace WeaponSystem.Services.Interfaces
{
    public interface IWeapon
    {
        void Activate();
        void Deactivate();
        bool CanActivate();
        bool CanAttack();
        void TriggerAttack();
        void UpgradeWeapon(int newDamage, float newCooldown);
        string GetName();
        WeaponData GetWeaponData();
    }
}