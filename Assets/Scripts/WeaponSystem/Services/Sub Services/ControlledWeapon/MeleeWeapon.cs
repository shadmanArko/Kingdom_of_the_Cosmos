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
        private readonly MeleeLightAttackSignal _meleeLightAttackSignal;
        private readonly MeleeHeavyAttackSignal _meleeHeavyAttackSignal;

        public MeleeWeapon(WeaponData data, SignalBus signalBus) : base(data)
        {
            _signalBus = signalBus;
            _meleeLightAttackSignal = new MeleeLightAttackSignal(data);
            _meleeHeavyAttackSignal = new MeleeHeavyAttackSignal(data);
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
            
        }

        public override void Deactivate()
        {
            
        }
        
        #endregion
        public override void TriggerLightAttack()
        {
            _signalBus.Fire(_meleeLightAttackSignal);
        }

        public override void TriggerHeavyAttack()
        {
            _signalBus.Fire(_meleeHeavyAttackSignal);
        }
    }
}