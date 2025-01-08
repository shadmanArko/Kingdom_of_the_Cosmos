using System;
using PlayerSystem.Controllers;
using PlayerSystem.PlayerSO;
using PlayerSystem.Services.HealthService;
using PlayerSystem.Signals.BattleSceneSignals;
using PlayerSystem.Views;
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
        [SerializeField] private PlayerHealthView playerHealthView;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<PlayerController>().AsSingle().NonLazy();
            
            //Scriptables
            Container.Bind<PlayerScriptableObject>().FromScriptableObject(playerScriptableObject).AsSingle().NonLazy();
            
            //Views
            Container.Bind<PlayerView>().FromComponentInNewPrefab(playerView).AsSingle();
            Container.Bind<PlayerHealthView>().FromComponentInNewPrefab(playerHealthView).AsSingle();
            
            //Services
            Container.Bind<PlayerHealthService>().AsSingle();

            //Signals
            Container.DeclareSignal<PlayerMovementSignal>();
            Container.DeclareSignal<CancelHeavyAttackSignal>();
        }
    }
}