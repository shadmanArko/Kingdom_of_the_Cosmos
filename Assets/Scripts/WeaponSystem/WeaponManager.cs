using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WeaponSystem.AutomaticWeapon;
using WeaponSystem.ControlledWeapon;
using WeaponSystem.WeaponModels;
using Zenject;

namespace WeaponSystem
{
    public class WeaponManager
    {
        [Inject] private SignalBus _signalBus;

        private List<IWeapon> controlledWeapons = new List<IWeapon>();
        private List<IWeapon> automaticWeapons = new List<IWeapon>();

        private IWeapon activeControlledWeapon;
        private int currentControlledIndex = 0;

        private void Start()
        {
            // Assume you load data from JSON here
            WeaponDatabase weaponDatabase = LoadWeaponDataFromJson();

            foreach (var category in weaponDatabase.WeaponCategories)
            {
                foreach (var weaponData in category.Weapons)
                {
                    IWeapon weapon = null;

                    if (weaponData.Type == "Controlled")
                    {
                        weapon = new MeleeWeapon(weaponData); // Or ProjectileWeapon, based on category
                        controlledWeapons.Add(weapon);
                    }
                    else if (weaponData.Type == "Automatic")
                    {
                        weapon = new ProjectileWeapon(weaponData);
                        automaticWeapons.Add(weapon);
                    }
                }
            }

            // Initialize first controlled weapon
            if (controlledWeapons.Count > 0)
            {
                activeControlledWeapon = controlledWeapons[0];
                activeControlledWeapon.Activate();
            }

            // Subscribe to signal bus for automatic triggers
            _signalBus.Subscribe<AutomaticWeaponTriggerSignal>(OnAutomaticWeaponTrigger);
        }

        private void Update()
        {
            HandleControlledWeaponSwitch();
        }

        public void AddNewWeapon(WeaponData weaponData)
        {
            IWeapon newWeapon;

            if (weaponData.Type == "Controlled")
            {
                newWeapon = new MeleeWeapon(weaponData); // Or ProjectileWeapon, based on category
                controlledWeapons.Add(newWeapon);

                // If no controlled weapon is active, activate the new one
                if (activeControlledWeapon == null)
                {
                    activeControlledWeapon = newWeapon;
                    activeControlledWeapon.Activate();
                }
            }
            else if (weaponData.Type == "Automatic")
            {
                newWeapon = new ProjectileWeapon(weaponData);
                automaticWeapons.Add(newWeapon);

                // Trigger the weapon immediately if it can activate
                if (newWeapon.CanActivate())
                {
                    newWeapon.Activate();
                }
            }


            Debug.Log("New weapon added: " + weaponData.Name);
        }

        public void UpgradeControlledWeapon(string weaponName, int newDamage, float newCooldown)
        {
            // Check both controlled and automatic weapons
            IWeapon weaponToUpgrade = controlledWeapons.Concat(automaticWeapons)
                .FirstOrDefault(w => w.GetName() == weaponName);

            if (weaponToUpgrade != null)
            {
                weaponToUpgrade.UpgradeWeapon(newDamage, newCooldown);
            }
        }

        public void UpgradeAutomaticWeapon(string weaponName, int newDamage, float newCooldown)
        {
            // Check both controlled and automatic weapons
            IWeapon weaponToUpgrade =
                automaticWeapons.Concat(automaticWeapons).FirstOrDefault(w => w.GetName() == weaponName);

            if (weaponToUpgrade != null)
            {
                weaponToUpgrade.UpgradeWeapon(newDamage, newCooldown);
            }
        }


        private void HandleControlledWeaponSwitch()
        {
            if (Input.GetKeyDown(KeyCode.Tab)) // Example switch key
            {
                activeControlledWeapon.Deactivate();
                currentControlledIndex = (currentControlledIndex + 1) % controlledWeapons.Count;
                activeControlledWeapon = controlledWeapons[currentControlledIndex];
                activeControlledWeapon.Activate();
            }
        }

        private void OnAutomaticWeaponTrigger(AutomaticWeaponTriggerSignal signal)
        {
            foreach (var weapon in automaticWeapons)
            {
                if (weapon.CanActivate())
                {
                    weapon.Activate();
                }
            }
        }

        private WeaponDatabase LoadWeaponDataFromJson()
        {
            // Implement JSON loading here
            return new WeaponDatabase();
        }
    }
}