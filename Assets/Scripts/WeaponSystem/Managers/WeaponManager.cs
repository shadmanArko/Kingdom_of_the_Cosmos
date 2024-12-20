﻿using System.Collections.Generic;
using System.Linq;
using DBMS.RunningData;
using DBMS.WeaponsData;
using Player;
using Player.Signals.BattleSceneSignals;
using RicochetWeaponSystem;
using UnityEngine;
using WeaponSystem.Models;
using WeaponSystem.Services.Interfaces;
using WeaponSystem.Services.Sub_Services.AutomaticWeapon;
using WeaponSystem.Services.Sub_Services.ControlledWeapon;
using WeaponSystem.Signals;
using Zenject;

namespace WeaponSystem.Managers
{
    public class WeaponManager
    {
        private SignalBus _signalBus;
        private WeaponDatabaseScriptable _weaponDatabaseScriptable;
        private RunningDataScriptable _runningDataScriptable;
        private WeaponDataLoader _weaponDataLoader;
        private PlayerController _playerController;

        private List<IWeapon> _controlledWeapons = new();
        private List<IWeapon> automaticWeapons = new();

        private IWeapon _activeControlledWeapon;
        private int _currentControlledIndex;

        // private RayCastSystem _rayCastSystem;
        private RicochetWeaponSystem.RicochetSystem _ricochetSystem;
        
        [Inject]
        public WeaponManager(SignalBus signalBus, WeaponDatabaseScriptable weaponDatabaseScriptable, WeaponDataLoader weaponDataLoader, RunningDataScriptable runningDataScriptable, PlayerController playerController, RicochetWeaponSystem.RicochetSystem ricochetSystem)
        {
            _signalBus = signalBus;
            _weaponDatabaseScriptable = weaponDatabaseScriptable;
            _runningDataScriptable = runningDataScriptable;
            _weaponDataLoader = weaponDataLoader;
            _playerController = playerController;
            _ricochetSystem = ricochetSystem;
            SubscribeToActions();
            Start();
        }

        private void SubscribeToActions()
        {
            _signalBus.Subscribe<ReloadSignal>(Reload);
            _signalBus.Subscribe<SwitchControlledWeaponSignal>(HandleControlledWeaponSwitch);
            _signalBus.Subscribe<WeaponThrowStartSignal>(StartWeaponThrow);
            _signalBus.Subscribe<WeaponThrowStopSignal>(StopWeaponThrow);
            _signalBus.Subscribe<AutomaticWeaponTriggerSignal>(OnAutomaticWeaponTrigger);
        }

        private void UnsubscribeToActions()
        {
            _signalBus.Unsubscribe<ReloadSignal>(Reload);
            _signalBus.Unsubscribe<SwitchControlledWeaponSignal>(HandleControlledWeaponSwitch);
            _signalBus.Unsubscribe<WeaponThrowStartSignal>(StartWeaponThrow);
            _signalBus.Unsubscribe<WeaponThrowStopSignal>(StopWeaponThrow);
            _signalBus.Unsubscribe<AutomaticWeaponTriggerSignal>(OnAutomaticWeaponTrigger);
        }

        private void Start()
        {
            LoadWeaponDataFromJson();
            NewTestControlledWeapon();

            _currentControlledIndex = 0;
        }

        public void AddNewWeapon(WeaponData weaponData)
        {
            IWeapon newWeapon;

            if (weaponData.type == "Controlled")
            {
                newWeapon = new MeleeWeapon(weaponData, _signalBus); // Or ProjectileWeapon, based on category
                _controlledWeapons.Add(newWeapon);

                // If no controlled weapon is active, activate the new one
                if (_activeControlledWeapon == null)
                {
                    _activeControlledWeapon = newWeapon;
                    _activeControlledWeapon.Activate();
                }
            }
            else if (weaponData.type == "Automatic")
            {
                newWeapon = new ProjectileWeapon(weaponData, _signalBus);
                automaticWeapons.Add(newWeapon);

                // Trigger the weapon immediately if it can activate
                if (newWeapon.CanActivate())
                {
                    newWeapon.Activate();
                }
            }
            
            Debug.Log("New weapon added: " + weaponData.name);
        }

        #region Upgrade Weapon

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

        #endregion
        
        private void HandleControlledWeaponSwitch()
        {
            _activeControlledWeapon.Deactivate();
            _currentControlledIndex = (_currentControlledIndex + 1) % _controlledWeapons.Count;
            _activeControlledWeapon = _controlledWeapons[_currentControlledIndex];
            _activeControlledWeapon.Activate();
        }

        public bool TriggerControlledWeapon()
        {
            if(!_controlledWeapons[_currentControlledIndex].CanAttack()) return false;
            _controlledWeapons[_currentControlledIndex].TriggerAttack();
            return true;
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
            
            _controlledWeapons[^1].Deactivate();
            Debug.Log("Unsubscribe melee weapon");
        }

        [SerializeField] private List<DummyEnemy> enemies;
        private void StartWeaponThrow()
        {
            var mouseDirection = _runningDataScriptable.attackDirection;
            List<RicochetHitInfo> hits = _ricochetSystem.CalculateRicochetPath(_playerController.transform.position, mouseDirection);
            Debug.Log("Casting Ray");
            Debug.Log($"Hit count: {hits.Count}");
            foreach (var hit in hits)
            {
                if (!hit.HitDummyEnemy.IsShielded)
                {
                    //TODO: Apply Damage
                    Debug.Log("Targeting non-shielded enemy");
                }
                else
                {
                    Debug.Log("RICOCHET");
                }
            }
        }

        private void StopWeaponThrow()
        {
            
        }

        #endregion
        
    }
}