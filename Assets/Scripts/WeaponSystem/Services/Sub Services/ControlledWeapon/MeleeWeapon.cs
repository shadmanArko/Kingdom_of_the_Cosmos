using Signals.BattleSceneSignals;
using UnityEngine;
using WeaponSystem.WeaponModels;
using Zenject;

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

        public override void Activate(SignalBus signalBus)
        {
            signalBus.Subscribe<MeleeAttackSignal>(TriggerAttack);
        }

        public override void Deactivate(SignalBus signalBus)
        {
            signalBus.Subscribe<MeleeAttackSignal>(TriggerAttack);
        }

        private void TriggerAttack()
        {
            Debug.Log("Melee attack triggered manually");
        }
    }
}