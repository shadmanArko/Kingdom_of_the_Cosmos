using DBMS.WeaponsData;
using Signals.BattleSceneSignals;
using UnityEngine;
using UnityEngine.Serialization;
using WeaponSystem;
using WeaponSystem.Managers;
using Zenject;
using zzz_TestScripts.Signals.BattleSceneSignals;

[CreateAssetMenu(fileName = "WeaponInstaller", menuName = "Installers/WeaponInstaller")]
public class WeaponInstaller : ScriptableObjectInstaller<WeaponInstaller>
{
    [FormerlySerializedAs("weaponDataScriptable")] [SerializeField] private WeaponDatabaseScriptable weaponDatabaseScriptable;
    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);
        
        Container.DeclareSignal<AutomaticWeaponTriggerSignal>();
        Container.DeclareSignal<MeleeAttackSignal>();
        Container.DeclareSignal<MouseMovementSignal>();
        Container.DeclareSignal<ReloadSignal>();
        Container.DeclareSignal<SwitchControlledWeaponSignal>();
        
        Container.Bind<WeaponManager>().AsSingle().NonLazy();
        Container.Bind<WeaponDataLoader>().AsSingle().NonLazy();
        
        
        Container.Bind<WeaponDatabaseScriptable>().FromScriptableObject(weaponDatabaseScriptable).AsSingle().NonLazy();
    }
}