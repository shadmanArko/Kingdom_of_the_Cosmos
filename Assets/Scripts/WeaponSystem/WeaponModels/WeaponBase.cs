namespace WeaponSystem.WeaponModels
{
    public abstract class WeaponBase : IWeapon
    {
        public WeaponData WeaponData;
        public virtual void Active()
        {
            throw new System.NotImplementedException();
        }

        public virtual void Inactive()
        {
            throw new System.NotImplementedException();
        }

        public virtual void CanActivate()
        {
            throw new System.NotImplementedException();
        }
    }
}