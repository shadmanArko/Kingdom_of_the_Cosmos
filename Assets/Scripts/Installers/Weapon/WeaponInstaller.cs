using DBMS.WeaponsData;
using PlayerSystem.Services;
using PlayerSystem.Signals.BattleSceneSignals;
using PlayerSystem.Signals.InputSignals;
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
        [SerializeField] private WeaponDatabaseScriptable weaponDatabaseScriptable;
        [SerializeField] private RicochetWeaponSystem.RicochetSystem _ricochetSystem;

        [SerializeField] private GameObject throwablePrefab;
            
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
        
            //Signals
            Container.DeclareSignal<AutomaticWeaponTriggerSignal>();

            Container.DeclareSignal<LightAttackInputSignal>();
            Container.DeclareSignal<MeleeLightAttackSignal>();
            
            Container.DeclareSignal<StartHeavyAttackInputSignal>();
            Container.DeclareSignal<StopHeavyAttackInputSignal>();
            Container.DeclareSignal<HeavyAttackChargeMeterSignal>();
            
            Container.DeclareSignal<MouseMovementSignal>();
            Container.DeclareSignal<ReloadSignal>();
            Container.DeclareSignal<SwitchControlledWeaponInputSignal>();
            
            Container.DeclareSignal<DashStartInputSignal>();
            Container.DeclareSignal<DashStopInputSignal>();
            Container.DeclareSignal<DashPerformSignal>();
            
            Container.DeclareSignal<WeaponThrowStartInputSignal>();
            Container.DeclareSignal<WeaponThrowStopInputSignal>();
            Container.DeclareSignal<WeaponThrowChargeSignal>();
            Container.DeclareSignal<WeaponThrowCancelSignal>();
            Container.DeclareSignal<WeaponThrowCompletedSignal>();
            
            Container.DeclareSignal<ToggleAutoAttackInputSignal>();
        
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