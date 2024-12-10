using Player.Controllers;
using Player.Services;
using Player.Signals.BattleSceneSignals;
using Player.Views;
using UnityEngine;
using Zenject;

namespace Installers.PlayerInstaller
{
    [CreateAssetMenu(fileName = "PlayerInstaller", menuName = "Installers/PlayerInstaller")]
    public class PlayerInstaller : ScriptableObjectInstaller<PlayerInstaller>
    {
        [SerializeField] private GameObject playerView;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<PlayerController>().AsSingle().NonLazy();
            Container.Bind<PlayerView>().FromComponentInNewPrefab(playerView).AsSingle();
            
            //Services
            Container.BindInterfacesAndSelfTo<WeaponThrowService>().AsSingle();

            Container.DeclareSignal<PlayerMovementSignal>();
            Container.DeclareSignal<CancelHeavyAttackSignal>();
        }
    }
}