using Player.Controllers;
using Player.Services;
using Player.Signals.BattleSceneSignals;
using Player.Views;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Installers.PlayerInstaller
{
    [CreateAssetMenu(fileName = "PlayerInstaller", menuName = "Installers/PlayerInstaller")]
    public class PlayerInstaller : ScriptableObjectInstaller<PlayerInstaller>
    {
        [SerializeField] private GameObject playerView;
        [SerializeField] private PlayerHealthView playerHealthView;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<PlayerController>().AsSingle().NonLazy();
            Container.Bind<PlayerView>().FromComponentInNewPrefab(playerView).AsSingle();
            Container.Bind<PlayerHealthView>().FromComponentInNewPrefab(playerHealthView).AsSingle();
            
            //Services

            Container.DeclareSignal<PlayerMovementSignal>();
            Container.DeclareSignal<CancelHeavyAttackSignal>();
        }
    }
}