using System.Collections.Generic;

namespace WeaponSystem.WeaponModels
{
    public class WeaponManager
    {
        public List<ControlledWeapon> ControlledWeapons;
        public List<AutomaticWeapon> AutomaticWeapons;

        private IWeapon _activeControlledWeapon;
        private int _controlledWeaponIndex;

        private void Init()
        {
            
        }

        public void HandleControlledWeaponSwitch()
        {
            
        }

        public void OnAutoWeaponTrigger()
        {
            
        }

        public void AddNewAutoWeapon()
        {
            
        }

        public void UpgradeWeapon()
        {
            
        }
    }
}