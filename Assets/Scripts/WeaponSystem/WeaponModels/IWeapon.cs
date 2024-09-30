namespace WeaponSystem.WeaponModels
{
    public interface IWeapon
    {
        void Active();
        void Inactive();
        void CanActivate();
    }
}