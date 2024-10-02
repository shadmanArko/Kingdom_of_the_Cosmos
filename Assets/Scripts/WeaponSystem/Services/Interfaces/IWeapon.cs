using Zenject;

namespace WeaponSystem
{
    public interface IWeapon
    {
        void Activate(SignalBus signalBus);
        void Deactivate(SignalBus signalBus);
        bool CanActivate();
        void UpgradeWeapon(int newDamage, float newCooldown);
        string GetName();
    }
}