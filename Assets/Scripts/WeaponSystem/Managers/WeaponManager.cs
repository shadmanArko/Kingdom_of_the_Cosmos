using System.Collections.Generic;
using System.Linq;
using DBMS.JsonToScriptable;
using DBMS.WeaponsData;
using UnityEngine;
using WeaponSystem.AutomaticWeapon;
using WeaponSystem.Models;
using WeaponSystem.Services.Sub_Services.ControlledWeapon;
using Zenject;
using zzz_TestScripts.Signals.BattleSceneSignals;

namespace WeaponSystem.Managers
{
    public class WeaponManager
    {
        private SignalBus _signalBus;
        private WeaponDatabaseScriptable _weaponDatabaseScriptable;
        private WeaponDataLoader _weaponDataLoader;

        private List<IWeapon> _controlledWeapons = new();
        private List<IWeapon> automaticWeapons = new();

        private IWeapon activeControlledWeapon;
        private int currentControlledIndex = 0;

        [Inject]
        public WeaponManager(SignalBus signalBus, WeaponDatabaseScriptable weaponDatabaseScriptable, WeaponDataLoader weaponDataLoader)
        {
            _signalBus = signalBus;
            _weaponDatabaseScriptable = weaponDatabaseScriptable;
            _weaponDataLoader = weaponDataLoader;
            SubscribeToActions();
            Start();
        }

        private void SubscribeToActions()
        {
            _signalBus.Subscribe<ReloadSignal>(Reload);
            _signalBus.Subscribe<SwitchControlledWeaponSignal>(HandleControlledWeaponSwitch);
        }

        private void Start()
        {
            Debug.Log($"signal bus is null {_signalBus == null}");
            // Assume you load data from JSON here
            LoadWeaponDataFromJson();
            NewTestControlledWeapon();
            
            _signalBus!.Subscribe<AutomaticWeaponTriggerSignal>(OnAutomaticWeaponTrigger);
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
            activeControlledWeapon.Deactivate(_signalBus);
            currentControlledIndex = (currentControlledIndex + 1) % _controlledWeapons.Count;
            activeControlledWeapon = _controlledWeapons[currentControlledIndex];
            activeControlledWeapon.Activate(_signalBus);
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

        private void LoadWeaponDataFromJson()
        {
            _weaponDataLoader.LoadWeaponData();
        }

        #region For Test
        
        private void NewTestControlledWeapon()
        {
            var weaponData = _weaponDatabaseScriptable.weaponDatabase.weaponCategories[0].weapons[0];
            var weaponData1 = _weaponDatabaseScriptable.weaponDatabase.weaponCategories[1].weapons[1];
            
            AddNewWeapon(weaponData);
            AddNewWeapon(weaponData1);
        }

        private void Reload()
        {
            if (_controlledWeapons.Count <= 0)
            {
                Debug.Log("No more weapons in equipped");
                return;
            }
            
            _controlledWeapons[^1].Deactivate(_signalBus);
            Debug.Log("Unsubscribe melee weapon");
        }

        #endregion
    }
}