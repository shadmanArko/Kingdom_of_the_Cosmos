using Cinemachine;
using GameData;
using ObjectPool;
using PlayerScripts;
using SaveAndLoad;
using UnityEngine;
using Utilities;
using Zenject;

namespace Installers
{
    [CreateAssetMenu(fileName = "BattleSceneInstaller", menuName = "Installers/BattleSceneInstaller")]
    public class BattleSceneInstaller : ScriptableObjectInstaller<BattleSceneInstaller>
    {
        [SerializeField] private GameDataScriptable gameDataScriptable;
        [SerializeField] private SaveDataScriptable saveDataScriptable;
    
        [SerializeField] private CinemachineVirtualCamera cineMachineVirtualCamera;
        [SerializeField] private PlayerController playerController;

        [SerializeField] private BulletPoolingManager bulletPoolingManager;
    
        public override void InstallBindings()
        {
            Container.Bind<GameDataScriptable>().FromInstance(gameDataScriptable).AsSingle();
            Container.Bind<SaveDataScriptable>().FromInstance(saveDataScriptable).AsSingle();
            Container.Bind<GameDataLoader>().AsTransient().NonLazy();
            Container.Bind<SaveAndLoadManager>().AsTransient().NonLazy();

            Container.Bind<CinemachineVirtualCamera>().FromComponentInNewPrefab(cineMachineVirtualCamera).AsSingle();
            Container.Bind<ScreenShakeManager>().AsSingle();
            Container.Bind<PlayerController>().FromComponentInNewPrefab(playerController).AsSingle().NonLazy();
            Container.Bind<BulletPoolingManager>().FromComponentInNewPrefab(bulletPoolingManager).AsSingle();
        }
    }
}