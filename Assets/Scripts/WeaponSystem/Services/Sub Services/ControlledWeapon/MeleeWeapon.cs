using Signals.BattleSceneSignals;
using UnityEngine;
using WeaponSystem.Models;
using Zenject;

namespace WeaponSystem.Services.Sub_Services.ControlledWeapon
{
    public class MeleeWeapon : WeaponBase
    {
        public MeleeWeapon(WeaponData data) : base(data) { }

        public override bool CanActivate()
        {
            // For controlled weapons, check input
            // return Input.GetKeyDown(KeyCode.Space); // Example key
            return true;
        }

        public override void Activate(SignalBus signalBus)
        {
            signalBus.Subscribe<MeleeAttackSignal>(TriggerAttack);
            Debug.Log($"Activated weapon: {weaponData.name}");
        }

        public override void Deactivate(SignalBus signalBus)
        {
            signalBus.Unsubscribe<MeleeAttackSignal>(TriggerAttack);
            Debug.Log($"Deactivated weapon: {weaponData.name}");
        }

        private void TriggerAttack()
        {
            Debug.Log($"Attacked with Weapon {weaponData.name}");
        }
    }
}