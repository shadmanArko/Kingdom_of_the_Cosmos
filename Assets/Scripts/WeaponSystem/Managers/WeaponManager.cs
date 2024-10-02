using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WeaponSystem.AutomaticWeapon;
using WeaponSystem.ControlledWeapon;
using WeaponSystem.WeaponModels;
using Zenject;

namespace WeaponSystem.Managers
{
    public class WeaponManager
    {
        [Inject] private SignalBus _signalBus;

        private List<IWeapon> _controlledWeapons = new();
        private List<IWeapon> automaticWeapons = new();

        private IWeapon activeControlledWeapon;
        private int currentControlledIndex = 0;

        [Inject]
        public WeaponManager(SignalBus signalBus)
        {
            _signalBus = signalBus;
            Start();
        }

        private void Start()
        {
            Debug.Log($"signal bus is null {_signalBus == null}");
            // Assume you load data from JSON here
            WeaponDatabase weaponDatabase = LoadWeaponDataFromJson();
            NewTestControlledWeapon();
            // foreach (var category in weaponDatabase.weaponCategories)
            // {
            //     foreach (var weaponData in category.weapons)
            //     {
            //         IWeapon weapon;
            //
            //         if (weaponData.type == "Controlled")
            //         {
            //             weapon = new MeleeWeapon(weaponData); // Or ProjectileWeapon, based on category
            //             _controlledWeapons.Add(weapon);
            //         }
            //         else if (weaponData.type == "Automatic")
            //         {
            //             weapon = new ProjectileWeapon(weaponData);
            //             automaticWeapons.Add(weapon);
            //         }
            //     }
            // }

            // Initialize first controlled weapon
            // if (_controlledWeapons.Count > 0)
            // {
            //     activeControlledWeapon = _controlledWeapons[0];
            //     activeControlledWeapon.Activate(_signalBus);
            // }

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

            if (weaponData.type == "Controlled")
            {
                newWeapon = new MeleeWeapon(weaponData); // Or ProjectileWeapon, based on category
                _controlledWeapons.Add(newWeapon);

                // If no controlled weapon is active, activate the new one
                if (activeControlledWeapon == null)
                {
                    activeControlledWeapon = newWeapon;
                    activeControlledWeapon.Activate(_signalBus);
                }
            }
            else if (weaponData.type == "Automatic")
            {
                newWeapon = new ProjectileWeapon(weaponData);
                automaticWeapons.Add(newWeapon);

                // Trigger the weapon immediately if it can activate
                if (newWeapon.CanActivate())
                {
                    newWeapon.Activate(_signalBus);
                }
            }
            
            Debug.Log("New weapon added: " + weaponData.name);
        }

        public void UpgradeControlledWeapon(string weaponName, int newDamage, float newCooldown)
        {
            // Check both controlled and automatic weapons
            IWeapon weaponToUpgrade = _controlledWeapons.Concat(automaticWeapons)
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
                activeControlledWeapon.Deactivate(_signalBus);
                currentControlledIndex = (currentControlledIndex + 1) % _controlledWeapons.Count;
                activeControlledWeapon = _controlledWeapons[currentControlledIndex];
                activeControlledWeapon.Activate(_signalBus);
            }
        }

        private void OnAutomaticWeaponTrigger(AutomaticWeaponTriggerSignal signal)
        {
            foreach (var weapon in automaticWeapons)
            {
                if (weapon.CanActivate())
                {
                    weapon.Activate(_signalBus);
                }
            }
        }

        private WeaponDatabase LoadWeaponDataFromJson()
        {
            // Implement JSON loading here
            return new WeaponDatabase();
        }

        #region For Test

        [ContextMenu("New Test Controlled Weapon")]
        private void NewTestControlledWeapon()
        {
            var weaponData = new WeaponData
            {
                cooldown = 1f,
                damage = 100,
                name = "Good Weapon",
                triggerCondition = "Good Condition",
                type = "Controlled"
            };
            
            AddNewWeapon(weaponData);
        }

        #endregion
    }
}