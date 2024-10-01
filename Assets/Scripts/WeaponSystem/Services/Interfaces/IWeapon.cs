namespace WeaponSystem
{
    public interface IWeapon
    {
        void Activate();
        void Deactivate();
        bool CanActivate();
        void UpgradeWeapon(int newDamage, float newCooldown);
        string GetName();
    }
}