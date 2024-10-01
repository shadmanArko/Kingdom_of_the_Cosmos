using UnityEngine;
using WeaponSystem;
using Zenject;

[CreateAssetMenu(fileName = "WeaponInstaller", menuName = "Installers/WeaponInstaller")]
public class WeaponInstaller : ScriptableObjectInstaller<WeaponInstaller>
{
    public override void InstallBindings()
    {
        Container.DeclareSignal<AutomaticWeaponTriggerSignal>();
        Container.Bind<WeaponManager>().AsSingle();
    }
}