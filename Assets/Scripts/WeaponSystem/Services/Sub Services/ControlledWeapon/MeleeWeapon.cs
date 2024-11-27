using Signals.BattleSceneSignals;
using UnityEngine;
using WeaponSystem.Models;
using Zenject;

namespace WeaponSystem.Services.Sub_Services.ControlledWeapon
{
    public class MeleeWeapon : WeaponBase
    {
        private SignalBus _signalBus;

        public MeleeWeapon(WeaponData data, SignalBus signalBus) : base(data)
        {
            _signalBus = signalBus;
        }
        
        public override bool CanActivate()
        {
            // For controlled weapons, check input
            // return Input.GetKeyDown(KeyCode.Space); // Example key
            return true;
        }

        public override bool CanAttack()
        {
            
            return true;
        }

        #region Activate and Deactivate

        public override void Activate()
        {
            // _signalBus.Subscribe<MeleeAttackSignal>(TriggerAttack);
            Debug.Log($"Activated weapon: {weaponData.name}");
        }

        public override void Deactivate()
        {
            // _signalBus.Unsubscribe<MeleeAttackSignal>(TriggerAttack);
            Debug.Log($"Deactivated weapon: {weaponData.name}");
        }
        
        #endregion
        public override void TriggerAttack()
        {
            Debug.Log($"Attacked with Weapon {weaponData.name}");
            _signalBus.Fire<MeleeAttackSignal>();
        }
    }
}