using PlayerSystem.Signals.BattleSceneSignals;
using UnityEngine;
using WeaponSystem.Models;
using WeaponSystem.Services.Bases;
using Zenject;

namespace WeaponSystem.Services.Sub_Services.ControlledWeapon
{
    public class MeleeWeapon : WeaponBase
    {
        private readonly SignalBus _signalBus;
        private readonly MeleeAttackSignal _meleeAttackSignal;

        public MeleeWeapon(WeaponData data, SignalBus signalBus) : base(data)
        {
            _signalBus = signalBus;
            _meleeAttackSignal = new MeleeAttackSignal(data);
        }
        
        public override bool CanActivate()
        {
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
            _signalBus.Fire(_meleeAttackSignal);
        }
    }
}