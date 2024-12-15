using DBMS.WeaponsData;
using Player.Services;
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

        [SerializeField] private GameObject throwablePrefab;
            
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
        
            //Signals
            Container.DeclareSignal<AutomaticWeaponTriggerSignal>();
            
            Container.DeclareSignal<MeleeAttackSignal>();
            
            Container.DeclareSignal<StartHeavyAttackSignal>();
            Container.DeclareSignal<StopHeavyAttackSignal>();
            Container.DeclareSignal<HeavyAttackChargeMeterSignal>();
            
            Container.DeclareSignal<MouseMovementSignal>();
            Container.DeclareSignal<ReloadSignal>();
            Container.DeclareSignal<SwitchControlledWeaponSignal>();
            
            Container.DeclareSignal<DashInputStartSignal>();
            Container.DeclareSignal<DashInputStopSignal>();
            Container.DeclareSignal<DashPerformSignal>();
            
            Container.DeclareSignal<WeaponThrowStartSignal>();
            Container.DeclareSignal<WeaponThrowStopSignal>();
            Container.DeclareSignal<WeaponThrowChargeSignal>();
            Container.DeclareSignal<WeaponThrowCancelSignal>();
            Container.DeclareSignal<WeaponThrowCompletedSignal>();
            
            Container.DeclareSignal<ToggleAutoAttackSignal>();
        
            Container.Bind<WeaponManager>().AsSingle().NonLazy();
            Container.Bind<WeaponDataLoader>().AsSingle().NonLazy();

            //Systems
            Container.Bind<RicochetWeaponSystem.RicochetSystem>().FromComponentInNewPrefab(_ricochetSystem).AsSingle().NonLazy();

            Container.Bind<ThrowableWeaponView>().FromComponentInNewPrefab(throwablePrefab).AsSingle();
            //Services
            Container.BindInterfacesAndSelfTo<WeaponThrowService>().AsSingle();
        
            //Scriptables
            Container.Bind<WeaponDatabaseScriptable>().FromScriptableObject(weaponDatabaseScriptable).AsSingle().NonLazy();

            //Prefabs
            // Container.Bind<GameObject>().FromComponentInNewPrefab(throwablePrefab).AsSingle();
        }
    }
}