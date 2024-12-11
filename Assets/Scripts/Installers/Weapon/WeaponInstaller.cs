using DBMS.WeaponsData;
using Player.Signals.BattleSceneSignals;
using UnityEngine;
using UnityEngine.Serialization;
using WeaponSystem.Managers;
using WeaponSystem.Signals;
using Zenject;

namespace Installers.Weapon
{
    [CreateAssetMenu(fileName = "WeaponInstaller", menuName = "Installers/WeaponInstaller")]
    public class WeaponInstaller : ScriptableObjectInstaller<WeaponInstaller>
    {
        [FormerlySerializedAs("weaponDataScriptable")] [SerializeField] private WeaponDatabaseScriptable weaponDatabaseScriptable;
        [SerializeField] private RicochetWeaponSystem.RicochetSystem _ricochetSystem;
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
        
            Container.DeclareSignal<AutomaticWeaponTriggerSignal>();
            Container.DeclareSignal<MeleeAttackSignal>();
            Container.DeclareSignal<StartHeavyAttackSignal>();
            Container.DeclareSignal<StopHeavyAttackSignal>();
            Container.DeclareSignal<HeavyAttackAngleIncrementSignal>();
            Container.DeclareSignal<MouseMovementSignal>();
            Container.DeclareSignal<ReloadSignal>();
            Container.DeclareSignal<SwitchControlledWeaponSignal>();
            Container.DeclareSignal<StartDashSignal>();
            Container.DeclareSignal<StopDashSignal>();
            Container.DeclareSignal<WeaponThrowStartSignal>();
            Container.DeclareSignal<WeaponThrowStopSignal>();
            Container.DeclareSignal<ToggleAutoAttackSignal>();
        
            Container.Bind<WeaponManager>().AsSingle().NonLazy();
            Container.Bind<WeaponDataLoader>().AsSingle().NonLazy();

            Container.Bind<RicochetWeaponSystem.RicochetSystem>().FromComponentInNewPrefab(_ricochetSystem).AsSingle()
                .NonLazy();
        
        
            Container.Bind<WeaponDatabaseScriptable>().FromScriptableObject(weaponDatabaseScriptable).AsSingle().NonLazy();
        }
    }
}