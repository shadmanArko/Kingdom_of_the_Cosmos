using WeaponSystem.WeaponModels;

namespace WeaponSystem.ControlledWeapon
{
    public class MeleeWeapon : WeaponBase
    {
        public MeleeWeapon(WeaponData data) : base(data) { }

        public override bool CanActivate()
        {
            // For controlled weapons, check input
            // return Input.GetKeyDown(KeyCode.Space); // Example key
            return false;
        }
    }
}