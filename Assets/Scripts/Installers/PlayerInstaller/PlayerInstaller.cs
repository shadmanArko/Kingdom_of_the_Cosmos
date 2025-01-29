using System;
using Experience;
using PlayerSystem.Controllers;
using PlayerSystem.PlayerSO;
using PlayerSystem.Services.HealthService;
using PlayerSystem.Signals.BattleSceneSignals;
using PlayerSystem.Views;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Installers.PlayerInstaller
{
    [CreateAssetMenu(fileName = "PlayerInstaller", menuName = "Installers/PlayerInstaller")]
    public class PlayerInstaller : ScriptableObjectInstaller<PlayerInstaller>
    {
        [SerializeField] private PlayerScriptableObject playerScriptableObject;
        
        [SerializeField] private GameObject playerView;
        [FormerlySerializedAs("playerStatView")] [FormerlySerializedAs("playerHealthView")] [SerializeField] private PlayerStatusUiView playerStatusUiView;
        [SerializeField] private ExpView expView;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<PlayerController>().AsSingle().NonLazy();
            
            //Scriptables
            Container.Bind<PlayerScriptableObject>().FromScriptableObject(playerScriptableObject).AsSingle().NonLazy();
            
            //Views
            Container.Bind<PlayerView>().FromComponentInNewPrefab(playerView).AsSingle();
            Container.Bind<PlayerStatusUiView>().FromComponentInNewPrefab(playerStatusUiView).AsSingle();
            // Container.Bind<ExpView>().FromComponentInNewPrefab(expView).AsSingle();

            Container.Bind<CompositeDisposable>().AsSingle();
            
            //Controllers
            Container.Bind<ExpController>().AsSingle();
            
            //Services
            Container.Bind<PlayerHealthService>().AsSingle();

            //Models
            Container.Bind<ExpModel>().AsSingle();
            
            
            //Signals
            Container.DeclareSignal<PlayerMovementSignal>();
            Container.DeclareSignal<CancelHeavyAttackSignal>();
        }
    }
}